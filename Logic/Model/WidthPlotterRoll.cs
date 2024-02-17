using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    public class WidthPlotterRoll : ICloneable
    {
        /// <summary>
        /// Ширина рулона
        /// </summary>
        public decimal WidthRoll { get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        public object Clone()
        {
            return new WidthPlotterRoll
            {
                Name = Name,
                WidthRoll = WidthRoll
            };
        }
    }
}
