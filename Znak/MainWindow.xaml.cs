using Logic;
using Logic.Model;
using System;
using System.Collections;
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

            new MainView().Show();
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
        FormatPaper formatPaper => CurrentFormatPaperSource?.FormatPaper ?? new();

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

        public event PropertyChangedEventHandler PropertyChanged; //???????????
        #endregion variables

        /// <summary>
        /// метод расчета стоимости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void But_Price_Click(object sender, RoutedEventArgs e)
        {
            //провкрка выбрана ли цветность и тип бумаги
            if ((RB_4_0.IsChecked == false && RB_4_4.IsChecked == false && RB_1_0.IsChecked == false && RB_1_1.IsChecked == false)
                || PaperType is null)
                return;
            //проверка выбран ли формат бумаги
            if (formatPaper.IsZero || PaperType is null)
                return;
            //устанавливаем значение цаетности и сторонности в зависимости от активных radioButton
            calculations.color = (RB_4_0.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);
            calculations.sidePrint = (RB_1_1.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);

            //устанавливаем значение дилерской цены в зависимости от активности checkBox
            calculations.diler = CB_Dealers.IsChecked ?? false;
            // цена за лист
            decimal priceInList = calculations.MainPrice(PaperType, SheetsCount, a4);
            TB_price_tirag.Text = Math.Round((priceInList * SheetsCount), 1).ToString() + " p";
            TB_price_per_sheet.Text = priceInList.ToString() + " p";

            Draw();
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
            //устанавливаем значение блидов в зависимости от активности checkBox
            int bleeds =0;
            if (CB_bleeds.IsChecked == true) bleeds += 4;

            if (RB_ProductFormat_A7.IsChecked == true) { WidthNull = 74 + bleeds;  HeightNull = 105 + bleeds; }
            if (RB_ProductFormat_A6.IsChecked == true) { WidthNull = 105 + bleeds; HeightNull = 148 + bleeds; }
            if (RB_ProductFormat_A5.IsChecked == true) { WidthNull = 148 + bleeds; HeightNull = 210 + bleeds; }
            if (RB_ProductFormat_A4.IsChecked == true) { WidthNull = 210 + bleeds; HeightNull = 297 + bleeds; }
            if (RB_ProductFormat_A3.IsChecked == true) { WidthNull = 420 + bleeds; HeightNull = 297 + bleeds; }
                    
            QuantityOnSheet();
            Tirag();
            
        }

        public record FormatPaperSource(FormatPaper FormatPaper, string Name);
        public ListCollectionView FormatPaperItemSource => new ListCollectionView( new List<FormatPaperSource>()
        {
            new FormatPaperSource(new FormatPaper(200, 287), "A4"),
            new FormatPaperSource(new FormatPaper(410, 287), "A3"),
            new FormatPaperSource(new FormatPaper(440, 310), "SRA3"),
            new FormatPaperSource(new FormatPaper(315, 460), "325X470"),
            new FormatPaperSource(new FormatPaper(320, 475), "330X485"),
    });

        public FormatPaperSource CurrentFormatPaperSource { get; set; }

        /// <summary>
        /// заполнение TextBox количество на листе
        /// </summary>
        private void QuantityOnSheet()
        {
            // заполнение TextBox количество на листе
            quantityOnSheetNull = calculations.QuantityProducts(formatPaper, ProductFormat());
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
            try
            {
                SheetsCountNull = (int)Math.Ceiling((double)productNull / (double)quantityOnSheetNull);
                if (SheetsCountNull <= 0) SheetsCountNull = 0;
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
            Bleeds();
        }

        /// <summary>
        /// Выбор формата бумаги через radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioComponent_Checked(object sender, RoutedEventArgs e)
        {
            a4 = CurrentFormatPaperSource?.Name == "A4";

            QuantityOnSheet();
            Tirag();
        }

        /// <summary>
        /// прибавление блидов к изделию
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bleeds()
        {
            
            //устанавливаем значение блидов в зависимости от активности checkBox
            if (calculations.bleeds == true) 
            { 
                WidthNull += 4; HeightNull += 4; 
            }
            else if(calculations.bleeds == false)
            {
                WidthNull -= 4; HeightNull -= 4;
            }


        }

        private void Draw()
        {
            var formatPaper = CurrentFormatPaperSource.FormatPaper;
            var formatProduct = ProductFormat();
            FlipFormatProduct();

            Canvas.Children.Clear();

            Canvas.Children.Add( new Rectangle()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 0, 0),
                Visibility = System.Windows.Visibility.Visible,
                StrokeThickness = 4,
                Stroke = System.Windows.Media.Brushes.Black,
                Height = formatPaper.Height,
                Width = formatPaper.Width,
            });

            int x = 0, y = 0;
            while(y + formatProduct.Height < formatPaper.Height)
            {
                while(x + formatProduct.Width < formatPaper.Width)
                {
                    Canvas.Children.Add(CreateRectangle(x,y,formatProduct.Width,formatProduct.Height));
                    x += formatProduct.Width;
                }
                x = 0;
                y += formatProduct.Height;
            }

            void FlipFormatProduct()
            {
                int x1 = formatPaper.Width / formatProduct.Width;
                int x2 = formatPaper.Height / formatProduct.Height;

                int y1 = formatPaper.Width / formatProduct.Height;
                int y2 = formatPaper.Height / formatProduct.Width;

                int x = x1 * x2;
                int y = y1 * y2;
                // Поворачиваем продукт
                if (y > x)
                    formatProduct = new FormatPaper(formatProduct.Height, formatProduct.Width);
            }
        }

        

        private Rectangle CreateRectangle(double X1, double Y1, double X2, double Y2) =>
            new Rectangle()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(X1, Y1, 0, 0),
                Visibility = System.Windows.Visibility.Visible,
                StrokeThickness = 4,
                Stroke = System.Windows.Media.Brushes.Black,
                Height = Y2,
                Width = X2,
            };
    }
}
