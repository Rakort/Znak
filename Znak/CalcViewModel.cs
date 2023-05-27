using Logic;
using Logic.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Znak
{
    public class CalcViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Свойства
        /// <summary>
        /// является ли клиент диллером
        /// </summary>
        public bool Diler { get; set; }
        /// <summary>
        /// наличие блидов
        /// </summary>
        public bool Bleeds { get; set; } 
        /// <summary>
        /// цветность
        /// </summary>
        public bool Color => CurrentColorPaperSource.Color;
        /// <summary>
        /// сторонность печати
        /// </summary>
        public bool SidePrint => CurrentColorPaperSource.SidePrint;
        /// <summary>
        /// колличество изделий в тираже
        /// </summary>
        public int Tirag { get; set; } 
        /// <summary>
        /// количество листов в тираже
        /// </summary>
        public int SheetsCount => QuantityOnSheet <= 0 ? 0 : Tirag / QuantityOnSheet;
        /// <summary>
        /// колличество изделий на листе
        /// </summary>
        public int QuantityOnSheet => QuantityProducts(CurrentFormatPaperSource?.FormatPaper, CurrentFormatProductSource?.FormatPaper);
        /// <summary>
        /// цена за лист    
        /// </summary>
        public decimal PriceSheet { get; set; }
        /// <summary>
        /// цена
        /// </summary>
        public decimal PriceTirag => PriceSheet * SheetsCount;
        /// <summary>
        /// обьект класса прайс выбранный в ComboBox
        /// </summary>
        public Price CurrentPrice { get; set; } 
        /// <summary>
        /// прайс лист
        /// </summary>
        public List<Price> PriceList { get; set; }

        public int CustomHeight { get; set; }
        public int CustomWidth { get; set; }

        public record FormatProductSource(FormatPaper FormatPaper, string Name);
        public FormatProductSource CurrentFormatProductSource { get; set; }
        public ListCollectionView FormatProductItemSource => new ListCollectionView(new List<FormatProductSource>()
        {
            new FormatProductSource(new FormatPaper(74, 105), "A7"),
            new FormatProductSource(new FormatPaper(105, 148), "A6"),
            new FormatProductSource(new FormatPaper(148, 210), "A5"),
            new FormatProductSource(new FormatPaper(210, 297), "A4"),
            new FormatProductSource(new FormatPaper(420, 297), "A3"),
        });

        public record FormatPaperSource(FormatPaper FormatPaper, string Name);
        public FormatPaperSource CurrentFormatPaperSource { get; set; }
        public ListCollectionView FormatPaperItemSource => new ListCollectionView(new List<FormatPaperSource>()
        {
            new FormatPaperSource(new FormatPaper(200, 287), "A4"),
            new FormatPaperSource(new FormatPaper(410, 287), "A3"),
            new FormatPaperSource(new FormatPaper(440, 310), "SRA3"),
            new FormatPaperSource(new FormatPaper(315, 460), "325X470"),
            new FormatPaperSource(new FormatPaper(320, 475), "330X485"),
        });

        public record ColorPaperSource(bool Color, bool SidePrint, string Name);
        public ColorPaperSource CurrentColorPaperSource { get; set; }
        public ListCollectionView ColorPaperItemSource => new ListCollectionView(new List<ColorPaperSource>()
        {
            new ColorPaperSource(true, false, "4+0"),
            new ColorPaperSource(true, true, "4+4"),
            new ColorPaperSource(false, false, "1+0"),
            new ColorPaperSource(false, true, "1+1"),
        });
        #endregion

        #region События изменения свойств

        public void OnCurrentFormatPaperSourceChanged()
        {
            Draw();
        }

        public void OnCurrentFormatProductSourceChanged()
        {
            CustomHeight = CurrentFormatProductSource.FormatPaper.Height;
            CustomWidth = CurrentFormatProductSource.FormatPaper.Width;
            Draw();
        }
        public void OnBleedsChanged(bool oldValue, bool newValue)
        {
            if (!oldValue && newValue)
            {
                CustomHeight += 4;
                CustomWidth += 4;
            }
            else if (oldValue && !newValue)
            {
                CustomHeight -= 4;
                CustomWidth -= 4;
            }
            Draw();
        }

        #endregion
        public CalcViewModel()
        {
            // загружает прайс лист
            PriceList = PriceManager.GetPrices();
        }

        /// <summary>
        /// получение списка цен
        /// </summary>
        /// <param name="priceClass"></param>
        /// <returns></returns>
        public List<decimal> GetList(Price priceClass)
        {
            var list = (SidePrint, Diler) switch
            {
                (true, false) => priceClass.Price_4_4,
                (false, false) => priceClass.Price_4_0,
                (true, true) => priceClass.Price_4_4_Diler,
                (false, true) => priceClass.Price_4_0_Diler
            };

            return list;
        }

        public ICommand CalcPrice => new SimpleCommand(() => MainPrice(),
            (_) => CurrentColorPaperSource != null && CurrentFormatPaperSource != null && CurrentFormatProductSource != null && CurrentPrice != null);

        /// <summary>
        /// главный метод расчета цены за 1 лист
        /// </summary>
        public void MainPrice()
        {
            //проверка стоимости печати относительно тиража
            int i = SheetsCount switch
            {
                > 0 and < 5 => 1,
                >= 5 and < 20 => 2,
                >= 20 and < 50 => 3,
                >= 50 and < 100 => 4,
                >= 100 => 5,
                _ => 0,
            };

            // цена за 1 лист
            decimal price = GetList(CurrentPrice)[i];
            //проверка а4 лист или увеличеный
            if (CurrentFormatPaperSource.FormatPaper == FormatPaper.A4)
                price = price / 2;

            //если печать чернобелая цена в 2 раза ниже
            if (!Color)
                price = price / 2;

            PriceSheet = price;
            Draw();
        }

        /// <summary>
        /// количество изделий на листе
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int QuantityProducts(FormatPaper formatPaper, FormatPaper formatProduct)
        {
            if (formatPaper == null || formatProduct == null || formatPaper.IsZero || formatProduct.IsZero)
                return 0;

            int x1 = formatPaper.Width / formatProduct.Width;
            int x2 = formatPaper.Height / formatProduct.Height;

            int y1 = formatPaper.Width / formatProduct.Height;
            int y2 = formatPaper.Height / formatProduct.Width;

            int x = x1 * x2;
            int y = y1 * y2;

            return Math.Max(x, y);
        }

        public record Rect(int X, int Y, int Width, int Height);

        public ObservableCollection<Rect> Lines { get; set; } = new ObservableCollection<Rect>();

        private void Draw()
        {
            var formatPaper = CurrentFormatPaperSource?.FormatPaper;
            var formatProduct = CurrentFormatProductSource?.FormatPaper;
            if (formatPaper == null || formatProduct == null) return;
            FlipFormatProduct();

            Lines.Clear();

            Lines.Add(new Rect(0,0,formatPaper.Width, formatPaper.Height));

            int x = 0, y = 0;
            while (y + formatProduct.Height < formatPaper.Height)
            {
                while (x + formatProduct.Width < formatPaper.Width)
                {
                    Lines.Add(new Rect(x, y, formatProduct.Width, formatProduct.Height));
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
    }
}
