using System.ComponentModel;
using System.Windows.Controls;
using Znak.ViewModel;

namespace Znak.View
{
    /// <summary>
    /// Логика взаимодействия для EditLaserPrintView.xaml
    /// </summary>
    public partial class EditLaserPrintView : UserControl
    {
        public EditLaserPrintView()
        {
            InitializeComponent(); 
            DataContext = new EditLaserPrintViewModel();
        }
    }
}
