using System.Windows.Controls;
using Znak.ViewModel;

namespace Znak.View
{
    public partial class EditLaminateView : UserControl
    {
        public EditLaminateView()
        {
             InitializeComponent();  
             DataContext = new EditLaminateViewModel();
        }
    }
}
