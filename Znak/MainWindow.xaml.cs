using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Logic;
using Logic.Model;

namespace Znak
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            // загружает прайс листы
            PriceList = PriceManager.GetPrices(PriceManager.pricesPath);
            LaminationPrice = PriceManager.GetLaminationPrice(PriceManager.LaminationPath);
            PostPechPrice = PriceManager.GetPostPechPrice(PriceManager.PostPechLacerPath);

            // строчка для работы биндингов
            DataContext = this;

            // значения по умолчанию для переменных количества услуг постпечати
            BigovkaQuantity = 1;
            FalcovkaQuantity = 1;
            SkruglenieQuantity = 4;
            NumeraciaQuantity = 1;
            PerforaciaQuantity = 1;
            LuversyQuantity = 1;
            DirokolQuantity = 1;
            SteplerQuantity = 2;
        }

        #region Variables

        /// <summary>
        /// наличие блидов
        /// </summary>	
        public bool? bleeds = null;

        /// <summary>
        /// цветность
        /// </summary>	
        public bool? color = null;

        /// <summary>
        /// сторонность печати
        /// </summary>
        public bool sidePrint;

        /// <summary>
        /// тираж      
        /// </summary>
        public int tirag;

        /// <summary>
        /// цена за лист    
        /// </summary>
        /// 
        public decimal priceSheet;

        /// <summary>
        /// цена
        /// </summary>
        public decimal price = 0;

        /// <summary>
        /// обьект класса прайс выбранный в ComboBox
        /// </summary>
        public Price priceClass;

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
        /// тип ламинации выбранный пользователем в ComboBox
        /// </summary>
        public LaminationPrice LaminationType { get; set; }

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

        /// <summary>
        /// прайс ламинации
        /// </summary>
        public List<LaminationPrice> LaminationPrice { get; set; }

        public event PropertyChangedEventHandler PropertyChanged; //???????????

        #endregion variables

        #region  Variables пост печати

        /// <summary>
        /// прайс лист постпечати
        /// </summary>
        public List<PostPechPrice> PostPechPrice { get; set; }

        /// <summary>
        /// количество бигов
        /// </summary>
        public int BigovkaQuantity { get; set; }

        /// <summary>
        /// количество фальцев
        /// </summary>
        public int FalcovkaQuantity { get; set; }

        /// <summary>
        /// количество скруглений углов на 1 изделие
        /// </summary>
        public int SkruglenieQuantity { get; set; }

        /// <summary>
        /// количество скруглений номеров на 1 изделие
        /// </summary>
        public int NumeraciaQuantity { get; set; }

        /// <summary>
        /// количество перфораций на 1 изделие
        /// </summary>
        public int PerforaciaQuantity { get; set; }

        /// <summary>
        /// количество люверсов на 1 изделие
        /// </summary>
        public int LuversyQuantity { get; set; }

        /// <summary>
        /// количество отверстий на 1 изделие
        /// </summary>
        public int DirokolQuantity { get; set; }

        /// <summary>
        /// количество скрепок на 1 изделие
        /// </summary>
        public int SteplerQuantity { get; set; }

        /// <summary>
        /// сумма всей постпечатки
        /// </summary>

        public decimal SUmmPostPechPrice = 0;

        #endregion

        #region работа с ценами

        /// <summary>
        /// получение списка цен
        /// </summary>  
        public List<decimal> GetList(Price priceClass)
        {
            var list = new List<decimal>();

            if (sidePrint == true) list = priceClass.Price_4_4;

            else if (sidePrint == false) list = priceClass.Price_4_0;

            return list;
        }

        /// <summary>
        /// главный метод расчета цены за 1 лист
        /// </summary>
        public decimal MainPrice(Price priceClass, int tirag, bool a4)
        {
            decimal list = 1; // цена за 1 лист
            int i = 0; //номер элемента массива

            //проверка стоимости печати относительно тиража
            if (tirag > 0 && tirag < 5) i = 1;
            else if (tirag >= 5 && tirag < 20) i = 2;
            else if (tirag >= 20 && tirag < 50) i = 3;
            else if (tirag >= 50 && tirag < 100) i = 4;
            else if (tirag >= 100 && tirag < 200) i = 5;
            else if (tirag >= 200) i = 6;

            //проверка а4 лист или увеличеный
            if (a4) list = GetList(priceClass)[i] / 2;
            else list = GetList(priceClass)[i];

            //если печать чернобелая цена в 2 раза ниже
            if (color == false) list = list / 2;

            return list;
        }

        /// <summary>
        /// метод расчета общей стоимости тиража
        /// </summary>
        private void But_Price_Click(object sender, RoutedEventArgs e)
        {
            // проверка указано ли количество листов
            if (SheetsCount < 1) { MessageBox messageBox = new("укажите количество листов"); messageBox.ShowDialog(); return; }
            //проверка выбрана ли цветность и тип бумаги
            if ((RB_4_0.IsChecked == false && RB_4_4.IsChecked == false && RB_1_0.IsChecked == false && RB_1_1.IsChecked == false)
                || PaperType is null)
            { MessageBox messageBox = new("не выбрана цветность или тип бумаги"); messageBox.ShowDialog(); return; }
            //проверка выбран ли формат бумаги
            if ((RB_PaperFormatA4.IsChecked == false && RB_PaperFormatA3.IsChecked == false && RB_PaperFormatSRA3.IsChecked == false && RB_PaperFormat_325X470.IsChecked == false && RB_PaperFormat_330X485.IsChecked == false)
                || PaperType is null)
            { MessageBox messageBox = new("не выбран формат бумаги"); messageBox.ShowDialog(); return; }

            //устанавливаем значение цветности и сторонности в зависимости от активных radioButton
            color = (RB_4_0.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);
            sidePrint = (RB_1_1.IsChecked ?? false) || (RB_4_4.IsChecked ?? false);

            // цена за лист
            decimal priceInList = MainPrice(PaperType, SheetsCount, a4);

            if (priceInList == 0 && (RB_4_4.IsChecked == true || RB_1_1.IsChecked == true))
            { MessageBox messageBox = new("не выбран формат бумаги"); messageBox.ShowDialog(); return; }

            PostPechPriceCalc();
            Laminat();

            TB_price_tirag.Text = Math.Round(priceInList * SheetsCount + SUmmPostPechPrice, 1).ToString() + " p";

            TB_price_per_sheet.Text = priceInList.ToString() + " p";

            //обнуляем сумму постпечатки
            SUmmPostPechPrice = 0;
        }

        /// <summary>
        /// расчет стоимости ламинации
        /// </summary>
        public void Laminat()
        {
            if (LaminationType != null)
            {
                // проверка на допустимость формата пакетной ламинации
                if (LaminationType.Measure.ToLower().Contains("пакетный") &&
                (RB_ProductFormat_A5.IsChecked == true || RB_ProductFormat_A4.IsChecked == true || RB_ProductFormat_A3.IsChecked == true))
                {
                    //проверка, списывается ламинат тоько того формата изделия который выбран
                    if ((LaminationType.Measure.Contains("А5") && RB_ProductFormat_A5.IsChecked == true) ||
                        (LaminationType.Measure.Contains("А4") && RB_ProductFormat_A4.IsChecked == true) ||
                        (LaminationType.Measure.Contains("А3") && RB_ProductFormat_A3.IsChecked == true))

                        SUmmPostPechPrice += product * LaminationType.LamPrice;
                    else { MessageBox messageBox = new("формат изделия и ламината не соответствуют"); messageBox.ShowDialog(); return; }
                }
                else if (LaminationType.Measure.ToLower().Contains("рулонный"))

                    SUmmPostPechPrice += SheetsCount * LaminationType.LamPrice;
                else { MessageBox messageBox = new("выберите формат изделия из стандартных"); messageBox.ShowDialog(); return; }
            }
        }

        #endregion работа с ценами

        #region расчеты

        /// <summary>
        /// определение формата изделия
        /// </summary>
        public FormatPaper ProductFormat()
        {
            FormatPaper formatPaper = new FormatPaper(WidthProducts, HeightProducts);

            return formatPaper;
        }

        /// <summary>
        /// количество изделий на листе
        /// </summary>
        public int QuantityProducts(FormatPaper fromatPaper, FormatPaper fromatProduct)
        {
            if (fromatPaper.Width == 0 || fromatPaper.Height == 0 || fromatProduct.Width == 0 || fromatProduct.Height == 0)
                return 0;

            int x1 = fromatPaper.Width / fromatProduct.Width;
            int x2 = fromatPaper.Height / fromatProduct.Height;

            int y1 = fromatPaper.Width / fromatProduct.Height;
            int y2 = fromatPaper.Height / fromatProduct.Width;

            int x = x1 * x2;
            int y = y1 * y2;

            return Math.Max(x, y);
        }

        /// <summary>
        /// считает всю активную постпечатку
        /// </summary>
        public void PostPechPriceCalc()
        {
            //список всех позиций постпечати
            Dictionary<string, string> measures = new()
            { {"биговка", "BigovkaQuantity"}, {"фальцовка", "FalcovkaQuantity"}, {"скругление", "SkruglenieQuantity"},
              {"нумерация", "NumeraciaQuantity"}, {"перфорация", "PerforaciaQuantity"}, {"люверсы маленькикие", "LuversyQuantity"},
              {"дырокол", "DirokolQuantity"}, {"степлирование", "SteplerQuantity"} };

            foreach (KeyValuePair<string, string> measure in measures)
            {
                if (IsChecked(measure.Key))
                {
                    if (product < 1) { MessageBox messageBox = new("Выбрана постпечать. Укажите количество изделий!"); messageBox.ShowDialog(); return; }

                    PostPechPrice _postPechPrice = GetPostPechPriceByMeasure(measure.Key); // находим цену

                    int quantity = GetQuantityByPropertyName(measure.Value); // находим колличество

                    SUmmPostPechPrice += (quantity * _postPechPrice.PostPech_Price) * product; //прибавляем стоиомть услуги к сумме всей постпечатки
                }
            }
            // определяем активность CB
            bool IsChecked(string measure)
            {
                switch (measure)
                {
                    case "биговка": return CB_Bigovka.IsChecked == true;
                    case "фальцовка": return CB_Falcovka.IsChecked == true;
                    case "скругление": return CB_Skruglenie.IsChecked == true;
                    case "нумерация": return CB_Numeracia.IsChecked == true;
                    case "перфорация": return CB_Perforacia.IsChecked == true;
                    case "люверсы маленькикие": return CB_Luversy.IsChecked == true;
                    case "дырокол": return CB_Dirokol.IsChecked == true;
                    case "степлирование": return CB_Stepler.IsChecked == true;
                    default: return false;
                }
            }

            //вытаскиваем цену
            PostPechPrice GetPostPechPriceByMeasure(string measure) { return PostPechPrice.Where(x => x.Measure.Contains(measure)).FirstOrDefault(); }
            //вытаскиваем колличество
            int GetQuantityByPropertyName(string propertyName) { PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName); return (int)propertyInfo.GetValue(this); }
        }

        /// <summary>
        /// расчет тиража от количества изделий и количества изделий от тиража
        /// </summary>
        private void TB_Products_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tirag();
        }

        /// <summary>
        ///  расчет тиража от количества изделий 
        /// </summary>
        private void Tirag()
        {
            try
            {
                SheetsCount = (int)Math.Ceiling((double)product / (double)quantityOnSheet);

                if (SheetsCount <= 0)
                    SheetsCount = 0;
                else LB_Star_Sheets.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора типа бумаги
            }
            catch (Exception) { }
        }

        /// <summary>
        /// определение формата бумаги отностиельно активных radioButton
        /// </summary>
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

            int Shets = SheetsCount; // сохрянем количество листов

            QuantityOnSheet();
            Tirag();

            //обнуление поля с количеством изделий
            product = 0;
            SheetsCount = Shets; // востанавливаем количество листов
        }

        #endregion расчеты

        #region работа с визуалкой

        /// <summary>
        /// заполнение полей высоты и ширины изделия отностиельно активных radioButton
        /// </summary> 
        private void RB_ProductFormat_Checked(object sender, RoutedEventArgs e)
        {
            CB_bleeds.IsChecked = false; // выключаем блиды после каждого изменения формата изделия

            if (RB_ProductFormat_Vizitka.IsChecked == true) { WidthProducts = 50; HeightProducts = 90; }
            if (RB_ProductFormat_A7.IsChecked == true) { WidthProducts = 74; HeightProducts = 105; }
            if (RB_ProductFormat_A6.IsChecked == true) { WidthProducts = 105; HeightProducts = 148; }
            if (RB_ProductFormat_A5.IsChecked == true) { WidthProducts = 148; HeightProducts = 210; }
            if (RB_ProductFormat_A4.IsChecked == true) { WidthProducts = 210; HeightProducts = 297; }
            if (RB_ProductFormat_A3.IsChecked == true) { WidthProducts = 420; HeightProducts = 297; }

            int Shets = SheetsCount; // сохрянем количество листов

            QuantityOnSheet();
            Tirag();

            //обнуление поля с количеством изделий
            product = 0;

            SheetsCount = Shets; // востанавливаем количество листов
        }

        /// <summary>
        /// заполнение TextBox количество на листе
        /// </summary>
        private void QuantityOnSheet()
        {
            // заполнение TextBox количество на листе
            quantityOnSheet = QuantityProducts(formatPaper, ProductFormat());
        }

        /// <summary>
        /// заполнение TextBox количество на листе при ручном изменении формата изделия
        /// </summary>
        private void TB_height_width_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuantityOnSheet();
        }

        /// <summary>
        /// прибавление блидов к изделию относительно активности CB
        /// </summary>
        private void CB_bleeds_Checked(object sender, RoutedEventArgs e)
        {
            //устанавливаем значение блидов в зависимости от активности checkBox
            bleeds = CB_bleeds.IsChecked ?? false;

            //устанавливаем значение блидов в зависимости от активности checkBox
            if (bleeds == true)
            {
                WidthProducts += 4; HeightProducts += 4;

                // при включенном чек-боксе меняет цвет текста на красный
                CB_bleeds.Foreground = new SolidColorBrush(Colors.Red);
            }
            else if (bleeds == false)
            {
                WidthProducts -= 4; HeightProducts -= 4;
                // при выключенном чек-боксе меняет цвет текста на черный
                CB_bleeds.Foreground = new SolidColorBrush(Colors.Black);
            }
            Tirag();
        }

        /// <summary>
        /// событие постановки курсора в TB_height сбрасывает диллерскую галочку при каждом изменении параметров изделия
        /// </summary>
        private void TB_height_GotFocus(object sender, RoutedEventArgs e)
        {
            CB_bleeds.IsChecked = false;
        }

        /// <summary>
        /// Сброс всех данных
        /// </summary>
        private void But_Reset_Click(object sender, RoutedEventArgs e)
        {
            //цикл по всем элементам грида
            foreach (var control in GR_Laser.Children)
            {
                if (control is RadioButton radioButton)
                { radioButton.IsChecked = false; }         //выключаем все RadioButton
                else if (control is CheckBox checkBox)
                { checkBox.IsChecked = false; }            //выключаем все чекбоксы
                else if (control is ComboBox comboBox)
                { comboBox.SelectedIndex = -1; }           //очищаем все комбо-боксы
                else if (control is Label label)
                { label.Visibility = Visibility.Visible; } //возвращаем видимость звездочкам обязательных для заполнения полей
            }

            //обнуляем поля с данными и текстбоксы
            product = 0;
            quantityOnSheet = 0;
            WidthProducts = 0;
            HeightProducts = 0;
            SheetsCount = 0;
            TB_price_per_sheet.Clear();
            TB_price_tirag.Clear();

        }

        /// <summary>
        /// очищение комбобокс с ламинацией
        /// </summary>
        private void But_Reset_Lam_Click(object sender, RoutedEventArgs e)
        {
            // очищение комбобокс с ламинацией
            CB_Lamination.SelectedIndex = -1;
        }

        /// <summary>
        /// сканирует все СВ и делает их красными при активности
        /// </summary>
        private void CB_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var control in GR_Laser.Children)
            {
                if (control is CheckBox)
                {
                    CheckBox checkBox = (CheckBox)control;

                    if (checkBox.IsChecked == true)
                        checkBox.Foreground = new SolidColorBrush(Colors.Red);
                    else
                        checkBox.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        /// <summary>
        /// отключение видимости звездочки после постановки курсора в поле количества листов
        /// </summary>
        private void TB_Sheets_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SheetsCount > 0)
                LB_Star_Sheets.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// отключение видимости звездочки после типа бумаги
        /// </summary>
        private void CB_Materials_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LB_Star_Paper.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора типа бумаги

        }

        /// <summary>
        /// отключение видимости звездочки после выбора цвета печати
        /// </summary>
        private void RB_Color_Checked(object sender, RoutedEventArgs e)
        {
            LB_Star_Color.Visibility = Visibility.Collapsed; // отключение видимости звездочки после выбора цвета печати
        }

        #endregion работа с визуалкой

        public record FormatPaperSource(FormatPaper FormatPaper, string Name);
        public CollectionView FormatPaperItemSource => new CollectionView(new List<FormatPaperSource>()
        {
            new FormatPaperSource(new FormatPaper(200, 287), "A4"),
            new FormatPaperSource(new FormatPaper(410, 287), "A3"),
            new FormatPaperSource(new FormatPaper(440, 310), "SRA3"),
            new FormatPaperSource(new FormatPaper(315, 460), "325X470"),
            new FormatPaperSource(new FormatPaper(320, 475), "330X485"),
        });
        public FormatPaperSource CurrentFormatPaperSource { get; set; }
    }
}
