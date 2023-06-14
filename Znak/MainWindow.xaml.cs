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
            PriceList = PriceManager.GetPrices(PriceManager.pricesPath);
            // строчка для работы биндингов
            DataContext = this;          
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
            CB_bleeds.IsChecked = false; // выключаем блиды после каждого изменения формата изделия

            if (RB_ProductFormat_Vizitka.IsChecked == true) { WidthProducts = 50; HeightProducts = 90; }
            if (RB_ProductFormat_A7.IsChecked == true) { WidthProducts = 74;  HeightProducts = 105; }
            if (RB_ProductFormat_A6.IsChecked == true) { WidthProducts = 105; HeightProducts = 148; }
            if (RB_ProductFormat_A5.IsChecked == true) { WidthProducts = 148; HeightProducts = 210; }
            if (RB_ProductFormat_A4.IsChecked == true) { WidthProducts = 210; HeightProducts = 297; }
            if (RB_ProductFormat_A3.IsChecked == true) { WidthProducts = 420; HeightProducts = 297; }
                
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

            LB_Star_FormatPeper.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора формата бумаги

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
           QuantityOnSheet();          
        }
        
        /// <summary>
        /// расчет тиража от количества изделий
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_Products_TextChanged(object sender, TextChangedEventArgs e) 
        {
          Tirag();
        }

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
                else LB_Star_Sheets.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора типа бумаги
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
        /// событие постановки курсора в TB_height сбрасывает диллерскую галочку при каждом изменении параметров изделия
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_height_GotFocus(object sender, RoutedEventArgs e)
        {
            CB_bleeds.IsChecked = false;
        }

        private void WindowPloter_Loaded(object sender, RoutedEventArgs e) //НАЙТИ ГДЕ ОН ЕСТЬ И УДАЛИТЬ НАХУЙ!!!!!!!
        {

        }

		/// <summary>
		/// отключение видимости звездочки после типа бумаги
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CB_Materials_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LB_Star_Peper.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора типа бумаги

		}
		/// <summary>
		/// отключение видимости звездочки после постановки курсора в поле количества листов
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TB_Sheets_GotFocus(object sender, RoutedEventArgs e)
		{
			LB_Star_Sheets.Visibility = Visibility.Collapsed;  // отключение видимости звездочки после постановки курсора в поле количества листов
		}

		/// <summary>
		/// отключение видимости звездочки после выбора цвета печати
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RB_Color_Checked(object sender, RoutedEventArgs e)
		{
			LB_Star_Color.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора цвета печати
		}
	}
}
