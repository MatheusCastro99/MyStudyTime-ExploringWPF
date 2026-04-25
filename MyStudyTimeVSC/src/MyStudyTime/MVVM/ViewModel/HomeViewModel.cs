using MyStudyTime.Core;
using MyStudyTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStudyTime.MVVM.ViewModel
{
    public class HomeViewModel : ObservableObject
    {
        private IDataService _dataService;
        private int _totalNotesCount;
        private int _totalFlashCardsCount;
        private int _subjectsCount;

        public int TotalNotesCount
        {
            get { return _totalNotesCount; }
            set
            {
                _totalNotesCount = value;
                OnPropertyChanged();
            }
        }

        public int TotalFlashCardsCount
        {
            get { return _totalFlashCardsCount; }
            set
            {
                _totalFlashCardsCount = value;
                OnPropertyChanged();
            }
        }

        public int SubjectsCount
        {
            get { return _subjectsCount; }
            set
            {
                _subjectsCount = value;
                OnPropertyChanged();
            }
        }

        public HomeViewModel(IDataService dataService)
        {
            _dataService = dataService;
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            var subjects = _dataService.LoadSubjects();
            SubjectsCount = subjects.Count;

            // Count total notes and flash cards
            int notesCount = 0;
            int flashCardsCount = 0;

            foreach (var subject in subjects)
            {
                notesCount += subject.Notes?.Count ?? 0;
                flashCardsCount += subject.FlashCards?.Count ?? 0;
            }

            // Add global flash cards count
            var globalFlashCards = _dataService.LoadFlashCards();
            flashCardsCount += globalFlashCards.Count;

            TotalNotesCount = notesCount;
            TotalFlashCardsCount = flashCardsCount;
        }
    }
}
