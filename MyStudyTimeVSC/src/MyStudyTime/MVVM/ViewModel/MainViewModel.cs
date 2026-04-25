using MyStudyTime.Core;
using MyStudyTime.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace MyStudyTime.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand SubjectViewCommand { get; set; }
        public RelayCommand CloseViewCommand { get; set; }

        public HomeViewModel HomeVm { get; set; }
        public SubjectViewModel SubjectVm { get; set; }

        public event EventHandler RequestClose;

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public void onRequestClose()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public MainViewModel(IDataService dataService)
        {
            HomeVm = new HomeViewModel(dataService);
            SubjectVm = new SubjectViewModel(dataService);
            CurrentView = HomeVm;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVm;
            });

            SubjectViewCommand = new RelayCommand(o =>
            {
                CurrentView = SubjectVm;
            });

            CloseViewCommand = new RelayCommand(o =>
            {
                onRequestClose();
            });
        }
    }
}
