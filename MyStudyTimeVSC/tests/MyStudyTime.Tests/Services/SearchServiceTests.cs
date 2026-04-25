using Xunit;
using MyStudyTime.Services;
using MyStudyTime.MVVM.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyStudyTime.Tests
{
    public class SearchServiceTests
    {
        private readonly SearchService _searchService = new();

        [Fact]
        public void SearchNotes_WithValidQuery_ReturnsMatchingNotes()
        {
            // Arrange
            var subject = new Subject("Biology") { Id = 1 };
            subject.Notes.Add(new Note("Photosynthesis is the process") { Id = 1, Tags = "process,energy" });
            subject.Notes.Add(new Note("Respiration occurs in cells") { Id = 2, Tags = "respiration,cells" });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchNotes("Photosynthesis", subjects);

            // Assert
            Assert.NotEmpty(results);
            Assert.Single(results);
            Assert.Equal("Photosynthesis is the process", results[0].Content);
        }

        [Fact]
        public void SearchNotes_WithTagQuery_ReturnsMatchingNotes()
        {
            // Arrange
            var subject = new Subject("Chemistry") { Id = 1 };
            subject.Notes.Add(new Note("Electrons") { Id = 1, Tags = "atom,subatomic" });
            subject.Notes.Add(new Note("Protons") { Id = 2, Tags = "atom,subatomic" });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchNotes("atom", subjects);

            // Assert
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void SearchFlashCards_WithValidQuery_ReturnsMatchingCards()
        {
            // Arrange
            var subject = new Subject("Math") { Id = 1 };
            subject.FlashCards.Add(new FlashCard("What is 2+2?", "4") { Id = 1 });
            subject.FlashCards.Add(new FlashCard("What is the capital of France?", "Paris") { Id = 2 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchFlashCards("2+2", subjects);

            // Assert
            Assert.Single(results);
            Assert.Equal("What is 2+2?", results[0].Title);
        }

        [Fact]
        public void SearchFlashCards_SearchingInAnswer_ReturnsMatchingCards()
        {
            // Arrange
            var subject = new Subject("Geography") { Id = 1 };
            subject.FlashCards.Add(new FlashCard("Capital of France?", "Paris") { Id = 1 });
            subject.FlashCards.Add(new FlashCard("Capital of Germany?", "Berlin") { Id = 2 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchFlashCards("Berlin", subjects);

            // Assert
            Assert.Single(results);
            Assert.Equal("Capital of Germany?", results[0].Title);
        }

        [Fact]
        public void SearchAll_ReturnsNotesAndFlashCards()
        {
            // Arrange
            var subject = new Subject("History") { Id = 1 };
            subject.Notes.Add(new Note("French Revolution: 1789") { Id = 1 });
            subject.FlashCards.Add(new FlashCard("When was the French Revolution?", "1789") { Id = 1 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchAll("French Revolution", subjects);

            // Assert
            Assert.Equal(2, results.Count);
            var noteResult = results.FirstOrDefault(r => r.Type == "Note");
            var cardResult = results.FirstOrDefault(r => r.Type == "FlashCard");

            Assert.NotNull(noteResult);
            Assert.NotNull(cardResult);
        }

        [Fact]
        public void SearchNotes_CaseInsensitive_FindsMatches()
        {
            // Arrange
            var subject = new Subject("Science") { Id = 1 };
            subject.Notes.Add(new Note("DNA is a double helix") { Id = 1 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchNotes("dna", subjects);

            // Assert
            Assert.Single(results);
        }

        [Fact]
        public void SearchFlashCards_WithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            var subject = new Subject("Math") { Id = 1 };
            subject.FlashCards.Add(new FlashCard("2+2=?", "4") { Id = 1 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchFlashCards("XYZ", subjects);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public void SearchAll_ResultsAreSortedByDate()
        {
            // Arrange
            var subject = new Subject("Test") { Id = 1 };

            // Create notes with different dates
            var oldNote = new Note("Old note") { Id = 1, CreatedDate = System.DateTime.Now.AddDays(-10) };
            var newNote = new Note("New note") { Id = 2, CreatedDate = System.DateTime.Now };

            subject.Notes.Add(oldNote);
            subject.Notes.Add(newNote);

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = _searchService.SearchAll("note", subjects);

            // Assert
            Assert.Equal(2, results.Count);
            // Most recent should be first
            Assert.Equal("New note", results[0].Content);
        }
    }
}
