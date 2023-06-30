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
        public const string pricesPath = "Price/Lacer_Price.json";

        public const string LaminationPath = "Price/Lamination_Price.json";

        public const string pricesPlotPath = "Price/Ploter_Price.json";

        public const string WidthPlotPath = "Price/WidthPloter_Price.json";

        public const string PostPechLacerPath = "Price/PostPech_Price.json";

        /// <summary>
        /// список цен
        /// </summary>
        private static List<Price> _prices;

        /// <summary>
        /// список цен ламинации
        /// </summary>
		
        private static List<LaminationPrice> _laminationPrice;

        /// <summary>
        /// список цен плотерной печати
        /// </summary>
        private static List<PricePloter> _pricePloter;

        /// <summary>
        /// список размеров рулонов плотера
        /// </summary>
        private static List<WidthPloterRoll> _widthPloterRoll;

		/// <summary>
		/// список цен лазерной постпечати 
		/// </summary>
		public static List<PostPechPrice> _postPechPrice;

        /// <summary>
        /// записывает цены в прайс
        /// </summary>
        /// <returns></returns>
        public static List<Price> GetPrices(string path)
        {
            // Если цен нет
            if (_prices == null)
            {
                // Читаем из файла
                _prices = Saver.Load<List<Price>>(path);
                // Если ничего не получили, берем стандартные
                if (_prices == null || !_prices.Any())
                    _prices = DefaultPrices;
            }
            // так я первый раз создаю json с ценами
            //Saver.Save(pricesPlotPath, DefaultPricesPloter);
            //Saver.Save(WidthPlotPath, DefaultWidthPloterRoll);
            return _prices;
        }

        /// <summary>
        /// записывает цены лазерной постпечати
        /// </summary>
        /// <returns></returns>
        public static List<PostPechPrice> GetPostPechPrice(string path)
        {
            // Если цен нет
            if (_postPechPrice == null)
            {
                // Читаем из файла
                _postPechPrice = Saver.Load<List<PostPechPrice>>(path);
               
            }           
            return _postPechPrice;
        }


        /// <summary>
        /// записывает цены ламинации
        /// </summary>
        /// <returns></returns>
        public static List<LaminationPrice> GetLaminationPrice(string path)
        {
            // Если цен нет
            if (_laminationPrice == null)
            {
                // Читаем из файла
                _laminationPrice = Saver.Load<List<LaminationPrice>>(path);
               
            }           
            return _laminationPrice;
        }

        /// <summary>
        /// записывает цены в прайс плотерной печати
        /// </summary>
        /// <returns></returns>
        public static List<PricePloter> GetPricesPlot(string path)
        {
            // Если цен нет
            if (_pricePloter == null)
            {
                // Читаем из файла
                _pricePloter = Saver.Load<List<PricePloter>>(path);
               
            }           
            return _pricePloter;
        }

        /// <summary>
        /// записывает размеры рулонов
        /// </summary>
        /// <returns></returns>
        public static List<WidthPloterRoll> GetWidthPlot(string path)
        {
            // Если цен нет
            if (_widthPloterRoll == null)
            {
                // Читаем из файла
                _widthPloterRoll = Saver.Load<List<WidthPloterRoll>>(path);

            }

            return _widthPloterRoll;
        }

        // пример создания параметров цены бумаги лазерной печати
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

        // пример создания параметров цены плотера
        public static List<PricePloter> DefaultPricesPloter => new List<PricePloter>
            {
                new PricePloter
                {
                    NamePloter = "Банер",
                    Price = 400,
                    FreeFieldPrice = 100,
                    PriceDiller = 200,
                }             
            };

        // пример создания параметров ширины рулонов плотера
        public static List<WidthPloterRoll> DefaultWidthPloterRoll => new List<WidthPloterRoll>
            {
                new WidthPloterRoll
                {
                    WidthRoll = 1270,
                    Measure = "1.27 M. ",
                },
                new WidthPloterRoll
                {
                    WidthRoll = 1500,
                    Measure = "1.5 M. ",
                },
            };
    }
}
