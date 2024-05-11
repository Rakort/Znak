using System.Windows.Controls;
using Znak.ViewModel;

namespace Znak.View
{
    public partial class EditPostPrintingView : UserControl
    {
        public EditPostPrintingView()
        {
             InitializeComponent();   
             DataContext = new EditPostPrintingViewModel();
        }
    }
}
