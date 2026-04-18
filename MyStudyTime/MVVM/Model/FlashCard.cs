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
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Tags { get; set; }
        public int ReviewCount { get; set; }
        public bool GotRight { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public DateTime CreatedDate { get; set; }

        // Navigation property for EF6
        public virtual Subject Subject { get; set; }

        public FlashCard()
        {
            Difficulty = Difficulty.Medium;
            Tags = string.Empty;
            ReviewCount = 0;
            GotRight = false;
            CreatedDate = DateTime.Now;
        }

        public FlashCard(string question, string answer)
        {
            Question = question;
            Answer = answer;
            Difficulty = Difficulty.Medium;
            Tags = string.Empty;
            ReviewCount = 0;
            GotRight = false;
            CreatedDate = DateTime.Now;
        }
    }
}
