using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Model
{
    public class LaserPrice : INotifyPropertyChanged, ICloneable
    {
        public string Name { get; set; }
        public List<LaserPriceItem> Prices { get; set; }

        public static List<LaserPriceItem> DefaultPrices => new List<LaserPriceItem> { 
            new LaserPriceItem(1,4),
            new LaserPriceItem(5,19),
            new LaserPriceItem(20,49),
            new LaserPriceItem(50,99),
            new LaserPriceItem(100,199),
            new LaserPriceItem(200,0)
        };

        public event PropertyChangedEventHandler PropertyChanged;

        public decimal GetPrice(int count, int side)
        {
            foreach (var item in Prices)
            {
                if (count <= item.To || item.To == 0)
                    return side == 1 ? item.ValueOneSide : item.ValueTwoSide;
            }
            throw new ArgumentOutOfRangeException();
        }

        public object Clone()
        {
            return new LaserPrice
            {
                Name = Name,
                Prices = Prices.Select(x => new LaserPriceItem(x.From, x.To, x.ValueOneSide, x.ValueTwoSide)).ToList()
            };
        }

        public class LaserPriceItem
        {
            public int From { get; set; }
            public int To { get; set; }
            public decimal ValueOneSide { get; set; }
            public decimal ValueTwoSide { get; set; }

            [JsonIgnore]
            public string Text => $"Цена тиража {valueText} листа";
            private string valueText => To == 0 ? $"{From}+" : $"{From}-{To}";

            public LaserPriceItem(int from, int to, decimal valueOneSide, decimal valueTwoSide)
            {
                From = from;
                To = to;
                ValueOneSide = valueOneSide;
                ValueTwoSide = valueTwoSide;
            }

            public LaserPriceItem(int from, int to)
            {
                From = from;
                To = to;
            }

            public LaserPriceItem()
            {
            }
        }
    }

}
