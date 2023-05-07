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
        /// <summary>
        /// путь расположния Json
        /// </summary>
        private const string pricesPath = "prices.json";
        /// <summary>
        /// список цен
        /// </summary>
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
        // пример создания параметров цены бумаги
        private static List<Price> DefaultPrices => new List<Price>
            {
                new Price
                {
                    NamePaper = "Офсетная 80 гр",
                    Price_4_0 = new List<decimal> { 0, 36, 30, 26.5m, 23.3m, 18.2m },
                    Price_4_4 = new List<decimal> { 0, 72, 55, 48, 40, 32.2m },

                    Price_4_0_Diler = new List<decimal> { 0, 14.9m, 14.9m, 12.4m, 11.2m, 9.9m },
                    Price_4_4_Diler = new List<decimal> { 0, 27.4m, 27.4m, 22.8m, 18.2m, 16.9m },
                }             
            };
    }
}
