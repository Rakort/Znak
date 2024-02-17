using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    public class PlotterPrice : ICloneable
    {
        /// <summary>
        /// название материала
        /// </summary>
        public string Name { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// цена свободного поля
        /// </summary>
        public decimal FreeFieldPrice { get; set; }

        public object Clone()
        {
            return new PlotterPrice
            {
                Name = Name,
                Price = Price,
                FreeFieldPrice = FreeFieldPrice
            };
        }
    }
}
