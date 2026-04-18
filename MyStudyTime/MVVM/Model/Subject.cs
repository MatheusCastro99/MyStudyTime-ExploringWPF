using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MyStudyTime.MVVM.Model
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation properties for EF6
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
        public virtual ICollection<FlashCard> FlashCards { get; set; } = new List<FlashCard>();

        public Subject()
        {
            CreatedDate = DateTime.Now;
        }

        public Subject(string name)
        {
            Name = name;
            CreatedDate = DateTime.Now;
        }
    }
}

