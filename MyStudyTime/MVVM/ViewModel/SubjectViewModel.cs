using MyStudyTime.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace MyStudyTime.MVVM.ViewModel
{
    public class Subject : ObservableObject
    {
        private string _name;
        private string _newNote;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string NewNote
        {
            get { return _newNote; }
            set
            {
                _newNote = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Notes { get; set; }

        public Subject(string name)
        {
            Name = name;
            Notes = new ObservableCollection<string>();
        }
    }

    internal class SubjectViewModel : ObservableObject
    {
        private string _newSubject;

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

        public SubjectViewModel()
        {
            Subjects = new ObservableCollection<Subject>
                {
                    new Subject("Math"),
                    new Subject("English"),
                    new Subject("Bio"),
                    new Subject("History"),
                    new Subject("Chemistry")
                };

            AddSubjectCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(NewSubject))
                {
                    Subjects.Add(new Subject(NewSubject));
                    NewSubject = string.Empty;
                }
            });

            RemoveSubjectCommand = new RelayCommand(o =>
            {
                if (o is Subject subject && Subjects.Contains(subject))
                {
                    Subjects.Remove(subject);
                }
            });

            OpenNoteWindowCommand = new RelayCommand(o =>
            {
                if (o is Subject subject)
                {
                    var noteWindow = new MyStudyTime.MVVM.View.NoteWindow
                    {
                        DataContext = new NoteWindowViewModel(subject)
                    };
                    noteWindow.Show();
                }
            });
        }
    }
}
