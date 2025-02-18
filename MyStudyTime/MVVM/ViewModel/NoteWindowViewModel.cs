using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStudyTime.Core;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace MyStudyTime.MVVM.ViewModel
{
    public class NoteWindowViewModel : ObservableObject
    {
        private string _newNote;
        private Subject _subject;
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

        public ObservableCollection<string> Notes => _subject.Notes;

        public ICommand AddNoteCommand { get; set; }

        public NoteWindowViewModel(Subject subject)
        {
            _subject = subject;
            AddNoteCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(NewNote))
                {
                    Notes.Add(NewNote);
                    NewNote = string.Empty;
                }
            });
        }
    }
}
