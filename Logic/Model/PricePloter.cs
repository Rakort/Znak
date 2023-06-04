using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    public class PricePloter
    {
        /// <summary>
        /// название материала
        /// </summary>
        public string NamePloter { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// цена свободного поля
        /// </summary>
        public decimal FreeFieldPrice { get; set; }

        public decimal PriceDiller { get; set; }
    }
}
