using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    public class PostPrintingPrice : ICloneable
	{
        /// <summary>
        /// цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// наименование
        /// </summary>
        public string Name { get; set; }

        public object Clone()
        {
            return new PostPrintingPrice { Name = Name, Price = Price };
        }
    }
}
