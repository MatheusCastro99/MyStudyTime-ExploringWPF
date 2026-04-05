using System;
using System.Collections.Generic;

namespace MyStudyTime.MVVM.Model
{
    public class Note
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
        public List<string> Tags { get; set; } = new List<string>();

        public Note()
        {
        }

        public Note(string content)
        {
            Content = content;
        }
    }
}
