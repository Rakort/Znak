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

            //загрузка цен
            PricePloter = PriceManager.DefaultPricesPloter;

            //загрузка параметров ширины рулонов
            WidthPloterRoll = PriceManager.DefaultWidthPloterRoll;
            DataContext = this;
        }

        #region Variables
        public double widthP { get; set; }
        public double heightP { get; set; }

        /// <summary>
        /// количество изделий
        /// </summary>
        public double quantityP { get; set; }

        /// <summary>
        /// площадь печати
        /// </summary>
        public double printArea { get; set; }

        /// <summary>
        /// площадь свободного поля
        /// </summary>
        public double freeFieldArea { get; set; }

        /// <summary>
        /// цена за метр
        /// </summary>
        public decimal priceMeter { get; set; }

        /// <summary>
        /// цена печати
        /// </summary>
        public decimal printingPrice { get; set; }

        /// <summary>
        /// цена свободного поля
        /// </summary>
        public decimal freeFieldPrice { get; set; }

        /// <summary>
        /// цена общая
        /// </summary>
        public decimal PricePlot { get; set; }

        public bool diller;

        #endregion Variables

        /// <summary>
        /// поле с материалами и ценами биндится к ComboBox
        /// </summary>
        public List<PricePloter> PricePloter { get; set; }

        /// <summary>
        /// тип бумаги выбранный пользователем в ComboBox
        /// </summary>
        public PricePloter PloterPeperType { get; set; }

        /// <summary>
        /// пареметры ширины рулонов биндится к ComboBox
        /// </summary>
        public List<WidthPloterRoll> WidthPloterRoll { get; set; }

        /// <summary>
        /// штрина рулона выбранная пользователем в ComboBox
        /// </summary>
        public WidthPloterRoll PloterWidthType { get; set; }

        /// <summary>
        /// заполнение полей высоты и ширины изделия отностиельно активных radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_ProductFormatPlot_Checked(object sender, RoutedEventArgs e)
        {

            if (RB_FormatPloter_A2.IsChecked == true) { widthP = 594; heightP = 420; }
            if (RB_FormatPloter_A1.IsChecked == true) { widthP = 841; heightP = 594; }
            if (RB_FormatPloter_A0.IsChecked == true) { widthP = 1189; heightP = 841; }

        }
        /// <summary>
        /// главный метод расчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BT_Calculation_Click(object sender, RoutedEventArgs e)
        {
            //устанавливаем значение дилерской цены в зависимости от активности checkBox
            diller = CB_dillerPlot.IsChecked ?? false;

            //проверка если выбран материал
            if (PloterPeperType != null)
            {
                //установка цены за метр
                if (diller)
                    priceMeter = PloterPeperType.PriceDiller;
                else
                    priceMeter = PloterPeperType.Price;
            }
            else return;
            //проверка на звполнение всех необходимах полей
            if(widthP!=null && heightP!=null && PloterWidthType != null && quantityP > 0) 
            {
                calculationAreaPrintedField();
                CalcfreeFieldArea();
                CalcPricePlot();
            }
            else return;
        }

        /// <summary>
        /// расчет площади печатного поля
        /// </summary>
        /// <returns></returns>
        public void calculationAreaPrintedField()
        {
            if (widthP > 1570 && heightP > 1570 || quantityP <= 0) return;
            else
            {
                printArea = Math.Round(((widthP / 1000) * (heightP / 1000)) * quantityP, 2);

            }
        }

        // работа со свободным полем

        /// <summary>
        /// количество изделий умещающихся на ширене рулона вариант #1
        /// </summary>
        int quantityInRoll;

        /// <summary>
        /// количество изделий умещающихся на ширене рулона вариант #2
        /// </summary>
        int quantityInRoll2; 

        /// <summary>
        /// сторона изделия #1
        /// </summary>
        double productSide; 

        /// <summary>
        /// сторона изделия #2
        /// </summary>
        double productSide2; 

        /// <summary>
        /// расчет отрезка материала для вычесления площади сп и инициализация многих переменных
        /// </summary>
        public void Calc()
        {
            //количество изделий на ширене рулона разложенных шириной
            int quantityInWidth = (int)(PloterWidthType.WidthRoll / widthP);

            //количество изделий на ширене рулона разложенных высотой
            int quantityInHeight = (int)(PloterWidthType.WidthRoll / heightP);


            if (heightP > PloterWidthType.WidthRoll)
            {
                quantityInRoll = quantityInWidth;
                productSide = widthP;
                productSide2 = heightP;
            }

            else if (widthP > PloterWidthType.WidthRoll)
            {
                quantityInRoll = quantityInHeight;
                productSide = heightP;
                productSide2 = widthP;
            }

            else
            {
                quantityInRoll = quantityInWidth;
                quantityInRoll2 = quantityInHeight;
                productSide = widthP;
                productSide2 = heightP;
            }

        }

        /// <summary>
        /// расчет площади свободного поля
        /// </summary>
        /// <returns></returns>
        public void CalcfreeFieldArea()
        {
            Calc();

            double materialQuantity = 0; //количество полосок ширины материала нужное для размещения всего количества изделий вар1
            double materialQuantity2 = 0; //количество полосок ширины материала нужное для размещения всего количества изделий вар2

            int x = 0;
            
                while (x < quantityP)
                {
                    x = x + quantityInRoll;
                materialQuantity += 1;
                }

                if (quantityInRoll2 > 0)
                {
                    int y = 0;
                    while (y < quantityP)
                    {
                        y = y + quantityInRoll2;
                    materialQuantity2 += 1;
                    }
                }
                else materialQuantity2 += 1;
                //площадь отрезка материала для печати данного количества изделий с вариантом раскладки 1
                double piece = ((PloterWidthType.WidthRoll / 1000) * ((productSide / 1000) * materialQuantity2));

                //площадь отрезка материала для печати данного количества изделий с вариантом раскладки 2
                double piece2 = ((PloterWidthType.WidthRoll / 1000) * ((productSide2 / 1000) * materialQuantity));

            //площади свободного поля
            double square = piece - printArea;
            double square2 = piece2 - printArea;

            //выводим меньшую площадь
            if (square > 0 && square < square2)         freeFieldArea = Math.Round(square,2);

            else if (square2 > 0 && square > square2)   freeFieldArea = Math.Round(square2, 2);

            else if (square2 > 0 && square2 == square)  freeFieldArea = Math.Round(square, 2);

            else                                        freeFieldArea = 0;

        }

        /// <summary>
        /// расчет цен печати, свободного поля и общей стоимости заказа
        /// </summary>
        public void CalcPricePlot() 
        {
            //расчет цены печати
            printingPrice = (decimal)printArea * priceMeter;
            
            //расчет цены свп
            freeFieldPrice = (decimal)freeFieldArea * PloterPeperType.FreeFieldPrice;

            //общая цена заказа
            PricePlot = printingPrice + freeFieldPrice;
        }
        
    }
}
