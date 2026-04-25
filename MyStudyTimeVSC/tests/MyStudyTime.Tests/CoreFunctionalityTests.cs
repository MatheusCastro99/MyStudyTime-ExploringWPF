using Xunit;
using MyStudyTime.Services;
using MyStudyTime.MVVM.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyStudyTime.Tests
{
    /// <summary>
    /// Core functionality tests to verify migration didn't break key features
    /// </summary>
    public class CoreFunctionalityTests
    {
        [Fact]
        public void Subject_CreationAndProperties_Work()
        {
            // Arrange & Act
            var subject = new Subject("Mathematics");

            // Assert
            Assert.NotNull(subject);
            Assert.Equal("Mathematics", subject.Name);
            Assert.NotEqual(0, subject.CreatedDate.Ticks);
        }

        [Fact]
        public void Note_CreationAndProperties_Work()
        {
            // Arrange & Act
            var note = new Note("Important equation: E=mc²")
            {
                Tags = "physics,einstein"
            };

            // Assert
            Assert.NotNull(note);
            Assert.Equal("Important equation: E=mc²", note.Text);
            Assert.Equal("physics,einstein", note.Tags);
        }

        [Fact]
        public void FlashCard_CreationWithAllProperties_Works()
        {
            // Arrange & Act
            var card = new FlashCard("What is 2+2?", "4")
            {
                Difficulty = Difficulty.Easy,
                Tags = "math,basic",
                ReviewCount = 5,
                GotRight = true
            };

            // Assert
            Assert.NotNull(card);
            Assert.Equal("What is 2+2?", card.Question);
            Assert.Equal("4", card.Answer);
            Assert.Equal(Difficulty.Easy, card.Difficulty);
            Assert.Equal("math,basic", card.Tags);
            Assert.Equal(5, card.ReviewCount);
            Assert.True(card.GotRight);
        }

        [Fact]
        public void Subject_CanContainNotesAndFlashCards()
        {
            // Arrange
            var subject = new Subject("Biology");
            var note = new Note("Photosynthesis process");
            var flashCard = new FlashCard("What is photosynthesis?", "Conversion of light to chemical energy");

            // Act
            subject.Notes.Add(note);
            subject.FlashCards.Add(flashCard);

            // Assert
            Assert.Single(subject.Notes);
            Assert.Single(subject.FlashCards);
            Assert.Contains(note, subject.Notes);
            Assert.Contains(flashCard, subject.FlashCards);
        }

        [Fact]
        public void SearchService_FindsNotesByContent()
        {
            // Arrange
            var searchService = new SearchService();
            var subject = new Subject("History");
            subject.Notes.Add(new Note("The French Revolution occurred in 1789"));
            subject.Notes.Add(new Note("The American Revolution started in 1776"));

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = searchService.SearchNotes("French", subjects);

            // Assert
            Assert.NotEmpty(results);
            Assert.Single(results);
            Assert.Contains("French Revolution", results[0].Content);
        }

        [Fact]
        public void SearchService_FindsFlashCardsByQuestion()
        {
            // Arrange
            var searchService = new SearchService();
            var subject = new Subject("Science");
            subject.FlashCards.Add(new FlashCard("What is DNA?", "Deoxyribonucleic acid"));
            subject.FlashCards.Add(new FlashCard("What is RNA?", "Ribonucleic acid"));

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = searchService.SearchFlashCards("DNA", subjects);

            // Assert
            Assert.NotEmpty(results);
            Assert.Single(results);
            Assert.Contains("What is DNA?", results[0].Title);
        }

        [Fact]
        public void SearchService_FindsFlashCardsByAnswer()
        {
            // Arrange
            var searchService = new SearchService();
            var subject = new Subject("Geography");
            subject.FlashCards.Add(new FlashCard("Capital of France?", "Paris"));
            subject.FlashCards.Add(new FlashCard("Capital of Germany?", "Berlin"));

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var results = searchService.SearchFlashCards("Paris", subjects);

            // Assert
            Assert.NotEmpty(results);
            Assert.Single(results);
            Assert.Contains("Paris", results[0].Content);
        }

        [Fact]
        public void FlashCard_DifficultyLevels_AreSupported()
        {
            // Arrange & Act
            var easyCard = new FlashCard("?", "A") { Difficulty = Difficulty.Easy };
            var mediumCard = new FlashCard("?", "A") { Difficulty = Difficulty.Medium };
            var hardCard = new FlashCard("?", "A") { Difficulty = Difficulty.Hard };

            // Assert
            Assert.Equal(Difficulty.Easy, easyCard.Difficulty);
            Assert.Equal(Difficulty.Medium, mediumCard.Difficulty);
            Assert.Equal(Difficulty.Hard, hardCard.Difficulty);
        }

        [Fact]
        public void RelayCommand_CanExecuteAndExecute()
        {
            // Arrange
            bool executed = false;
            var command = new MyStudyTime.Core.RelayCommand(
                execute: (obj) => { executed = true; },
                canExecute: (obj) => true
            );

            // Act
            bool canExecute = command.CanExecute(null);
            command.Execute(null);

            // Assert
            Assert.True(canExecute);
            Assert.True(executed);
        }

        [Fact]
        public void Converters_BoolToVisibility_Works()
        {
            // Arrange
            var converter = new MyStudyTime.Core.BoolToVisibilityConverter();

            // Act
            var visibleResult = converter.Convert(true, null, null, null);
            var collapsedResult = converter.Convert(false, null, null, null);

            // Assert
            Assert.NotNull(visibleResult);
            Assert.NotNull(collapsedResult);
        }

        [Fact]
        public void MultipleNotesTags_ParseCorrectly()
        {
            // Arrange & Act
            var note = new Note("Quantum mechanics basics")
            {
                Tags = "physics,quantum,advanced,exam-prep"
            };

            // Assert
            Assert.Equal("physics,quantum,advanced,exam-prep", note.Tags);
            Assert.Contains("physics", note.Tags);
            Assert.Contains("quantum", note.Tags);
        }

        [Fact]
        public void FlashCardCollections_OperationsWork()
        {
            // Arrange
            var cards = new ObservableCollection<FlashCard>
            {
                new FlashCard("Q1", "A1"),
                new FlashCard("Q2", "A2"),
                new FlashCard("Q3", "A3")
            };

            // Act
            var count1 = cards.Count;
            var card = cards.FirstOrDefault(c => c.Question == "Q2");
            cards.Remove(card);
            var count2 = cards.Count;

            // Assert
            Assert.Equal(3, count1);
            Assert.Equal(2, count2);
            Assert.NotNull(card);
        }
    }
}
