using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
	public class LaminatePrice : INotifyPropertyChanged
	{
        /// <summary>
        /// цена
        /// </summary>
        public decimal LamPrice { get; set; }

        /// <summary>
        /// текст описывающий ламинацию
        /// </summary>
        public string Measure { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
