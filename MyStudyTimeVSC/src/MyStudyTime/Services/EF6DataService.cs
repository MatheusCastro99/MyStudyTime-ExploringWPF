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
                System.Diagnostics.Debug.WriteLine($"[SaveSubjects] Called with {subjects.Count} subjects");

                foreach (var subj in subjects)
                {
                    string msg = $"  Subject: {subj.Name} (ID={subj.Id}), Notes={subj.Notes?.Count ?? 0}, FlashCards={subj.FlashCards?.Count ?? 0}";
                    System.Diagnostics.Debug.WriteLine(msg);
                }

                using (var context = new StudyTimeDbContext())
                {
                    // First, handle new subjects (must save first to get their IDs)
                    foreach (var subject in subjects.Where(s => s.Id == 0).ToList())
                    {
                        string msg = $"[SaveSubjects] Adding new subject: {subject.Name}";
                        System.Diagnostics.Debug.WriteLine(msg);
                        context.Subjects.Add(subject);
                    }

                    // Save new subjects to generate their IDs
                    context.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("[SaveSubjects] New subjects saved, IDs generated");

                    // Now update the subjects in the collection with their generated IDs
                    var newSubjectsInDb = context.Subjects.ToList();
                    foreach (var subject in subjects.Where(s => s.Id == 0).ToList())
                    {
                        var dbSubject = newSubjectsInDb.FirstOrDefault(s => s.Name == subject.Name && s.CreatedDate == subject.CreatedDate);
                        if (dbSubject != null)
                        {
                            string msg = $"[SaveSubjects] Updated {subject.Name} ID from 0 to {dbSubject.Id}";
                            System.Diagnostics.Debug.WriteLine(msg);
                            subject.Id = dbSubject.Id;
                        }
                    }

                    // Now handle all subjects (existing and newly saved)
                    foreach (var subject in subjects)
                    {
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
                            foreach (var note in notesToAdd)
                            {
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
                            foreach (var card in cardsToAdd)
                            {
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
                    System.Diagnostics.Debug.WriteLine("[SaveSubjects] Calling final SaveChanges()");
                    context.SaveChanges();
                    System.Diagnostics.Debug.WriteLine("[SaveSubjects] Final SaveChanges() completed successfully");
                }
            }
            catch (Exception ex)
            {
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
