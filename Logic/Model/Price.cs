using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    public class Price
    {
        public string NamePaper { get; set; }
        public List<decimal> Price_4_0 { get; set; }
        public List<decimal> Price_4_4 { get; set; }
        public List<decimal> Price_4_0_Diler { get; set; }
        public List<decimal> Price_4_4_Diler { get; set; }

    }
}
