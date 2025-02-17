using MyStudyTime.Core;
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
    internal class MainViewModel : ObservableObject 
    {
        public RelayCommand HomeViewComand { get; set; }
        public RelayCommand SubjectViewComand { get; set; }
        public RelayCommand CloseViewComand { get; set; }

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



        public MainViewModel()
        {
            HomeVm = new HomeViewModel();
            SubjectVm = new SubjectViewModel();
            CurrentView = HomeVm;

            HomeViewComand = new RelayCommand(o =>
            {
                CurrentView = HomeVm;
            });

            SubjectViewComand = new RelayCommand(o =>
            {
                CurrentView = SubjectVm;
            });

            CloseViewComand = new RelayCommand(o =>
            {
                onRequestClose();
            });
        }
    }
}
