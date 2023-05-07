using Logic.Model;

namespace Logic

{
    public class Calculations
    {
        public bool diler = false; //является ли клиент диллером
        public bool color; // цветность
        public bool sidePrint; // сторонность печати
        public int tirag; // тираж       
        public decimal priceSheet; // цена за лист    
        public decimal price = 0;// цена
        public Price priceClass;  // обьект класса прайс выбранный в ComboBox

        /// <summary>
        /// получение списка цен
        /// </summary>
        /// <param name="priceClass"></param>
        /// <returns></returns>
        public List<decimal> GetList(Price priceClass) 
        {
            var list = new List<decimal>();

            if (sidePrint == true && diler == false)
            {
                list = priceClass.Price_4_4;
            }
            else if(sidePrint == false && diler == false)
                list = priceClass.Price_4_0;


            if (sidePrint == true && diler == true)
            {
                list = priceClass.Price_4_4_Diler;
            }
            else if (sidePrint == false && diler == true)
                list = priceClass.Price_4_0_Diler;


            return list;
        }

        /// <summary>
        /// главный метод расчета цены за 1 лист
        /// </summary>
        /// <param name="priceClass"></param>
        /// <returns></returns>
        public decimal MainPrice(Price priceClass, int tirag,bool a4) 
        {
            decimal list =1;// цена за 1 лист
            int i = 0;//номер элемента массива

            //проверка стоимости печати относительно тиража
            if (tirag > 0 && tirag < 5) i = 1;
            else if (tirag >= 5 && tirag < 20) i = 2;
            else if (tirag >= 20 && tirag < 50) i = 3;
            else if (tirag >= 50 && tirag < 100) i = 4;
            else if (tirag >= 100) i = 5;

            //проверка а4 лист или увеличеный
            if (a4) list = GetList(priceClass)[i] / 2;
            else list = GetList(priceClass)[i];

            //если печать чернобелая цена в 2 раза ниже
            if (color == false) list = list / 2;

            return list;
        }

        /// <summary>
        /// количество изделий на листе
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public int QuantityProducts(FormatPaper fromatPaper, FormatPaper fromatProduct)
        {
            try
            {
                int x1 = fromatPaper.Width / fromatProduct.Width;
                int x2 = fromatPaper.Height / fromatProduct.Height;

                int y1 = fromatPaper.Width / fromatProduct.Height;
                int y2 = fromatPaper.Height / fromatProduct.Width;

                int x = x1 * x2;
                int y = y1 * y2;

                return (x < y) ? y : x;
            }
            catch (Exception e) { return 0; }
        }
    }
}
