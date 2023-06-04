using Logic;
using Logic.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            // загружает прайс лист
            PriceList = PriceManager.GetPrices();
            // строчка для работы биндингов
            DataContext = this;

           // new MainView().Show();
        }
        #region variables
        /// <summary>
        /// колличество изделий в тираже
        /// </summary>
        public int product { get; set; }
        
        /// <summary>
        /// колличество изделий на листе
        /// </summary>
        public int quantityOnSheet { get; set; }

        /// <summary>
        /// сохранение формата бумаги отностиельно активных radioButton
        /// </summary>
        FormatPaper formatPaper = new();

        /// <summary>
        /// тип бумаги выбранный пользователем в ComboBox
        /// </summary>
        public Price PaperType { get; set; }

        /// <summary>
        /// ширина изделия, поле биндится к TB_width
        /// </summary>
        public int WidthProducts { get; set; }   

        /// <summary>
        /// высота изделия, поле биндится к TB_height
        /// </summary>
        public int HeightProducts { get; set; }   

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

        public event PropertyChangedEventHandler PropertyChanged; //???????????
        #endregion variables

        /// <summary>
        /// метод расчета стоимости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Price_Click(object sender, RoutedEventArgs e)
        {
            //проверка выбрана ли цветность и тип бумаги
            if ((RB_4_0.IsChecked == false && RB_4_4.IsChecked == false && RB_1_0.IsChecked == false && RB_1_1.IsChecked == false)
                || PaperType is null)
                return;
            //проверка выбран ли формат бумаги
            if ((RB_PaperFormatA4.IsChecked == false && RB_PaperFormatA3.IsChecked == false && RB_PaperFormatSRA3.IsChecked == false && RB_PaperFormat_325X470.IsChecked == false && RB_PaperFormat_330X485.IsChecked == false)
                || PaperType is null)
                return;
            //устанавливаем значение цветности и сторонности в зависимости от активных radioButton
            calculations.color = (RB_4_0.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);
            calculations.sidePrint = (RB_1_1.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);

            //устанавливаем значение дилерской цены в зависимости от активности checkBox
            calculations.diler = CB_Dealers.IsChecked ?? false;
            // цена за лист
            decimal priceInList = calculations.MainPrice(PaperType, SheetsCount, a4);
            TB_price_tirag.Text = Math.Round((priceInList * SheetsCount), 1).ToString() + " p";
            TB_price_per_sheet.Text = priceInList.ToString() + " p";
            
        }

        /// <summary>
        /// определение формата изделия
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public FormatPaper ProductFormat()
        {                             
            FormatPaper formatPaper = new FormatPaper(WidthProducts,HeightProducts);

            return formatPaper;
        }
        /// <summary>
        /// заполнение полей высоты и ширины изделия отностиельно активных radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_ProductFormat_Checked(object sender, RoutedEventArgs e)
        {
            //устанавливаем значение блидов в зависимости от активности checkBox
            int bleeds =0;
            // if (CB_bleeds.IsChecked == true) bleeds += 4;
            CB_bleeds.IsChecked = false; // выключаем блиды

            if (RB_ProductFormat_A7.IsChecked == true) { WidthProducts = 74 + bleeds;  HeightProducts = 105 + bleeds; }
            if (RB_ProductFormat_A6.IsChecked == true) { WidthProducts = 105 + bleeds; HeightProducts = 148 + bleeds; }
            if (RB_ProductFormat_A5.IsChecked == true) { WidthProducts = 148 + bleeds; HeightProducts = 210 + bleeds; }
            if (RB_ProductFormat_A4.IsChecked == true) { WidthProducts = 210 + bleeds; HeightProducts = 297 + bleeds; }
            if (RB_ProductFormat_A3.IsChecked == true) { WidthProducts = 420 + bleeds; HeightProducts = 297 + bleeds; }
                
            QuantityOnSheet();
            Tirag();        
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
            if (RB_PaperFormatSRA3.IsChecked == true) formatPaper = new FormatPaper(440, 310);
            if (RB_PaperFormat_325X470.IsChecked == true) formatPaper = new FormatPaper(315, 460);
            if (RB_PaperFormat_330X485.IsChecked == true) formatPaper = new FormatPaper(320, 475);

            //оределяет выбран лист А4 или нет
            a4 = RB_PaperFormatA4.IsChecked ?? false;

            QuantityOnSheet();
            Tirag();
        }
        /// <summary>
        /// заполнение TextBox количество на листе
        /// </summary>
        private void QuantityOnSheet()
        {
            // заполнение TextBox количество на листе
            quantityOnSheet = calculations.QuantityProducts(formatPaper, ProductFormat());
        }

        /// <summary>
        /// заполнение TextBox количество на листе при ручном изменении формата изделия
        /// </summary>
        private void TB_height_width_TextChanged(object sender, TextChangedEventArgs e)
        {
            //WidthProducts = 0;
            //HeightProducts = 0;
            //if (CB_bleeds.IsChecked == true)
            //{
               //WidthProducts += 4;
               //HeightProducts += 4;
            //}
            QuantityOnSheet();  /*CB_bleeds.IsChecked = false*/;         
        }
        
        /// <summary>
        /// расчет тиража от количества изделий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_Products_TextChanged(object sender, TextChangedEventArgs e) {Tirag();}

        /// <summary>
        /// расчет тиража от количества изделий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tirag()
        {
            try
            {
                SheetsCount = (int)Math.Ceiling((double)product / (double)quantityOnSheet);
                if (SheetsCount <= 0) SheetsCount = 0;
            }
            catch (Exception ) { }
            
        }

        /// <summary>
        /// прибавление блидов к изделию относительно активности CB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CB_bleeds_Checked(object sender, RoutedEventArgs e) 
        {
            //устанавливаем значение дилерской цены в зависимости от активности checkBox
            calculations.bleeds = CB_bleeds.IsChecked ?? false;

            //устанавливаем значение блидов в зависимости от активности checkBox
            if (calculations.bleeds == true)
            {
                WidthProducts += 4; HeightProducts += 4;
            }
            else if (calculations.bleeds == false)
            {
                WidthProducts -= 4; HeightProducts -= 4;
            }
        }

      /// <summary>
      /// открытие окна плотерной печати
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>      
        private void But_Ploter_Click(object sender, RoutedEventArgs e)
        {
            WindowPloter ploter = new();
            ploter.ShowDialog();
        }
        /// <summary>
        /// событие постановки курсора в TB_height
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_height_GotFocus(object sender, RoutedEventArgs e)
        {
            CB_bleeds.IsChecked = false;
        }
    }
}
