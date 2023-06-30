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
    public partial class WindowPloter : UserControl, INotifyPropertyChanged
    {

        
        public WindowPloter()
        {
            InitializeComponent();

            //загрузка цен
            PricePloter = PriceManager.GetPricesPlot(PriceManager.pricesPlotPath);
            PostPechPrice = PriceManager.GetPostPechPrice(PriceManager.PostPechLacerPath);
            //загрузка параметров ширины рулонов
            WidthPloterRoll = PriceManager.GetWidthPlot(PriceManager.WidthPlotPath);
            DataContext = this;

            // количество изделий по умолчанию 
			quantityP = 1;

			InicialPostPehc();
        }
		
		#region Variables

			/// <summary>
			/// интервал через который ставятся люверсы в мм
			/// </summary>
           
			decimal Interval;

			/// <summary>
			/// цена за один люверс
			/// </summary>
			
			decimal PriceLuvers;

			/// <summary>
			/// цена за погонный метр проклейки
			/// </summary>
           
            decimal PriceSizings;

			/// <summary>
			/// цена за м кв ламинации
			/// </summary>
            
            decimal PriceLam = 350;

        public decimal widthP { get; set; }
        public decimal heightP { get; set; }

        /// <summary>
        /// количество изделий
        /// </summary>
        public decimal quantityP { get; set; }

        /// <summary>
        /// площадь печати
        /// </summary>
        public decimal printArea { get; set; }

        /// <summary>
        /// площадь свободного поля
        /// </summary>
        public decimal freeFieldArea { get; set; }

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
        
        /// <summary>
		/// стоимость проклейки
		/// </summary>
		decimal Sizings = 0;
 
		/// <summary>
		/// стоимость люверсов на изделия
		/// </summary>
		decimal luversQuantityPrice = 0;

		/// <summary>
		/// ламинация пленки стоиммость на изделие
		/// </summary>
		decimal lam = 0;

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
		/// прайс лист постпечати
		/// </summary>
		public List<PostPechPrice> PostPechPrice { get; set; }

        /// <summary>
        /// штрина рулона выбранная пользователем в ComboBox
        /// </summary>
        public WidthPloterRoll PloterWidthType { get; set; }

		/// <summary>
		/// инициализация цен постпечати
		/// </summary>
		public void InicialPostPehc()
		{
			PostPechPrice _postPechPrice = (PostPechPrice)PostPechPrice.Where(x => x.Measure.Contains("люверсы банерные")).FirstOrDefault();
			PriceLuvers = _postPechPrice.PostPech_Price;
            PostPechPrice _postPechPrice1 = (PostPechPrice)PostPechPrice.Where(x => x.Measure.Contains("интервал люверсов")).FirstOrDefault();
			Interval = _postPechPrice1.PostPech_Price;
			PostPechPrice _postPechPrice2 = (PostPechPrice)PostPechPrice.Where(x => x.Measure.Contains("проклейка")).FirstOrDefault();
			PriceSizings = _postPechPrice2.PostPech_Price;
            PostPechPrice _postPechPrice3 = (PostPechPrice)PostPechPrice.Where(x => x.Measure.Contains("ламинация пленки")).FirstOrDefault();
			PriceLam = _postPechPrice3.PostPech_Price;
		}

        /// <summary>
        /// заполнение полей высоты и ширины изделия отностиельно активных radioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RB_ProductFormatPlot_Checked(object sender, RoutedEventArgs e)
        {

            if (RB_FormatPloter_A2.IsChecked == true) { widthP = 594; heightP = 420; }
            if (RB_FormatPloter_A1.IsChecked == true) { widthP = 841; heightP = 594; }
            if (RB_FormatPloter_A0.IsChecked == true) { widthP = 1189; heightP = 841;}

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
            if(widthP > 0 && heightP > 0 && PloterWidthType != null && quantityP > 0) 
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
        decimal productSide;

        /// <summary>
        /// сторона изделия #2
        /// </summary>
        decimal productSide2; 

        /// <summary>
        /// расчет отрезка материала для вычесления площади сп и инициализация многих переменных
        /// </summary>
        public void Calc()
        {
            
				//количество изделий на ширине рулона разложенных шириной
				int quantityInWidth = (int)(PloterWidthType.WidthRoll / widthP);

				//количество изделий на ширине рулона разложенных высотой
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

            decimal materialQuantity = 0; //количество полосок ширины материала нужное для размещения всего количества изделий вар1
            decimal materialQuantity2 = 0; //количество полосок ширины материала нужное для размещения всего количества изделий вар2

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
            decimal piece = ((PloterWidthType.WidthRoll / 1000) * ((productSide / 1000) * materialQuantity2));

            //площадь отрезка материала для печати данного количества изделий с вариантом раскладки 2
            decimal piece2 = ((PloterWidthType.WidthRoll / 1000) * ((productSide2 / 1000) * materialQuantity));

            //площади свободного поля
            decimal square = piece - printArea;
            decimal square2 = piece2 - printArea;

            //выводим меньшую площадь
            if (square > 0 && square < square2 || square2 <= 0)       freeFieldArea = Math.Round(square,2);

            else if (square2 > 0 && square > square2 || square <= 0)   freeFieldArea = Math.Round(square2, 2);

            else if (square2 > 0 && square2 == square)  freeFieldArea = Math.Round(square, 2);

            else                                        freeFieldArea = 0;

        }

        /// <summary>
        /// расчет цен печати, свободного поля и общей стоимости заказа
        /// </summary>
        public void CalcPricePlot() 
        {
            //расчет цены печати + округление до сотых
            printingPrice = Math.Round( printArea * priceMeter, 2);

			//расчет цены свп + округление до сотых
			if (CB_SvP.IsChecked == true) //проверка включен ли СB
				freeFieldPrice = Math.Round(freeFieldArea * PloterPeperType.FreeFieldPrice, 2);
			else freeFieldPrice = 0;

            //общая цена заказа + округление до сотых
            PricePlot = Math.Round(printingPrice + freeFieldPrice + Sizings + luversQuantityPrice + lam, 2);
        }

		#region отключение звездочек
		/// <summary>
		/// отключение видимости звездочки после выбора материала
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CB_MaterialsPloter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
				LB_Star_Material.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора типа бумаги

		}
        /// <summary>
		/// отключение видимости звездочки после выбора ширины рулона
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CB_WidthPloterRoll_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			LB_Star_Roll_Width.Visibility = Visibility.Collapsed;
		}

        /// <summary>
		/// отключение видимости звездочки после
		/// постановки курсора в поле формат изделия
		/// и брос люверсов и других параметров
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TB_widthP_GotFocus(object sender, RoutedEventArgs e)
		{
            LB_Star_Roll_Product_Format.Visibility = Visibility.Collapsed;

			//отключене люверсов
			CB_Luvers.IsChecked = false;
            //отключене проклейки по периметру
			CB_Sizing_Perim.IsChecked = false;
			//ламинации пленки
			CB_Lam.IsChecked = false;
			//свободного поля
            CB_SvP.IsChecked = false;	
			
		}
		#endregion отключение звездочек

       /// <summary>
		/// Сброс всех данных
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void But_Reset_Click(object sender, RoutedEventArgs e)
		{      		
			widthP = 0;
            heightP = 0;     
            quantityP = 1;     
            printArea = 0;
			freeFieldArea = 0;   
            priceMeter = 0;       
            printingPrice = 0;
            freeFieldPrice = 0;
            PricePlot = 0;

		   // очищение комбобоксов с шириной и материалом
		   CB_MaterialsPloter.SelectedIndex = -1;
		   CB_WidthPloterRoll.SelectedIndex = -1;

            //цикл по всем элементам грида
			foreach (var control in Ploter_Grid.Children)
            {
			   //выключаем все RadioButton
			   if (control is RadioButton)
			     {
				    RadioButton radioButton = (RadioButton)control;
				    radioButton.IsChecked = false;
			     }      
               //выключаем все чекбоксы
			   if (control is CheckBox)
			     {
				    CheckBox checkBox = (CheckBox)control;
				    checkBox.IsChecked = false;
			     }      
              //возвращаем видимость звездочкам обязательных для заполнения полей
               if (control is Label)
			     {
				    Label label = (Label)control;
				    label.Visibility = Visibility;
			     }   
            }
	   	}

		/// <summary>
		/// Добавление люверсов
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CB_Luvers_Checked(object sender, RoutedEventArgs e)

		{   

			//количество люверсов в изделиях
			decimal luversQuantity = (Math.Floor((widthP + heightP) / Interval) * 2)* quantityP;

			luversQuantityPrice = luversQuantity * PriceLuvers;

			TB_LuversQuantity.Text = luversQuantity.ToString() + " шт"; // заполнение TB

			TB_LuversPrice.Text = luversQuantityPrice.ToString() + " р"; // заполнение TB

			//устанавливаем значение люверсов в зависимости от активности checkBox
			if (CB_Luvers.IsChecked == true)
			{

				// при включенном чек-боксе меняет цвет текста на красный
				CB_Luvers.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF71313"));
			}
			else 
			{   //очищаем TB с данными о люверсах
				TB_LuversQuantity.Clear();
				TB_LuversPrice.Clear();
                luversQuantityPrice = 0;

				// при выключенном чек-боксе меняет цвет текста на черный
				CB_Luvers.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
			}
		}

		/// <summary>
		/// проклейка по периметру
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CB_Sizing_Perim_Copy_Checked(object sender, RoutedEventArgs e)
		{

			// стоимость проклейки 
			Sizings = (((widthP + heightP) * 2)/1000) * PriceSizings;

			TB_Price_Sizing_Perim.Text = Sizings.ToString() + " р";

			//устанавливаем значение проклейки в зависимости от активности checkBox
			if (CB_Sizing_Perim.IsChecked == true)
			{

				// при включенном чек-боксе меняет цвет текста на красный
				CB_Sizing_Perim.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF71313"));
			}
			else 
			{   //очищаем TB с данными проклейке
				TB_Price_Sizing_Perim.Clear();
				Sizings = 0;
				
				// при выключенном чек-боксе меняет цвет текста на черный
				CB_Sizing_Perim.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
			}
		}

		/// <summary>
		/// расчет ламинации пленки
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CB_Lam_Checked(object sender, RoutedEventArgs e)
		{   
           
            lam = (widthP/1000 * heightP/1000) * PriceLam;

            //заполнение TB
			TB_Price_Lam.Text = Math.Round(lam, 2).ToString() + " р";

            //устанавливаем значение ламинации в зависимости от активности checkBox
			if (CB_Lam.IsChecked == true)
			{
				// при включенном чек-боксе меняет цвет текста на красный
				CB_Lam.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF71313"));
			}
			else 
			{   
                //очищаем TB с данными о люверсах
				TB_Price_Lam.Clear();				
                lam = 0;

				// при выключенном чек-боксе меняет цвет текста на черный
				CB_Lam.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
			}
		}

		 /// <summary>
		 /// добавление свободного поля к стоимости заказа
		 /// </summary>
		 /// <param name="sender"></param>
		 /// <param name="e"></param>
		private void CB_SvP_Checked(object sender, RoutedEventArgs e)
		{
			//проверка на звполнение всех необходимах полей
			if (widthP > 0 && heightP > 0 && PloterWidthType != null && quantityP > 0)
			{ 
				//заполняем ТВ свп
				TB_freeFieldPrice.Text = Math.Round(freeFieldArea * PloterPeperType.FreeFieldPrice, 2).ToString() + " р";
			}

            //устанавливаем значение своб поля в зависимости от активности checkBox
			if (CB_SvP.IsChecked == true)
			{
				// при включенном чек-боксе меняет цвет текста на красный
				CB_SvP.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF71313"));
			}
			else 
			{   
                //очищаем переменную данными свп
								
                freeFieldPrice = 0;

				TB_freeFieldPrice.Clear();

				// при выключенном чек-боксе меняет цвет текста на черный
				CB_SvP.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
			}


		}
	}
}
