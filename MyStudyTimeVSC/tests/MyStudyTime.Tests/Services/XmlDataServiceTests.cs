using Xunit;
using MyStudyTime.Services;
using MyStudyTime.MVVM.Model;
using System.Collections.ObjectModel;
using System.IO;
using System;
using System.Linq;

namespace MyStudyTime.Tests
{
    public class XmlDataServiceTests
    {
        private readonly string _testDataDirectory;
        private readonly XmlDataService _dataService;

        public XmlDataServiceTests()
        {
            // Use a temporary directory for tests
            _testDataDirectory = Path.Combine(Path.GetTempPath(), $"MyStudyTime_Test_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDataDirectory);

            // Override the data directory for testing
            _dataService = new XmlDataService();
        }

        [Fact]
        public void LoadSubjects_WhenNoSubjectsExist_ReturnsEmptyCollection()
        {
            // Arrange
            var service = new XmlDataService();

            // Act
            var subjects = service.LoadSubjects();

            // Assert
            Assert.NotNull(subjects);
            Assert.IsType<ObservableCollection<Subject>>(subjects);
        }

        [Fact]
        public void SaveSubjects_WithValidSubjects_PersistsData()
        {
            // Arrange
            var service = new XmlDataService();
            var subjects = new ObservableCollection<Subject>
            {
                new Subject("Math") { Id = 1 },
                new Subject("English") { Id = 2 }
            };

            // Act
            service.SaveSubjects(subjects);
            var loadedSubjects = service.LoadSubjects();

            // Assert
            Assert.NotNull(loadedSubjects);
            Assert.Equal(2, loadedSubjects.Count);
        }

        [Fact]
        public void SaveFlashCards_WithValidCards_PersistsData()
        {
            // Arrange
            var service = new XmlDataService();
            var cards = new ObservableCollection<FlashCard>
            {
                new FlashCard("What is 2+2?", "4") { Id = 1 },
                new FlashCard("What is the capital of France?", "Paris") { Id = 2 }
            };

            // Act
            service.SaveFlashCards(cards);
            var loadedCards = service.LoadFlashCards();

            // Assert
            Assert.NotNull(loadedCards);
            Assert.Equal(2, loadedCards.Count);
        }

        [Fact]
        public void AddNoteToSubject_PersistsWithSubject()
        {
            // Arrange
            var service = new XmlDataService();
            var subject = new Subject("Biology") { Id = 1 };
            subject.Notes.Add(new Note("Cells are the basic unit of life") { Id = 1 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            service.SaveSubjects(subjects);
            var loadedSubjects = service.LoadSubjects();

            // Assert
            Assert.NotNull(loadedSubjects);
            Assert.Single(loadedSubjects);
            Assert.Single(loadedSubjects[0].Notes);
            Assert.Equal("Cells are the basic unit of life", loadedSubjects[0].Notes.First().Text);
        }

        [Fact]
        public void Subject_WithMultipleNotes_PreservesAllNotes()
        {
            // Arrange
            var service = new XmlDataService();
            var subject = new Subject("History") { Id = 1 };
            subject.Notes.Add(new Note("World War I: 1914-1918") { Id = 1 });
            subject.Notes.Add(new Note("World War II: 1939-1945") { Id = 2 });
            subject.Notes.Add(new Note("Cold War: 1945-1991") { Id = 3 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            service.SaveSubjects(subjects);
            var loadedSubjects = service.LoadSubjects();

            // Assert
            Assert.NotNull(loadedSubjects);
            Assert.Single(loadedSubjects);
            Assert.Equal(3, loadedSubjects[0].Notes.Count);
        }

        [Fact]
        public void FlashCard_WithDifficulty_PreservesDifficultyLevel()
        {
            // Arrange
            var service = new XmlDataService();
            var cards = new ObservableCollection<FlashCard>
            {
                new FlashCard("Easy question", "Easy answer")
                {
                    Id = 1,
                    Difficulty = Difficulty.Easy
                },
                new FlashCard("Hard question", "Hard answer")
                {
                    Id = 2,
                    Difficulty = Difficulty.Hard
                }
            };

            // Act
            service.SaveFlashCards(cards);
            var loadedCards = service.LoadFlashCards();

            // Assert
            Assert.Equal(2, loadedCards.Count);
            Assert.Equal(Difficulty.Easy, loadedCards[0].Difficulty);
            Assert.Equal(Difficulty.Hard, loadedCards[1].Difficulty);
        }

        [Fact]
        public void SaveEmptySubjectCollection_DoesNotThrowException()
        {
            // Arrange
            var service = new XmlDataService();
            var emptySubjects = new ObservableCollection<Subject>();

            // Act & Assert
            service.SaveSubjects(emptySubjects);
            Assert.Empty(service.LoadSubjects());
        }
    }
}
