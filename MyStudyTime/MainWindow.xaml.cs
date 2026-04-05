using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyStudyTime.MVVM.ViewModel;
using MyStudyTime.Services;

namespace MyStudyTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            // Initialize services here when window is fully loaded
            IDataService dataService = new XmlDataService();
            MainViewModel viewModel = new MainViewModel(dataService);
            this.DataContext = viewModel;

            viewModel.RequestClose += (s, ev) => this.Close();
        }
    }
}
