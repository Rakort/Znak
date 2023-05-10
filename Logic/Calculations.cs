using Logic.Model;
using System;
using System.ComponentModel;

namespace Logic

{
    public class Calculations : INotifyPropertyChanged
    {
        /// <summary>
        /// наличие блидов
        /// </summary>
        public bool diler { get; set; } //является ли клиент диллером
        public bool bleeds { get; set; } = false; // цветность
        public bool color; // цветность
        public bool sidePrint; // сторонность печати
        public int tirag; // тираж       
        public decimal priceSheet; // цена за лист    
        public decimal price = 0;// цена
        public Price priceClass { get; set; }  // обьект класса прайс выбранный в ComboBox

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsValid()
        {
            return priceClass != null;
        }

        /// <summary>
        /// получение списка цен
        /// </summary>
        /// <param name="priceClass"></param>
        /// <returns></returns>
        public List<decimal> GetList(Price priceClass) 
        {
            var list = (sidePrint, diler) switch
            {
                (true, false) => priceClass.Price_4_4,
                (false, false) => priceClass.Price_4_0,
                (true, true) => priceClass.Price_4_4_Diler,
                (false, true) => priceClass.Price_4_0_Diler
            };
            return list;
        }

        /// <summary>
        /// главный метод расчета цены за 1 лист
        /// </summary>
        /// <returns></returns>
        public decimal MainPrice(int tirag,bool a4) 
        {
            //проверка стоимости печати относительно тиража
            int i = tirag switch
            {
                > 0 and < 5 => 1,
                >= 5 and < 20 => 2,
                >= 20 and < 50 => 3,
                >= 50 and < 100 => 4,
                >= 100 => 5,
                _ => 0,
            };

            // цена за 1 лист
            decimal price = GetList(priceClass)[i];
            //проверка а4 лист или увеличеный
            if (a4) 
                price = price / 2;

            //если печать чернобелая цена в 2 раза ниже
            if (!color) 
                price = price / 2;

            return price;
        }

        /// <summary>
        /// количество изделий на листе
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int QuantityProducts(FormatPaper fromatPaper, FormatPaper fromatProduct)
        {
            if (fromatPaper.IsZero || fromatProduct.IsZero)
                return 0;

            int x1 = fromatPaper.Width / fromatProduct.Width;
            int x2 = fromatPaper.Height / fromatProduct.Height;

            int y1 = fromatPaper.Width / fromatProduct.Height;
            int y2 = fromatPaper.Height / fromatProduct.Width;

            int x = x1 * x2;
            int y = y1 * y2;

            return Math.Max(x, y);
        }

    }
}
