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
using System.Windows.Shapes;

namespace MyStudyTime.MVVM.View
{
    /// <summary>
    /// Interaction logic for FlashCardReviewWindow.xaml
    /// </summary>
    public partial class FlashCardReviewWindow : Window
    {
        public FlashCardReviewWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModel.FlashCardReviewViewModel viewModel)
            {
                viewModel.ReviewCompleted += (s, ev) => this.Close();
            }
        }
    }
}
