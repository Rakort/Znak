using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            PricePloter = PriceManager.GetPricesPlot();
            PostPechPrice = PriceManager.GetPostPechPrice(PriceManager.PostPechLacerPath);
            //загрузка параметров ширины рулонов
            WidthPloterRoll = PriceManager.GetWidthPlot();
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

		/// <summary>
		/// ширина изделия
		/// </summary>
        public decimal widthP { get; set; }

		/// <summary>
		/// высота изделия
		/// </summary>
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

        /// <summary>
        /// стоимость проклейки
        /// </summary>
        decimal Sizings = 0;

		/// <summary>
        /// стоимость склейки
        /// </summary>
        decimal SKleyka = 0;

        /// <summary>
        /// стоимость люверсов на изделия
        /// </summary>
        decimal luversQuantityPrice = 0;

        /// <summary>
        /// ламинация пленки стоиммость на изделие
        /// </summary>
        decimal lam = 0;

#endregion Variables

		#region Variables постпечать

		/// <summary>
		/// длинна склейки биндится к ComboBox
		/// </summary>
		public decimal GluingLength { get; set; }

        /// <summary>
        /// поле с материалами и ценами биндится к ComboBox
        /// </summary>
        public List<PlotterPrice> PricePloter { get; set; }

        /// <summary>
        /// тип бумаги выбранный пользователем в ComboBox
        /// </summary>
        public PlotterPrice PloterPeperType { get; set; }

        /// <summary>
        /// пареметры ширины рулонов биндится к ComboBox
        /// </summary>
        public List<WidthPlotterRoll> WidthPloterRoll { get; set; }

        /// <summary>
        /// прайс лист постпечати
        /// </summary>
        public List<PostPechPrice> PostPechPrice { get; set; }

        /// <summary>
        /// штрина рулона выбранная пользователем в ComboBox
        /// </summary>
        public WidthPlotterRoll PloterWidthType { get; set; }

		#endregion Variables постпечати

		#region расчеты

		/// <summary>
        /// расчет цен печати, свободного поля и общей стоимости заказа
        /// </summary>
        public void CalcPricePlot()
        {
			// расчет стоимости склейки
			if (CB_Sizing_Line.IsChecked == true)
			{
				SKleyka = (GluingLength / 1000) * PriceSizings;

				TB_SkleiPrice.Text = SKleyka.ToString() + " р";
			}

            //расчет цены печати + округление до сотых
            printingPrice = Math.Round(printArea * priceMeter, 2);

			//расчет цены свп + округление до сотых
			if ((widthP < 1550 || heightP < 1550) && CB_SvP.IsChecked == true) //проверка включен ли СB
			{
				freeFieldPrice = Math.Round(freeFieldArea * PloterPeperType.FreeFieldPrice, 2);

				//заполняем ТВ свп
				TB_freeFieldPrice.Text = freeFieldPrice.ToString() + " р";
			}
			else freeFieldPrice = 0;

            //общая цена заказа + округление до сотых
            PricePlot = Math.Round(printingPrice + freeFieldPrice + Sizings + SKleyka + luversQuantityPrice + lam, 2);
        }

        /// <summary>
        /// заполнение полей высоты и ширины изделия отностиельно активных radioButton
        /// </summary>
        private void RB_ProductFormatPlot_Checked(object sender, RoutedEventArgs e)
        {

            if (RB_FormatPloter_A2.IsChecked == true) { widthP = 594; heightP = 420; }
            if (RB_FormatPloter_A1.IsChecked == true) { widthP = 841; heightP = 594; }
            if (RB_FormatPloter_A0.IsChecked == true) { widthP = 1189; heightP = 841; }

        }

        /// <summary>
        /// главный метод расчета
        /// </summary>
        private void BT_Calculation_Click(object sender, RoutedEventArgs e)
        {
            //проверка если выбран материал
            if (PloterPeperType != null)
            {
                    priceMeter = PloterPeperType.Price;
            }
            else { MessageBox messageBox = new("выберите материал"); messageBox.ShowDialog(); return; }

            //проверка на звполнение всех необходимах полей
            if (widthP > 0 && heightP > 0 && PloterWidthType != null && quantityP > 0)
            {
				//if (PloterPeperType.NamePloter.Contains("банер")) return;

                //проверка габариты изделия не должны быть больше ширины выбранного рулона и если это не банер или сетка
                if (PloterPeperType.Name.Contains("Банер") || !(widthP > PloterWidthType.WidthRoll && heightP > PloterWidthType.WidthRoll))
                {
                    calculationAreaPrintedField();
                    CalcfreeFieldArea();
                    CalcPricePlot();
                }
                else { MessageBox messageBox = new("изделие не помещается на выбранную ширину рулона"); messageBox.ShowDialog(); return; }

            }
            else { MessageBox messageBox = new("не выбраны размеры изделия или ширина рулона"); messageBox.ShowDialog(); return; }
        }

        /// <summary>
        /// расчет площади печатного поля
        /// </summary>
        /// <returns></returns>
        public void calculationAreaPrintedField()
        {
            if ((!PloterPeperType.Name.Contains("Банер") && widthP > 1570 && heightP > 1570) || quantityP <= 0) return;
            else
            {
                printArea = Math.Round(((widthP / 1000) * (heightP / 1000)) * quantityP, 2);
            }
        }

		#endregion расчеты

		#region постпечатка

		/// <summary>
		/// инициализация цен постпечати
		/// </summary>
		public void InicialPostPehc()
        {
            PostPechPrice _postPechPrice = PostPechPrice.Where(x => x.Measure.Contains("люверсы банерные")).FirstOrDefault();
            PriceLuvers = _postPechPrice.PostPech_Price;
            PostPechPrice _postPechPrice1 = PostPechPrice.Where(x => x.Measure.Contains("интервал люверсов")).FirstOrDefault();
            Interval = _postPechPrice1.PostPech_Price;
            PostPechPrice _postPechPrice2 = PostPechPrice.Where(x => x.Measure.Contains("проклейка")).FirstOrDefault();
            PriceSizings = _postPechPrice2.PostPech_Price;
            PostPechPrice _postPechPrice3 = PostPechPrice.Where(x => x.Measure.Contains("ламинация пленки")).FirstOrDefault();
            PriceLam = _postPechPrice3.PostPech_Price;
        }

		/// <summary>
		/// Добавление люверсов
		/// </summary>
		private void CB_Luvers_Checked(object sender, RoutedEventArgs e)
        {
            //количество люверсов в изделиях
            decimal luversQuantity = (Math.Floor((widthP + heightP) / Interval) * 2) * quantityP;

            luversQuantityPrice = luversQuantity * PriceLuvers;

            TB_LuversQuantity.Text = luversQuantity.ToString() + " шт"; // заполнение TB

            TB_LuversPrice.Text = luversQuantityPrice.ToString() + " р"; // заполнение TB

			CleanerMax(CB_Luvers,TB_LuversQuantity,TB_LuversPrice,ref luversQuantityPrice, null);        
        }

        /// <summary>
        /// проклейка по периметру
        /// </summary>
        private void CB_Sizing_Perim_Checked(object sender, RoutedEventArgs e)
        {
			if (CB_Sizing_Perim.IsChecked == true)
			{
				// стоимость проклейки периметра
				Sizings = (((widthP + heightP) * 2) / 1000) * PriceSizings;

				TB_Price_Sizing_Perim.Text = Sizings.ToString() + " р";
			}

			CleanerMin(CB_Sizing_Perim,TB_Price_Sizing_Perim,() => Sizings = 0);
        }

        /// <summary>
        /// расчет ламинации пленки
        /// </summary>	
        private void CB_Lam_Checked(object sender, RoutedEventArgs e)
        {
            lam = (widthP / 1000 * heightP / 1000) * PriceLam;

            //заполнение TB
            TB_Price_Lam.Text = Math.Round(lam, 2).ToString() + " р";

			CleanerMin(CB_Lam, TB_Price_Lam, () => lam = 0);
        }

        /// <summary>
        /// Сброс всех данных
        /// </summary>
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
		/// окрашивание проклейки и заполнение ее полей
		/// </summary>
		private void CB_Sizing_Line_Checked(object sender, RoutedEventArgs e)
		{
			//устанавливаем значение проклейки в зависимости от активности checkBox	

			CleanerMax(CB_Sizing_Line, TB_SkleiPrice,TB_Price_Sizing_Line,ref SKleyka, () => GluingLength = 0);
		}

