using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MyStudyTime.MVVM.Model
{
    public class Subject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

        [XmlIgnore]
        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();

        [XmlElement("Note")]
        public List<Note> NotesList
        {
            get { return new List<Note>(Notes); }
            set
            {
                Notes.Clear();
                foreach (var note in value ?? new List<Note>())
                {
                    Notes.Add(note);
                }
            }
        }

        [XmlIgnore]
        public ObservableCollection<FlashCard> FlashCards { get; set; } = new ObservableCollection<FlashCard>();

        [XmlElement("FlashCard")]
        public List<FlashCard> FlashCardsList
        {
            get { return new List<FlashCard>(FlashCards); }
            set
            {
                FlashCards.Clear();
                foreach (var card in value ?? new List<FlashCard>())
                {
                    FlashCards.Add(card);
                }
            }
        }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public Subject()
        {
        }

        public Subject(string name)
        {
            Name = name;
        }
    }
}

