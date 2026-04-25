using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MyStudyTime.Core;
using MyStudyTime.MVVM.Model;
using MyStudyTime.Services;

namespace MyStudyTime.MVVM.ViewModel
{
    public class FlashCardReviewViewModel : ObservableObject
    {
        private List<FlashCard> _cards;
        private int _currentIndex;
        private FlashCard _currentCard;
        private bool _isAnswerRevealed;
        private int _correctCount;
        private int _incorrectCount;
        private IDataService _dataService;
        private Subject _subject;
        private ObservableCollection<Subject> _allSubjects;

        public FlashCard CurrentCard
        {
            get { return _currentCard; }
            set
            {
                _currentCard = value;
                OnPropertyChanged();
            }
        }

        public bool IsAnswerRevealed
        {
            get { return _isAnswerRevealed; }
            set
            {
                _isAnswerRevealed = value;
                OnPropertyChanged();
            }
        }

        public int CurrentIndex => _currentIndex + 1;
        public int TotalCards => _cards.Count;
        public int CorrectCount
        {
            get { return _correctCount; }
            set
            {
                _correctCount = value;
                OnPropertyChanged();
            }
        }

        public int IncorrectCount
        {
            get { return _incorrectCount; }
            set
            {
                _incorrectCount = value;
                OnPropertyChanged();
            }
        }

        public double ProgressPercentage => TotalCards == 0 ? 0 : (CurrentIndex / (double)TotalCards) * 100;

        public ICommand RevealAnswerCommand { get; set; }
        public ICommand MarkCorrectCommand { get; set; }
        public ICommand MarkIncorrectCommand { get; set; }
        public ICommand EndReviewCommand { get; set; }

        public event EventHandler ReviewCompleted;

        public FlashCardReviewViewModel(Subject subject, ObservableCollection<Subject> allSubjects, IDataService dataService)
        {
            _subject = subject;
            _allSubjects = allSubjects;
            _dataService = dataService;
            _cards = subject.FlashCards.ToList();
            _currentIndex = 0;
            _correctCount = 0;
            _incorrectCount = 0;
            _isAnswerRevealed = false;

            if (_cards.Count > 0)
            {
                CurrentCard = _cards[0];
            }

            RevealAnswerCommand = new RelayCommand(o =>
            {
                IsAnswerRevealed = true;
            });

            MarkCorrectCommand = new RelayCommand(o =>
            {
                if (CurrentCard != null)
                {
                    CurrentCard.ReviewCount++;
                    CurrentCard.LastReviewDate = DateTime.Now;
                    CurrentCard.Difficulty = Difficulty.Easy;
                    CurrentCard.GotRight = true;
                    CorrectCount++;

                    MoveToNext();
                }
            });

            MarkIncorrectCommand = new RelayCommand(o =>
            {
                if (CurrentCard != null)
                {
                    CurrentCard.ReviewCount++;
                    CurrentCard.LastReviewDate = DateTime.Now;
                    CurrentCard.Difficulty = Difficulty.Hard;
                    CurrentCard.GotRight = false;
                    IncorrectCount++;

                    MoveToNext();
                }
            });

            EndReviewCommand = new RelayCommand(o =>
            {
                SaveAndComplete();
            });
        }

        private void MoveToNext()
        {
            _currentIndex++;
            IsAnswerRevealed = false;
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(ProgressPercentage));

            if (_currentIndex < _cards.Count)
            {
                CurrentCard = _cards[_currentIndex];
            }
            else
            {
                SaveAndComplete();
            }
        }

        private void SaveAndComplete()
        {
            // Save review data - just save the subjects collection which includes all changes
            _dataService.SaveSubjects(_allSubjects);

            ReviewCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}
