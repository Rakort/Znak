using Logic;
using Logic.Model;
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

namespace Znak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // загружает прайс лист
            PriceList = PriceManager.GetPrices();
            // уточнить у Саши!!!
            DataContext = this; 
        }
        public Price PaperType { get; set; }// тип бумаги выбранный пользователем в ComboBox
        
        public bool a4 = false; // A4 или А3 лист "false == a3"

        /// <summary>
        /// прайс лист
        /// </summary>
        public List<Price> PriceList { get; set; }

        Calculations calculations = new Calculations();

 

        #region сторонность и цветность печати, методы radio button

        /// <summary>
        /// изменение сторонности печати "fals"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_4_0_Checked(object sender, RoutedEventArgs e)
        {
            calculations.color = true;
            calculations.sidePrint = false;          
        }
        /// <summary>
        /// изменение сторонности печати "true"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_4_4_Checked(object sender, RoutedEventArgs e)
        {
            calculations.color = true;
            calculations.sidePrint = true;
        }
        /// <summary>
        /// изменение цветности печати "fals"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_1_0_Checked(object sender, RoutedEventArgs e)
        {
            calculations.color = false;
            calculations.sidePrint = false;
        }
        /// <summary>
        /// изменение цветности печати "true"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_1_1_Checked(object sender, RoutedEventArgs e)
        {
            calculations.color = false;
            calculations.sidePrint = true;
        }
        #endregion

        /// <summary>
        /// метод расчета стоимости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Price_Click(object sender, RoutedEventArgs e)
        {
            decimal priceInList = calculations.MainPrice(PaperType, int.Parse(TB_Sheets.Text), a4);
            TB_price_per_circulation.Text = (priceInList * decimal.Parse(TB_Sheets.Text)).ToString();
            TB_price_per_sheet.Text = priceInList.ToString();
        }
    }
}
