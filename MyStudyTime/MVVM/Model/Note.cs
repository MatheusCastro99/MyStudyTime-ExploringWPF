using System;
using System.Collections.Generic;

namespace MyStudyTime.MVVM.Model
{
    public class Note
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string Text { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property for EF6
        public virtual Subject Subject { get; set; }

        public Note()
        {
            CreatedDate = DateTime.Now;
            Tags = string.Empty;
        }

        public Note(string text)
        {
            Text = text;
            CreatedDate = DateTime.Now;
            Tags = string.Empty;
        }
    }
}
