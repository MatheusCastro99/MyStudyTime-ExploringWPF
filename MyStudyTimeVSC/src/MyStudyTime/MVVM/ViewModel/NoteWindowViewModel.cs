using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MyStudyTime.Core;
using MyStudyTime.MVVM.Model;
using MyStudyTime.Services;

namespace MyStudyTime.MVVM.ViewModel
{
    public class NoteWindowViewModel : ObservableObject
    {
        private string _newNote;
        private string _newTags;
        private Subject _subject;
        private IDataService _dataService;
        private ObservableCollection<Subject> _allSubjects;
        private ObservableCollection<Note> _notesObservable;

        public string SubjectName => _subject.Name;

        public string NewNote
        {
            get { return _newNote; }
            set
            {
                _newNote = value;
                OnPropertyChanged();
            }
        }

        public string NewTags
        {
            get { return _newTags; }
            set
            {
                _newTags = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Note> Notes
        {
            get
            {
                if (_notesObservable == null)
                {
                    _notesObservable = new ObservableCollection<Note>(_subject.Notes);
                }
                return _notesObservable;
            }
        }

        public ICommand AddNoteCommand { get; set; }
        public ICommand RemoveNoteCommand { get; set; }

        public NoteWindowViewModel(Subject subject, ObservableCollection<Subject> allSubjects, IDataService dataService)
        {
            _subject = subject;
            _allSubjects = allSubjects;
            _dataService = dataService;
            _notesObservable = new ObservableCollection<Note>(_subject.Notes);

            AddNoteCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(NewNote))
                {
                    var note = new Note(NewNote)
                    {
                        SubjectId = _subject.Id,
                        Tags = NewTags ?? string.Empty
                    };

                    _subject.Notes.Add(note);
                    _notesObservable.Add(note);

                    // Save to data service
                    _dataService.SaveSubjects(_allSubjects);

                    NewNote = string.Empty;
                    NewTags = string.Empty;
                }
            });

            RemoveNoteCommand = new RelayCommand(o =>
            {
                if (o is Note note && _notesObservable.Contains(note))
                {
                    _subject.Notes.Remove(note);
                    _notesObservable.Remove(note);

                    // Save to data service
                    _dataService.SaveSubjects(_allSubjects);
                }
            });
        }
    }
}