#endregion постпечатка

        #region работа с визуалкой

        /// <summary>
        /// отключение видимости звездочки после выбора материала
        /// </summary>
        private void CB_MaterialsPloter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LB_Star_Material.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора типа бумаги
        }

        /// <summary>
		/// отключение видимости звездочки после выбора ширины рулона
		/// </summary>
		private void CB_WidthPloterRoll_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LB_Star_Roll_Width.Visibility = Visibility.Collapsed;

			CB_SvP.IsChecked = false;	// отключение галочки свп	
        }

        /// <summary>
        /// отключение видимости звездочки после
        /// постановки курсора в поле формат изделия
        /// и брос люверсов и других параметров
        /// </summary>
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

		/// <summary>
		/// устанавливает цвет СВ и очищает все параметры пи отключении СВ
		/// </summary>
		public void CleanerMax(CheckBox checkBox, TextBox textBox1, TextBox textBox2, ref decimal Sum1, Action setter)
		{
			//устанавливаем цвет в зависимости от активности checkBox
			if (checkBox.IsChecked == true)
			{
				// при включенном чек-боксе меняет цвет текста на красный
				checkBox.Foreground = new SolidColorBrush(Colors.Red);
			}
			else
			{
				//очищаем TB и поляс данными	
				if(textBox1!=null)textBox1.Clear();	
				if(textBox2!=null)textBox2.Clear();	
				if(Sum1!=null)Sum1 = 0;				
				if(setter!=null) setter();// очишение переданного поля, забинденного с ТВ

				// при выключенном чек-боксе меняет цвет текста на черный
				checkBox.Foreground = new SolidColorBrush(Colors.Black);
			}
		}

		/// <summary>
		/// устанавливает цвет СВ и очищает все параметры пи отклюцении СВ
		/// </summary>
		public void CleanerMin(CheckBox checkBox, TextBox textBox1, Action setter)
		{
			//устанавливаем цвет в зависимости от активности checkBox
			if (checkBox.IsChecked == true)
			{
				// при включенном чек-боксе меняет цвет текста на красный
				checkBox.Foreground = new SolidColorBrush(Colors.Red);
			}
			else
			{
				//очищаем TB и поляс данными	
				textBox1.Clear();
				setter();

				// при выключенном чек-боксе меняет цвет текста на черный
				checkBox.Foreground = new SolidColorBrush(Colors.Black);
			}
		}

