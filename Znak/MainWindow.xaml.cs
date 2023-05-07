using Logic;
using Logic.Model;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Znak
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            // загружает прайс лист
            PriceList = PriceManager.GetPrices();
            // строчка для работы биндингов
            DataContext = this;
            
        }
        /// <summary>
        /// сохранение формата бумаги отностиельно активных radioButton
        /// </summary>
        FormatPaper formatPaper = new();

        /// <summary>
        /// тип бумаги выбранный пользователем в ComboBox
        /// </summary>
        public Price PaperType { get; set; }

        //int? со значением null, для отсутствия ноля в TextBox до присвоения значения полю
        /// <summary>
        /// ширина изделия, поле биндится к TB_width
        /// </summary>
        public int? WidthNull { get; set; }   
        public int WidthP => WidthNull ?? 0;

        /// <summary>
        /// высота изделия, поле биндится к TB_height
        /// </summary>
        //int? со значением null, для отсутствия ноля в TextBox до присвоения значения полю
        public int? HeightNull { get; set; }   
        public int HeightP => HeightNull ?? 0;

        /// <summary>
        /// количество листов в тираже
        /// </summary>
        // int? со значением null, для отсутствия ноля в TextBox до присвоения значения полю
        public int? SheetsCountNull { get; set; }
        /// <summary>
        /// количество листов в тираже
        /// </summary>
        public int SheetsCount => SheetsCountNull ?? 0;

        /// <summary>
        /// A4 или А3 лист "false == a3"
        /// </summary>
        public bool a4 = false; 

        /// <summary>
        /// прайс лист
        /// </summary>
        public List<Price> PriceList { get; set; }

        Calculations calculations = new Calculations();

        public event PropertyChangedEventHandler PropertyChanged;

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
        public FormatPaper ProductFormat()
        {                             
            FormatPaper formatPaper = new FormatPaper(WidthP,HeightP);

            return formatPaper;
        }
        /// <summary>
        /// заполнение полей высоты и ширины изделия отностиельно активных radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_ProductFormat_Checked(object sender, RoutedEventArgs e)
        {
            if (RB_ProductFormat_A7.IsChecked == true) { WidthNull = 74;  HeightNull = 105; }
            if (RB_ProductFormat_A6.IsChecked == true) { WidthNull = 105; HeightNull = 148; }
            if (RB_ProductFormat_A5.IsChecked == true) { WidthNull = 148; HeightNull = 210; }
            if (RB_ProductFormat_A4.IsChecked == true) { WidthNull = 210; HeightNull = 297; }
            if (RB_ProductFormat_A3.IsChecked == true) { WidthNull = 420; HeightNull = 297; }
        }

        /// <summary>
        /// определение формата бумаги отностиельно активных radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_PaperFormat_Checked(object sender, RoutedEventArgs e)
        {          
            if (RB_PaperFormatA4.IsChecked == true) formatPaper = new FormatPaper(200, 287);
            if (RB_PaperFormatA3.IsChecked == true) formatPaper = new FormatPaper(410, 287);
            if (RB_PaperFormat_325X470.IsChecked == true) formatPaper = new FormatPaper(315, 460);
            if (RB_PaperFormat_330X485.IsChecked == true) formatPaper = new FormatPaper(320, 475);
            //оределяет выбран лист А4 или нет
            a4 = RB_PaperFormatA4.IsChecked ?? false;
        }
     
        private void TB_height_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        
    }
}
