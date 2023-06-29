using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
	public class LaminationPrice
	{
        /// <summary>
        /// цена
        /// </summary>
        public decimal LamPrice { get; set; }

        /// <summary>
        /// теккст описывающий ламинацию
        /// </summary>
        public string Measure { get; set; }
	}
}
