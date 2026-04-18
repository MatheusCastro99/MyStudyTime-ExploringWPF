using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MyStudyTime.Core;
using MyStudyTime.MVVM.Model;
using MyStudyTime.Services;

namespace MyStudyTime.MVVM.ViewModel
{
    public class FlashCardViewModel : ObservableObject
    {
        private string _newQuestion;
        private string _newAnswer;
        private string _newDifficulty;
        private string _newTags;
        private Subject _subject;
        private IDataService _dataService;
        private ObservableCollection<Subject> _allSubjects;
        private bool _isReviewMode;
        private int _currentCardIndex;
        private List<FlashCard> _reviewCards;
        private FlashCard _currentCard;
        private ObservableCollection<FlashCard> _flashCardsObservable;

        public string SubjectName => _subject.Name;

        public string NewQuestion
        {
            get { return _newQuestion; }
            set
            {
                _newQuestion = value;
                OnPropertyChanged();
            }
        }

        public string NewAnswer
        {
            get { return _newAnswer; }
            set
            {
                _newAnswer = value;
                OnPropertyChanged();
            }
        }

        public string NewDifficulty
        {
            get { return _newDifficulty; }
            set
            {
                _newDifficulty = value;
                OnPropertyChanged();
            }
        }

        public string NewTags
        {
            get { return _newTags; }
            set
            {
                _newTags = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FlashCard> FlashCards
        {
            get
            {
                if (_flashCardsObservable == null)
                {
                    _flashCardsObservable = new ObservableCollection<FlashCard>(_subject.FlashCards);
                }
                return _flashCardsObservable;
            }
        }

        public bool IsReviewMode
        {
            get { return _isReviewMode; }
            set
            {
                _isReviewMode = value;
                OnPropertyChanged();
            }
        }

        public int CurrentCardIndex
        {
            get { return _currentCardIndex; }
            set
            {
                _currentCardIndex = value;
                OnPropertyChanged();
            }
        }

        public FlashCard CurrentCard
        {
            get { return _currentCard; }
            set
            {
                _currentCard = value;
                OnPropertyChanged();
            }
        }

        public int TotalCards => FlashCards.Count;
        public int ReviewedCards => _currentCardIndex;

        public ICommand AddFlashCardCommand { get; set; }
        public ICommand RemoveFlashCardCommand { get; set; }
        public ICommand StartReviewCommand { get; set; }
        public ICommand MarkCorrectCommand { get; set; }
        public ICommand MarkIncorrectCommand { get; set; }
        public ICommand EndReviewCommand { get; set; }

        public FlashCardViewModel(Subject subject, ObservableCollection<Subject> allSubjects, IDataService dataService)
        {
            _subject = subject;
            _allSubjects = allSubjects;
            _dataService = dataService;
            _isReviewMode = false;
            _currentCardIndex = 0;
            _reviewCards = new List<FlashCard>();
            _flashCardsObservable = new ObservableCollection<FlashCard>(_subject.FlashCards);

            AddFlashCardCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(NewQuestion) && !string.IsNullOrWhiteSpace(NewAnswer))
                {
                    var difficulty = Difficulty.Medium;
                    if (Enum.TryParse<Difficulty>(NewDifficulty ?? "Medium", out var parsedDifficulty))
                    {
                        difficulty = parsedDifficulty;
                    }

                    var card = new FlashCard
                    {
                        Question = NewQuestion,
                        Answer = NewAnswer,
                        Difficulty = difficulty,
                        SubjectId = _subject.Id,
                        Tags = NewTags ?? string.Empty
                    };

                    _subject.FlashCards.Add(card);
                    _flashCardsObservable.Add(card);

                    // Save to data service
                    _dataService.SaveSubjects(_allSubjects);

                    NewQuestion = string.Empty;
                    NewAnswer = string.Empty;
                    NewDifficulty = "Medium";
                    NewTags = string.Empty;
                }
            });

            RemoveFlashCardCommand = new RelayCommand(o =>
            {
                if (o is FlashCard card && _flashCardsObservable.Contains(card))
                {
                    _subject.FlashCards.Remove(card);
                    _flashCardsObservable.Remove(card);

                    // Save to data service
                    _dataService.SaveSubjects(_allSubjects);
                }
            });

            StartReviewCommand = new RelayCommand(o =>
            {
                if (FlashCards.Count > 0)
                {
                    // Create the review window
                    var reviewWindow = new MVVM.View.FlashCardReviewWindow
                    {
                        Owner = Application.Current.MainWindow,
                        DataContext = new FlashCardReviewViewModel(_subject, _allSubjects, _dataService)
                    };
                    reviewWindow.ShowDialog();
                }
            });

            MarkCorrectCommand = new RelayCommand(o =>
            {
                if (CurrentCard != null)
                {
                    CurrentCard.ReviewCount++;
                    CurrentCard.LastReviewDate = DateTime.Now;
                    CurrentCard.Difficulty = Difficulty.Easy;
                    CurrentCard.GotRight = true;

                    MoveToNextCard();
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

                    MoveToNextCard();
                }
            });

            EndReviewCommand = new RelayCommand(o =>
            {
                IsReviewMode = false;
                CurrentCardIndex = 0;
                CurrentCard = null;
                _currentCardIndex = 0;

                // Save review data
                _dataService.SaveSubjects(_allSubjects);
            });

            NewDifficulty = "Medium";
        }

        private void MoveToNextCard()
        {
            _currentCardIndex++;
            OnPropertyChanged(nameof(ReviewedCards));

            if (_currentCardIndex < _reviewCards.Count)
            {
                CurrentCard = _reviewCards[_currentCardIndex];
            }
            else
            {
                // Review complete
                IsReviewMode = false;
                CurrentCard = null;
                _currentCardIndex = 0;

                // Save review data
                var allSubjects = _dataService.LoadSubjects();
                _dataService.SaveSubjects(allSubjects);
            }
        }
    }
}
