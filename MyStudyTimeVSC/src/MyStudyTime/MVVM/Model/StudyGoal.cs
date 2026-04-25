using System;
using System.Collections.Generic;

namespace MyStudyTime.MVVM.Model
{
    public class StudyGoal
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DayOfWeek Day { get; set; }
        public List<string> SubjectIds { get; set; } = new List<string>();
        public string Notes { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public StudyGoal()
        {
        }

        public StudyGoal(DayOfWeek day)
        {
            Day = day;
        }
    }
}
