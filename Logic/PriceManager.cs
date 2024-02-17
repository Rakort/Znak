using Logic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Logic.Model.LaserPrice;

namespace Logic
{
    public class PriceManager
    {
        /// <summary>
        /// путь расположния Json
        /// </summary>
		
        public const string LaserPrice = "Price/Laser_Price.json";

        public const string LaminatePath = "Price/Lamination_Price.json";

        public const string PricesPlotPath = "Price/Ploter_Price.json";

        public const string WidthPlotPath = "Price/WidthPloter_Price.json";

        public const string PostPechLacerPath = "Price/PostPech_Price.json";

        /// <summary>
        /// список цен
        /// </summary>
		 
        private static List<LaserPrice> _prices;

        /// <summary>
        /// список цен ламинации
        /// </summary>
		
        private static List<LaminatePrice> _laminationPrice;

        /// <summary>
        /// список цен плотерной печати
        /// </summary>
		 
        private static List<PlotterPrice> _pricePloter;

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
        public static List<LaserPrice> GetLaserPrices()
        {
            // Читаем из файла
            var _prices = Saver.Load<List<LaserPrice>>(PriceManager.LaserPrice);
            // Если ничего не получили, берем стандартные
            if (!_prices.Any())
                _prices = DefaultPrices;
            
            return _prices;
        }

        /// <summary>
        /// Читаем цены лазерной постпечати
        /// </summary>
        /// <returns></returns>
        public static List<PostPechPrice> GetPostPechPrice(string path)
        {
            return Saver.Load<List<PostPechPrice>>(path);
        }


        /// <summary>
        /// Читаем цены ламинации
        /// </summary>
        /// <returns></returns>
        public static List<LaminatePrice> GetLaminatePrice()
        {
            return Saver.Load<List<LaminatePrice>>(PriceManager.LaminatePath);
        }

        /// <summary>
        /// Читаем цены в прайс плотерной печати
        /// </summary>
        /// <returns></returns>
        public static List<PlotterPrice> GetPricesPlot()
        {
            return Saver.Load<List<PlotterPrice>>(PriceManager.PricesPlotPath);
        }

        /// <summary>
        /// Читаем размеры рулонов
        /// </summary>
        /// <returns></returns>
        public static List<WidthPloterRoll> GetWidthPlot(string path)
        {
            return Saver.Load<List<WidthPloterRoll>>(path);
        }

        public static void Save(IEnumerable<LaminatePrice> data)
        {
            Saver.Save(PriceManager.LaminatePath, data);
        }

        public static void Save(IEnumerable<LaserPrice> data)
        {
            Saver.Save(PriceManager.LaserPrice, data);
        }
        public static void Save(IEnumerable<PlotterPrice> data)
        {
            Saver.Save(PriceManager.PricesPlotPath, data);
        }

        #region примеры создания

        // пример создания параметров цены бумаги лазерной печати
        private static List<LaserPrice> DefaultPrices => new List<LaserPrice>
            {
                new LaserPrice
                {
                    Name = "Офсетная 80 гр",
                    Prices = new List<LaserPriceItem> {
                        new LaserPriceItem(1,4,36,72),
                        new LaserPriceItem(5,19,30,55),
                        new LaserPriceItem(20,49,26.5m,48),
                        new LaserPriceItem(50,99,23.3m,40),
                        new LaserPriceItem(100,19,918.2m,32.2m ),
                        new LaserPriceItem(200,0,18.2m,32.2m)
        }
                }
            };

        // пример создания параметров цены плотера
        public static List<PlotterPrice> DefaultPricesPloter => new List<PlotterPrice>
            {
                new PlotterPrice
                {
                    Name = "Банер",
                    Price = 400,
                    FreeFieldPrice = 100
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
#endregion примеры создания
    }
}
