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
        /// <summary>
        /// тип бумаги выбранный пользователем в ComboBox
        /// </summary>
        public Price PaperType { get; set; }

        /// <summary>
        /// количество листов в тираже
        /// </summary>
        public int SheetsCount { get; set; }

        /// <summary>
        /// A4 или А3 лист "false == a3"
        /// </summary>
        public bool a4 = false; 

        /// <summary>
        /// прайс лист
        /// </summary>
        public List<Price> PriceList { get; set; }

        Calculations calculations = new Calculations();

        /// <summary>
        /// метод расчета стоимости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Price_Click(object sender, RoutedEventArgs e)
        {
            //устанавливаем значение цаетности и сторонности в зависимости от активных radioButton
            calculations.color = (RB_4_0.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);
            calculations.sidePrint = (RB_1_1.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);

            //устанавливаем значение дилерской цены в зависимости от активности checkBox
            calculations.diler = CB_Dealers.IsChecked ?? false;

            decimal priceInList = calculations.MainPrice(PaperType, SheetsCount, a4);
            TB_price_tirag.Text = (priceInList * SheetsCount).ToString();
            TB_price_per_sheet.Text = priceInList.ToString();
        }

        /// <summary>
        /// определение формата изделия
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public FormatPaper FromatProduct()
        {
            int width=0;
            int height=0;

            var formatPaper = new FormatPaper();
           // if (RB_ProductFormat_A7.IsChecked) formatPaper(width, height);

            return formatPaper;
        }
        // 74, 105 
        //105, 148
        // 148, 210
        //210, 297      
        //420, 297
    }
}
