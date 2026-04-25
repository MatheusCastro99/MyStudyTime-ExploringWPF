using Xunit;
using MyStudyTime.MVVM.Model;
using System;

namespace MyStudyTime.Tests
{
    public class ModelTests
    {
        [Fact]
        public void Subject_Constructor_InitializesWithName()
        {
            // Act
            var subject = new Subject("Mathematics");

            // Assert
            Assert.Equal("Mathematics", subject.Name);
            Assert.NotEqual(DateTime.MinValue, subject.CreatedDate);
        }

        [Fact]
        public void Subject_Collections_AreInitialized()
        {
            // Act
            var subject = new Subject("Science");

            // Assert
            Assert.NotNull(subject.Notes);
            Assert.NotNull(subject.FlashCards);
            Assert.Empty(subject.Notes);
            Assert.Empty(subject.FlashCards);
        }

        [Fact]
        public void Note_Constructor_WithText_InitializesCorrectly()
        {
            // Act
            var note = new Note("Important concept");

            // Assert
            Assert.Equal("Important concept", note.Text);
            Assert.Equal(string.Empty, note.Tags);
            Assert.NotEqual(DateTime.MinValue, note.CreatedDate);
        }

        [Fact]
        public void Note_Constructor_Default_InitializesCorrectly()
        {
            // Act
            var note = new Note();

            // Assert
            Assert.Null(note.Text);
            Assert.Equal(string.Empty, note.Tags);
            Assert.NotEqual(DateTime.MinValue, note.CreatedDate);
        }

        [Fact]
        public void FlashCard_Constructor_WithQuestionAndAnswer_InitializesCorrectly()
        {
            // Act
            var card = new FlashCard("What is 2+2?", "4");

            // Assert
            Assert.Equal("What is 2+2?", card.Question);
            Assert.Equal("4", card.Answer);
            Assert.Equal(Difficulty.Medium, card.Difficulty);
            Assert.Equal(0, card.ReviewCount);
            Assert.False(card.GotRight);
        }

        [Fact]
        public void FlashCard_DifficultyLevels_AllSupported()
        {
            // Arrange
            var easyCard = new FlashCard("Easy", "Answer") { Difficulty = Difficulty.Easy };
            var mediumCard = new FlashCard("Medium", "Answer") { Difficulty = Difficulty.Medium };
            var hardCard = new FlashCard("Hard", "Answer") { Difficulty = Difficulty.Hard };

            // Assert
            Assert.Equal(Difficulty.Easy, easyCard.Difficulty);
            Assert.Equal(Difficulty.Medium, mediumCard.Difficulty);
            Assert.Equal(Difficulty.Hard, hardCard.Difficulty);
        }

        [Fact]
        public void FlashCard_ReviewCount_IncrementsCorrectly()
        {
            // Arrange
            var card = new FlashCard("Q", "A");

            // Act
            card.ReviewCount = 5;

            // Assert
            Assert.Equal(5, card.ReviewCount);
        }

        [Fact]
        public void FlashCard_Tags_CanBeSet()
        {
            // Arrange
            var card = new FlashCard("Q", "A");

            // Act
            card.Tags = "important,exam";

            // Assert
            Assert.Equal("important,exam", card.Tags);
        }

        [Fact]
        public void StudyGoal_Constructor_InitializesWithDay()
        {
            // Act
            var goal = new StudyGoal(DayOfWeek.Monday);

            // Assert
            Assert.Equal(DayOfWeek.Monday, goal.Day);
            Assert.NotNull(goal.SubjectIds);
            Assert.Empty(goal.SubjectIds);
        }

        [Fact]
        public void StudyGoal_Id_IsUnique()
        {
            // Act
            var goal1 = new StudyGoal();
            var goal2 = new StudyGoal();

            // Assert
            Assert.NotEqual(goal1.Id, goal2.Id);
        }

        [Fact]
        public void Subject_NavigationProperties_SupportsEF()
        {
            // Arrange
            var subject = new Subject("History");
            var note = new Note("World War II") { SubjectId = 1 };
            var flashCard = new FlashCard("When was WWII?", "1939-1945") { SubjectId = 1 };

            // Act
            subject.Notes.Add(note);
            subject.FlashCards.Add(flashCard);

            // Assert
            Assert.Single(subject.Notes);
            Assert.Single(subject.FlashCards);
            Assert.Equal(subject, note.Subject ?? subject);
        }
    }
}
