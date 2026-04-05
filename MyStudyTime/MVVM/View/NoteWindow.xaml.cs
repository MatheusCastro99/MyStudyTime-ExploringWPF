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
    /// Interaction logic for NoteWindow.xaml
    /// </summary>
    public partial class NoteWindow : Window
    {
        public NoteWindow()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Height = Double.NaN; // Reset height to auto
                if (!double.IsNaN(textBox.Width))
                {
                    textBox.Measure(new Size(textBox.Width, Double.PositiveInfinity));
                    textBox.Height = textBox.DesiredSize.Height;
                }
            }
        }
    }
}
