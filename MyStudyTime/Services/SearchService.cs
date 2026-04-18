using MyStudyTime.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyStudyTime.Services
{
    public interface ISearchService
    {
        List<SearchResult> SearchNotes(string query, ObservableCollection<Subject> subjects);
        List<SearchResult> SearchFlashCards(string query, ObservableCollection<Subject> subjects);
        List<SearchResult> SearchAll(string query, ObservableCollection<Subject> subjects);
    }

    public class SearchResult
    {
        public string Type { get; set; } // "Note" or "FlashCard"
        public string SubjectName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public object OriginalObject { get; set; }
    }

    public class SearchService : ISearchService
    {
        public List<SearchResult> SearchNotes(string query, ObservableCollection<Subject> subjects)
        {
            var results = new List<SearchResult>();
            var searchTerm = query.ToLower();

            foreach (var subject in subjects)
            {
                foreach (var note in subject.Notes)
                {
                    var noteContent = note.Text?.ToLower() ?? "";
                    var tags = (note.Tags ?? "").ToLower();

                    if (noteContent.Contains(searchTerm) || tags.Contains(searchTerm))
                    {
                        results.Add(new SearchResult
                        {
                            Type = "Note",
                            SubjectName = subject.Name,
                            Title = $"Note in {subject.Name}",
                            Content = note.Text,
                            Date = note.CreatedDate,
                            OriginalObject = note
                        });
                    }
                }
            }

            return results.OrderByDescending(r => r.Date).ToList();
        }

        public List<SearchResult> SearchFlashCards(string query, ObservableCollection<Subject> subjects)
        {
            var results = new List<SearchResult>();
            var searchTerm = query.ToLower();

            foreach (var subject in subjects)
            {
                foreach (var card in subject.FlashCards)
                {
                    var question = card.Question?.ToLower() ?? "";
                    var answer = card.Answer?.ToLower() ?? "";
                    var tags = (card.Tags ?? "").ToLower();

                    if (question.Contains(searchTerm) || answer.Contains(searchTerm) || tags.Contains(searchTerm))
                    {
                        results.Add(new SearchResult
                        {
                            Type = "FlashCard",
                            SubjectName = subject.Name,
                            Title = card.Question,
                            Content = card.Answer,
                            Date = card.LastReviewDate.HasValue ? card.LastReviewDate.Value : DateTime.Now,
                            OriginalObject = card
                        });
                    }
                }
            }

            return results.OrderByDescending(r => r.Date).ToList();
        }

        public List<SearchResult> SearchAll(string query, ObservableCollection<Subject> subjects)
        {
            var noteResults = SearchNotes(query, subjects);
            var cardResults = SearchFlashCards(query, subjects);

            var combined = new List<SearchResult>(noteResults);
            combined.AddRange(cardResults);

            return combined.OrderByDescending(r => r.Date).ToList();
        }
    }
}