#endregion работа с визуалкой

		#region работа со свободным полем

		/// <summary>
        /// добавление свободного поля к стоимости заказа
        /// </summary>
        private void CB_SvP_Checked(object sender, RoutedEventArgs e)
        {
            //проверка на заполнение всех необходимых полей
            if (widthP > 0 && heightP > 0 && PloterWidthType != null && quantityP > 0)
            {
                //заполняем ТВ свп
                TB_freeFieldPrice.Text = Math.Round(freeFieldArea * PloterPeperType.FreeFieldPrice, 2).ToString() + " р";
            }

			CleanerMin(CB_SvP, TB_freeFieldPrice, () => freeFieldPrice = 0);
			
        }

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
        public void CalcfreeFieldArea()
        {
			if (widthP > 1580 && heightP > 1580) { freeFieldArea = 0; return; }
			if (widthP > PloterWidthType.WidthRoll && heightP > PloterWidthType.WidthRoll) { freeFieldArea = 0; return; }

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
            if (square > 0 && square < square2 || square2 <= 0) freeFieldArea = Math.Round(square, 2);

            else if (square2 > 0 && square > square2 || square <= 0) freeFieldArea = Math.Round(square2, 2);

            else if (square2 > 0 && square2 == square) freeFieldArea = Math.Round(square, 2);

            else freeFieldArea = 0;
        }

#endregion работа со свободным полем
	}
}
