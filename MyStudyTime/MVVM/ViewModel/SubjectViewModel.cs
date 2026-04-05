using MyStudyTime.Core;
using MyStudyTime.MVVM.Model;
using MyStudyTime.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;

namespace MyStudyTime.MVVM.ViewModel
{
    internal class SubjectViewModel : ObservableObject
    {
        private string _newSubject;
        private IDataService _dataService;

        public string NewSubject
        {
            get { return _newSubject; }
            set
            {
                _newSubject = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Subject> Subjects { get; set; }

        public ICommand AddSubjectCommand { get; set; }
        public ICommand RemoveSubjectCommand { get; set; }
        public ICommand OpenNoteWindowCommand { get; set; }

        public SubjectViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Debug.WriteLine("SubjectViewModel: Constructor called");
            Subjects = _dataService.LoadSubjects();
            Debug.WriteLine($"SubjectViewModel: Loaded {Subjects.Count} subjects");

            // If no subjects exist, initialize with defaults
            if (Subjects.Count == 0)
            {
                Debug.WriteLine("SubjectViewModel: Creating default subjects");
                var defaults = new List<Subject>
                {
                    new Subject("Math"),
                    new Subject("English"),
                    new Subject("Bio"),
                    new Subject("History"),
                    new Subject("Chemistry")
                };
                foreach (var subject in defaults)
                {
                    Subjects.Add(subject);
                }
                _dataService.SaveSubjects(Subjects);
                Debug.WriteLine("SubjectViewModel: Default subjects saved");
            }

            AddSubjectCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(NewSubject))
                {
                    Subjects.Add(new Subject(NewSubject));
                    _dataService.SaveSubjects(Subjects);
                    NewSubject = string.Empty;
                }
            });

            RemoveSubjectCommand = new RelayCommand(o =>
            {
                if (o is Subject subject && Subjects.Contains(subject))
                {
                    Subjects.Remove(subject);
                    _dataService.SaveSubjects(Subjects);
                }
            });

            OpenNoteWindowCommand = new RelayCommand(o =>
            {
                if (o is Subject subject)
                {
                    var noteWindow = new MyStudyTime.MVVM.View.NoteWindow
                    {
                        DataContext = new NoteWindowViewModel(subject, _dataService)
                    };
                    noteWindow.Show();
                }
            });
        }
    }
}
