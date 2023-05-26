using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Logic;
using Logic.Model;

namespace Znak
{
    /// <summary>
    /// Логика взаимодействия для WindowPloter.xaml
    /// </summary>
    public partial class WindowPloter : Window, INotifyPropertyChanged
    {
        public WindowPloter()
        {
            InitializeComponent();

            MainWindow window = new ();
            //загрузка цен
            PricePloter = PriceManager.DefaultPricesPloter;
            DataContext = this;
        }

        /// <summary>
        /// поле с материалами и ценами биндится к ComboBox
        /// </summary>
        public List<PricePloter> PricePloter { get; set;}

        /// <summary>
        /// тип бумаги выбранный пользователем в ComboBox
        /// </summary>
        public List<PricePloter> PloterPeperType { get; set; }
       
    }
}
