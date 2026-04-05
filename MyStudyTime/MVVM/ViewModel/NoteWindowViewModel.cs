using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStudyTime.Core;
using MyStudyTime.MVVM.Model;
using MyStudyTime.Services;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MyStudyTime.MVVM.ViewModel
{
    public class NoteWindowViewModel : ObservableObject
    {
        private string _newNote;
        private string _newTags;
        private Subject _subject;
        private IDataService _dataService;
        private ObservableCollection<Subject> _allSubjects;

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

        public ObservableCollection<Note> Notes => _subject.Notes;

        public ICommand AddNoteCommand { get; set; }
        public ICommand RemoveNoteCommand { get; set; }

        public NoteWindowViewModel(Subject subject, IDataService dataService)
        {
            _subject = subject;
            _dataService = dataService;

            AddNoteCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(NewNote))
                {
                    var note = new Note(NewNote);

                    // Parse tags from comma-separated input
                    if (!string.IsNullOrWhiteSpace(NewTags))
                    {
                        var tags = NewTags.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                        foreach (var tag in tags)
                        {
                            note.Tags.Add(tag.Trim());
                        }
                    }

                    Notes.Add(note);

                    // Save to data service
                    var allSubjects = _dataService.LoadSubjects();
                    _dataService.SaveSubjects(allSubjects);

                    NewNote = string.Empty;
                    NewTags = string.Empty;
                }
            });

            RemoveNoteCommand = new RelayCommand(o =>
            {
                if (o is Note note && Notes.Contains(note))
                {
                    Notes.Remove(note);

                    // Save to data service
                    var allSubjects = _dataService.LoadSubjects();
                    _dataService.SaveSubjects(allSubjects);
                }
            });
        }
    }
}
