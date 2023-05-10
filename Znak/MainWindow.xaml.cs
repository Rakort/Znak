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
        }
        #region variables
        /// <summary>
        /// колличество изделий в тираже
        /// </summary>
        // //int? со значением null, для отсутствия ноля в TextBox до присвоения значения полю
        public int? productNull { get; set; }
        public int product => productNull ?? 0;

        /// <summary>
        /// колличество изделий на листе
        /// </summary>
        // //int? со значением null, для отсутствия ноля в TextBox до присвоения значения полю
        public int? quantityOnSheetNull { get; set; }
        public int quantityOnSheet => quantityOnSheetNull ?? 0;

        /// <summary>
        /// сохранение формата бумаги отностиельно активных radioButton
        /// </summary>
        FormatPaper formatPaper = new();

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

        public Calculations calculations { get; set; } = new Calculations();

        // Из интерфейса INotifyPropertyChanged. Нужно для обновления зачаний в визуальных компонентах
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion variables

        /// <summary>
        /// метод расчета стоимости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Price_Click(object sender, RoutedEventArgs e)
        {
            if (!calculations.IsValid())
                return;
            //провкрка выбрана ли цветность и тип бумаги
            if (RB_4_0.IsChecked == false && RB_4_4.IsChecked == false && RB_1_0.IsChecked == false && RB_1_1.IsChecked == false)
                return;
            //проверка выбран ли формат бумаги
            if (RB_PaperFormatA4.IsChecked == false && RB_PaperFormatA3.IsChecked == false && RB_PaperFormatSRA3.IsChecked == false && RB_PaperFormat_325X470.IsChecked == false && RB_PaperFormat_330X485.IsChecked == false)
                return;
            //устанавливаем значение цаетности и сторонности в зависимости от активных radioButton
            calculations.color = (RB_4_0.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);
            calculations.sidePrint = (RB_1_1.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);

            // цена за лист
            decimal priceInList = calculations.MainPrice(SheetsCount, a4);
            TB_price_tirag.Text = Math.Round((priceInList * SheetsCount), 1).ToString() + " p";
            TB_price_per_sheet.Text = priceInList.ToString() + " p";

        }

        /// <summary>
        /// определение формата изделия
        /// </summary>
        public FormatPaper ProductFormat => new FormatPaper(WidthP, HeightP);

        /// <summary>
        /// заполнение полей высоты и ширины изделия отностиельно активных radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_ProductFormat_Checked(object sender, RoutedEventArgs e)
        {
            if (RB_ProductFormat_A7.IsChecked == true) { WidthNull = 74; HeightNull = 105; }
            if (RB_ProductFormat_A6.IsChecked == true) { WidthNull = 105; HeightNull = 148; }
            if (RB_ProductFormat_A5.IsChecked == true) { WidthNull = 148; HeightNull = 210; }
            if (RB_ProductFormat_A4.IsChecked == true) { WidthNull = 210; HeightNull = 297; }
            if (RB_ProductFormat_A3.IsChecked == true) { WidthNull = 420; HeightNull = 297; }

            //устанавливаем значение блидов в зависимости от активности checkBox
            if (calculations.bleeds) { WidthNull += 4; HeightNull += 4; }

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
            quantityOnSheetNull = calculations.QuantityProducts(formatPaper, ProductFormat);
        }

        /// <summary>
        /// заполнение TextBox количество на листе при ручном изменении формата изделия
        /// </summary>
        private void TB_height_width_TextChanged(object sender, TextChangedEventArgs e) { QuantityOnSheet();}
        
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
            if (productNull.HasValue && quantityOnSheetNull.HasValue && quantityOnSheetNull > 0)
            { 
                SheetsCountNull = productNull / quantityOnSheetNull;
                if (SheetsCountNull <= 0)
                    SheetsCountNull = 0; 
            }
            
        }

        /// <summary>
        /// прибавление блидов к изделию относительно активности CB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CB_bleeds_Checked(object sender, RoutedEventArgs e) 
        {
            //устанавливаем значение дилерской цены в зависимости от активности checkBox
            Bleeds();
        }

        /// <summary>
        /// прибавление блидов к изделию
        /// </summary>
        private void Bleeds()
        {
            //устанавливаем значение блидов в зависимости от активности checkBox
            if (calculations.bleeds) 
            { 
                WidthNull += 4; HeightNull += 4; 
            }
            else
            {
                WidthNull -= 4; HeightNull -= 4;
            }


        }
    }
}
