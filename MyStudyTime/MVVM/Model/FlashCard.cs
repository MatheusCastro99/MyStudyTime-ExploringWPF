using System;
using System.Collections.Generic;

namespace MyStudyTime.MVVM.Model
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public class FlashCard
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Question { get; set; }
        public string Answer { get; set; }
        public Difficulty Difficulty { get; set; } = Difficulty.Medium;
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastReviewDate { get; set; }
        public int ReviewCount { get; set; } = 0;
        public string ParentSubjectId { get; set; } // For subject-scoped organization

        public FlashCard()
        {
        }

        public FlashCard(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }
    }
}
