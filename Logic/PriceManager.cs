using Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public class PriceManager
    {
        private const string pricesPath = "prices.json";
        private static List<Price> _prices;
        /// <summary>
        /// записывает цены в прайс
        /// </summary>
        /// <returns></returns>
        public static List<Price> GetPrices()
        {
            // Если цен нет
            if (_prices == null)
            {
                // Читаем из файла
                _prices = Saver.Load<List<Price>>(pricesPath);
                // Если ничего не получили, берем стандартные
                if (_prices == null || !_prices.Any())
                    _prices = DefaultPrices;
            }

            return _prices;
        }

        private static List<Price> DefaultPrices => new List<Price>
            {
                new Price
                {
                    NamePaper = "Офсетная 80 гр",
                    Price_4_0 = new List<decimal> { 0, 36, 30, 26.5m, 23.3m, 18.2m },
                    Price_4_4 = new List<decimal> { 0, 72, 55, 48, 40, 32.2m },

                    Price_4_0_Diler = new List<decimal> { 0, 14.9m, 14.9m, 12.4m, 11.2m, 9.9m },
                    Price_4_4_Diler = new List<decimal> { 0, 27.4m, 27.4m, 22.8m, 18.2m, 16.9m },
                },
                new Price
                {
                    NamePaper = "Меловка 105 гр",
                    Price_4_0 = new List<decimal> { 0, 40, 30, 27.5m, 23.6m, 18.6m },
                    Price_4_4 = new List<decimal> { 0, 76, 56, 49, 40.5m, 33.5m },

                    Price_4_0_Diler = new List<decimal> { 0, 20.46m, 15.95m, 13.31m, 12.1m, 10.67m },
                    Price_4_4_Diler = new List<decimal> { 0, 35.2m, 26.73m, 22.22m, 17.82m, 16.5m },
                },
                new Price
                {
                    NamePaper = "Меловка 300 гр",
                    Price_4_0 = new List<decimal> { 0, 48, 37, 33.6m, 30, 24.8m },
                    Price_4_4 = new List<decimal> { 0, 83, 64, 55, 47, 41.5m },

                    Price_4_0_Diler = new List<decimal> { 0, 27.28m, 23.32m, 19.36m, 17.49m, 15.51m },
                    Price_4_4_Diler = new List<decimal> { 0, 46.8m, 37.2m, 30.96m, 24.72m, 22.92m },
                },

                new Price
                {
                    NamePaper = "Самоклейка",
                    Price_4_0 = new List<decimal> { 0, 60, 50, 47, 42, 38 },
                    Price_4_4 = new List<decimal> { 0, 0, 0, 0, 0, 0 },

                    Price_4_0_Diler = new List<decimal> { 0, 41.80m, 33.66m, 25.96m, 25.41m, 22.55m },
                    Price_4_4_Diler = new List<decimal> { 0, 0, 0, 0, 0, 0 },
                }
            };
    }
}
