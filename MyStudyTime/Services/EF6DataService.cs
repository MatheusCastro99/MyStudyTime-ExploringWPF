using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using MyStudyTime.Database;
using MyStudyTime.MVVM.Model;

namespace MyStudyTime.Services
{
    public class EF6DataService : IDataService
    {
        private StudyTimeDbContext _context;

        public EF6DataService()
        {
            _context = new StudyTimeDbContext();
        }

        public ObservableCollection<Subject> LoadSubjects()
        {
            try
            {
                var subjects = _context.Subjects
                    .Include(s => s.Notes)
                    .Include(s => s.FlashCards)
                    .ToList();

                var observableSubjects = new ObservableCollection<Subject>();
                foreach (var subject in subjects)
                {
                    observableSubjects.Add(subject);
                }
                return observableSubjects;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading subjects: {ex.Message}");
                return new ObservableCollection<Subject>();
            }
        }

        public void SaveSubjects(ObservableCollection<Subject> subjects)
        {
            try
            {
                string logPath = @"C:\Users\mathe\Documents\Programming\MyStudyTime\MyStudyTime\debug_log.txt";
                System.Diagnostics.Debug.WriteLine($"[SaveSubjects] Called with {subjects.Count} subjects");
                File.AppendAllText(logPath, $"\n[SaveSubjects] Called with {subjects.Count} subjects\n");

                foreach (var subj in subjects)
                {
                    string msg = $"  Subject: {subj.Name} (ID={subj.Id}), Notes={subj.Notes?.Count ?? 0}, FlashCards={subj.FlashCards?.Count ?? 0}";
                    System.Diagnostics.Debug.WriteLine(msg);
                    File.AppendAllText(logPath, msg + "\n");
                }

                using (var context = new StudyTimeDbContext())
                {
                    // Log call in Diagnostics table for debugging
                    try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", $"[SaveSubjects] Called with {subjects.Count} subjects"); } catch { }
                    // First, handle new subjects (must save first to get their IDs)
                    foreach (var subject in subjects.Where(s => s.Id == 0).ToList())
                    {
                        string msg = $"[SaveSubjects] Adding new subject: {subject.Name}";
                        System.Diagnostics.Debug.WriteLine(msg);
                        File.AppendAllText(logPath, msg + "\n");
                        context.Subjects.Add(subject);
                    }

                    // Save new subjects to generate their IDs
                    context.SaveChanges();
                    try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", "[SaveSubjects] New subjects saved, IDs generated"); } catch { }
                    File.AppendAllText(logPath, "[SaveSubjects] New subjects saved, IDs generated\n");

                    // Now update the subjects in the collection with their generated IDs
                    var newSubjectsInDb = context.Subjects.ToList();
                    foreach (var subject in subjects.Where(s => s.Id == 0).ToList())
                    {
                        var dbSubject = newSubjectsInDb.FirstOrDefault(s => s.Name == subject.Name && s.CreatedDate == subject.CreatedDate);
                        if (dbSubject != null)
                        {
                            string msg = $"[SaveSubjects] Updated {subject.Name} ID from 0 to {dbSubject.Id}";
                            System.Diagnostics.Debug.WriteLine(msg);
                            try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", msg); } catch { }
                            File.AppendAllText(logPath, msg + "\n");
                            subject.Id = dbSubject.Id;
                        }
                    }

                    // Now handle all subjects (existing and newly saved)
                    foreach (var subject in subjects)
                    {
                        string msg = $"[SaveSubjects] Processing subject: {subject.Name} (ID={subject.Id}), Notes count={subject.Notes?.Count ?? 0}";
                        try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", msg); } catch { }
                        File.AppendAllText(logPath, msg + "\n");
                        var dbSubject = context.Subjects.Find(subject.Id);

                        if (dbSubject != null)
                        {
                            // Update existing subject properties
                            dbSubject.Name = subject.Name;
                            dbSubject.CreatedDate = subject.CreatedDate;

                            // Handle notes
                            var dbNotes = context.Notes.Where(n => n.SubjectId == subject.Id).ToList();

                            // Add new notes
                            var notesToAdd = subject.Notes.Where(n => n.Id == 0).ToList();
                            string notesMsg = $"[SaveSubjects]   Notes to add: {notesToAdd.Count}";
                            try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", notesMsg); } catch { }
                            File.AppendAllText(logPath, notesMsg + "\n");
                            foreach (var note in notesToAdd)
                            {
                                string noteMsg = $"[SaveSubjects]     Adding note: {note.Text}";
                                try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", noteMsg); } catch { }
                                File.AppendAllText(logPath, noteMsg + "\n");
                                note.SubjectId = subject.Id;
                                context.Notes.Add(note);
                            }

                            // Remove deleted notes
                            foreach (var dbNote in dbNotes.Where(dn => !subject.Notes.Any(n => n.Id == dn.Id)))
                            {
                                context.Notes.Remove(dbNote);
                            }

                            // Update existing notes
                            foreach (var note in subject.Notes.Where(n => n.Id != 0))
                            {
                                var dbNote = context.Notes.Find(note.Id);
                                if (dbNote != null)
                                {
                                    dbNote.Text = note.Text;
                                    dbNote.Tags = note.Tags;
                                    dbNote.CreatedDate = note.CreatedDate;
                                }
                            }

                            // Handle flash cards
                            var dbCards = context.FlashCards.Where(f => f.SubjectId == subject.Id).ToList();

                            // Add new cards
                            var cardsToAdd = subject.FlashCards.Where(f => f.Id == 0).ToList();
                            string cardsMsg = $"[SaveSubjects]   FlashCards to add: {cardsToAdd.Count}";
                            try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", cardsMsg); } catch { }
                            File.AppendAllText(logPath, cardsMsg + "\n");
                            foreach (var card in cardsToAdd)
                            {
                                string cardMsg = $"[SaveSubjects]     Adding card: {card.Question}";
                                try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", cardMsg); } catch { }
                                File.AppendAllText(logPath, cardMsg + "\n");
                                card.SubjectId = subject.Id;
                                context.FlashCards.Add(card);
                            }

                            // Remove deleted cards
                            foreach (var dbCard in dbCards.Where(dc => !subject.FlashCards.Any(f => f.Id == dc.Id)))
                            {
                                context.FlashCards.Remove(dbCard);
                            }

                            // Update existing cards
                            foreach (var card in subject.FlashCards.Where(f => f.Id != 0))
                            {
                                var dbCard = context.FlashCards.Find(card.Id);
                                if (dbCard != null)
                                {
                                    dbCard.Question = card.Question;
                                    dbCard.Answer = card.Answer;
                                    dbCard.Difficulty = card.Difficulty;
                                    dbCard.Tags = card.Tags;
                                    dbCard.ReviewCount = card.ReviewCount;
                                    dbCard.GotRight = card.GotRight;
                                    dbCard.LastReviewDate = card.LastReviewDate;
                                    dbCard.CreatedDate = card.CreatedDate;
                                }
                            }
                        }
                    }

                    // Remove subjects that were deleted (where ID is not 0)
                    var subjectIds = subjects.Select(s => s.Id).ToList();
                    var subjectsToRemove = context.Subjects
                        .Where(s => !subjectIds.Contains(s.Id))
                        .ToList();

                    foreach (var subject in subjectsToRemove)
                    {
                        context.Subjects.Remove(subject);
                    }

                    // Final save for all changes
                    try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", "[SaveSubjects] Calling final SaveChanges()"); } catch { }
                    File.AppendAllText(logPath, "[SaveSubjects] Calling final SaveChanges()\n");
                    context.SaveChanges();
                    try { context.Database.ExecuteSqlCommand("INSERT INTO Diagnostics (Message) VALUES({0})", "[SaveSubjects] Final SaveChanges() completed successfully"); } catch { }
                    File.AppendAllText(logPath, "[SaveSubjects] Final SaveChanges() completed successfully\n");
                }
            }
            catch (Exception ex)
            {
                string logPath = @"C:\Users\mathe\Documents\Programming\MyStudyTime\MyStudyTime\debug_log.txt";
                File.AppendAllText(logPath, $"Error saving subjects: {ex.Message}\n{ex.StackTrace}\n");
                System.Diagnostics.Debug.WriteLine($"Error saving subjects: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public ObservableCollection<FlashCard> LoadFlashCards()
        {
            try
            {
                var flashCards = _context.FlashCards.ToList();
                var observableCards = new ObservableCollection<FlashCard>();
                foreach (var card in flashCards)
                {
                    observableCards.Add(card);
                }
                return observableCards;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading flash cards: {ex.Message}");
                return new ObservableCollection<FlashCard>();
            }
        }

        public void SaveFlashCards(ObservableCollection<FlashCard> flashCards)
        {
            try
            {
                using (var context = new StudyTimeDbContext())
                {
                    foreach (var card in flashCards)
                    {
                        var existingCard = context.FlashCards.Find(card.Id);
                        if (existingCard == null)
                        {
                            context.FlashCards.Add(card);
                        }
                        else
                        {
                            context.Entry(existingCard).CurrentValues.SetValues(card);
                        }
                    }

                    var cardsToRemove = context.FlashCards
                        .Where(f => !flashCards.Any(fc => fc.Id == f.Id))
                        .ToList();

                    foreach (var card in cardsToRemove)
                    {
                        context.FlashCards.Remove(card);
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving flash cards: {ex.Message}");
            }
        }

        public ObservableCollection<StudyGoal> LoadStudyGoals()
        {
            // StudyGoals are not implemented in current schema
            return new ObservableCollection<StudyGoal>();
        }

        public void SaveStudyGoals(ObservableCollection<StudyGoal> goals)
        {
            // StudyGoals are not implemented in current schema
            System.Diagnostics.Debug.WriteLine("SaveStudyGoals called but not implemented in EF6 schema");
        }
    }
}
