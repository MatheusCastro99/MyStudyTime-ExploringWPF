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

            // Try to use EF6DataService, but fall back to XmlDataService if database isn't available
            IDataService dataService;
            try
            {
                dataService = new EF6DataService();
                // Test the connection
                var testLoad = dataService.LoadSubjects();
                System.Diagnostics.Debug.WriteLine("✓ Using EF6DataService for data persistence");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠ EF6DataService unavailable ({ex.Message}), falling back to XmlDataService");
                System.Diagnostics.Debug.WriteLine("  Note: LocalDB may not be installed. Install SQL Server LocalDB to enable database persistence.");
                dataService = new XmlDataService();
            }

            MainViewModel viewModel = new MainViewModel(dataService);
            this.DataContext = viewModel;

            viewModel.RequestClose += (s, ev) => this.Close();
        }
    }
}
