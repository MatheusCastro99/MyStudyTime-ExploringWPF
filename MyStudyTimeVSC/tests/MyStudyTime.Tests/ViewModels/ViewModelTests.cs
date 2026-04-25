using Xunit;
using MyStudyTime.MVVM.ViewModel;
using MyStudyTime.MVVM.Model;
using MyStudyTime.Services;
using System.Collections.ObjectModel;

namespace MyStudyTime.Tests
{
    public class HomeViewModelTests
    {
        [Fact]
        public void HomeViewModel_LoadsDashboardData()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subject = new Subject("Math") { Id = 1 };
            subject.Notes.Add(new Note("Test note") { Id = 1 });
            subject.FlashCards.Add(new FlashCard("2+2=?", "4") { Id = 1 });

            var subjects = new ObservableCollection<Subject> { subject };
            dataService.SaveSubjects(subjects);

            // Act
            var viewModel = new HomeViewModel(dataService);

            // Assert
            Assert.Equal(1, viewModel.SubjectsCount);
            Assert.Equal(1, viewModel.TotalNotesCount);
            Assert.True(viewModel.TotalFlashCardsCount >= 1);
        }

        [Fact]
        public void HomeViewModel_CountsMultipleSubjects()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subjects = new ObservableCollection<Subject>
            {
                new Subject("Math") { Id = 1 },
                new Subject("English") { Id = 2 },
                new Subject("Science") { Id = 3 }
            };
            dataService.SaveSubjects(subjects);

            // Act
            var viewModel = new HomeViewModel(dataService);

            // Assert
            Assert.Equal(3, viewModel.SubjectsCount);
        }

        [Fact]
        public void HomeViewModel_AggregatesNotesAcrossSubjects()
        {
            // Arrange
            var dataService = new XmlDataService();

            var subject1 = new Subject("Math") { Id = 1 };
            subject1.Notes.Add(new Note("Algebra") { Id = 1 });
            subject1.Notes.Add(new Note("Geometry") { Id = 2 });

            var subject2 = new Subject("English") { Id = 2 };
            subject2.Notes.Add(new Note("Shakespeare") { Id = 3 });

            var subjects = new ObservableCollection<Subject> { subject1, subject2 };
            dataService.SaveSubjects(subjects);

            // Act
            var viewModel = new HomeViewModel(dataService);

            // Assert
            Assert.Equal(3, viewModel.TotalNotesCount);
        }
    }

    public class SubjectViewModelTests
    {
        [Fact]
        public void SubjectViewModel_LoadsSubjectsFromDataService()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subjects = new ObservableCollection<Subject>
            {
                new Subject("Math") { Id = 1 },
                new Subject("English") { Id = 2 }
            };
            dataService.SaveSubjects(subjects);

            // Act
            var viewModel = new SubjectViewModel(dataService);

            // Assert
            Assert.NotEmpty(viewModel.Subjects);
        }

        [Fact]
        public void SubjectViewModel_AddSubjectCommand_AddsNewSubject()
        {
            // Arrange
            var dataService = new XmlDataService();
            var initialSubjects = new ObservableCollection<Subject>
            {
                new Subject("Math") { Id = 1 }
            };
            dataService.SaveSubjects(initialSubjects);

            var viewModel = new SubjectViewModel(dataService);
            int initialCount = viewModel.Subjects.Count;

            // Act
            viewModel.NewSubject = "Physics";
            viewModel.AddSubjectCommand.Execute(null);

            // Assert
            Assert.Equal(initialCount + 1, viewModel.Subjects.Count);
            Assert.NotEmpty(viewModel.Subjects);
        }

        [Fact]
        public void SubjectViewModel_RemoveSubjectCommand_RemovesSubject()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subject = new Subject("History") { Id = 1 };
            var subjects = new ObservableCollection<Subject> { subject };
            dataService.SaveSubjects(subjects);

            var viewModel = new SubjectViewModel(dataService);
            int initialCount = viewModel.Subjects.Count;

            // Act
            viewModel.RemoveSubjectCommand.Execute(subject);

            // Assert
            Assert.Equal(initialCount - 1, viewModel.Subjects.Count);
        }

        [Fact]
        public void SubjectViewModel_NewSubjectProperty_UpdatesOnSet()
        {
            // Arrange
            var dataService = new XmlDataService();
            var viewModel = new SubjectViewModel(dataService);

            // Act
            viewModel.NewSubject = "Chemistry";

            // Assert
            Assert.Equal("Chemistry", viewModel.NewSubject);
        }

        [Fact]
        public void SubjectViewModel_AddSubjectCommand_ClearsNewSubjectAfterAdd()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subjects = new ObservableCollection<Subject>();
            dataService.SaveSubjects(subjects);

            var viewModel = new SubjectViewModel(dataService);
            viewModel.NewSubject = "Biology";

            // Act
            viewModel.AddSubjectCommand.Execute(null);

            // Assert
            Assert.Null(viewModel.NewSubject);
        }
    }

    public class NoteWindowViewModelTests
    {
        [Fact]
        public void NoteWindowViewModel_LoadsNotesFromSubject()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subject = new Subject("History") { Id = 1 };
            subject.Notes.Add(new Note("French Revolution") { Id = 1 });
            subject.Notes.Add(new Note("Industrial Revolution") { Id = 2 });

            var subjects = new ObservableCollection<Subject> { subject };

            // Act
            var viewModel = new NoteWindowViewModel(subject, subjects, dataService);

            // Assert
            Assert.NotEmpty(viewModel.Notes);
            Assert.Equal("History", viewModel.SubjectName);
        }

        [Fact]
        public void NoteWindowViewModel_AddNoteCommand_AddsNoteToSubject()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subject = new Subject("Science") { Id = 1 };
            var subjects = new ObservableCollection<Subject> { subject };

            var viewModel = new NoteWindowViewModel(subject, subjects, dataService);
            int initialCount = viewModel.Notes.Count;

            // Act
            viewModel.NewNote = "Photosynthesis";
            viewModel.NewTags = "biology,process";
            viewModel.AddNoteCommand.Execute(null);

            // Assert
            Assert.Equal(initialCount + 1, viewModel.Notes.Count);
        }

        [Fact]
        public void NoteWindowViewModel_AddNoteCommand_ClearsInputsAfterAdd()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subject = new Subject("Math") { Id = 1 };
            var subjects = new ObservableCollection<Subject> { subject };

            var viewModel = new NoteWindowViewModel(subject, subjects, dataService);
            viewModel.NewNote = "Calculus";
            viewModel.NewTags = "math,advanced";

            // Act
            viewModel.AddNoteCommand.Execute(null);

            // Assert
            Assert.Empty(viewModel.NewNote ?? "");
            Assert.Empty(viewModel.NewTags ?? "");
        }

        [Fact]
        public void NoteWindowViewModel_RemoveNoteCommand_RemovesNote()
        {
            // Arrange
            var dataService = new XmlDataService();
            var subject = new Subject("English") { Id = 1 };
            var note = new Note("Shakespeare") { Id = 1 };
            subject.Notes.Add(note);

            var subjects = new ObservableCollection<Subject> { subject };
            var viewModel = new NoteWindowViewModel(subject, subjects, dataService);
            int initialCount = viewModel.Notes.Count;

            // Act
            viewModel.RemoveNoteCommand.Execute(note);

            // Assert
            Assert.Equal(initialCount - 1, viewModel.Notes.Count);
        }
    }
}
