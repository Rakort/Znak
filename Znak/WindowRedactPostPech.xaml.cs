using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Logic;
using Logic.Model;

namespace Znak
{
	public partial class WindowRedactPostPech : UserControl, INotifyPropertyChanged
    {
        public WindowRedactPostPech()
        {
             InitializeComponent();  
			 PriceList = PriceManager.GetPostPechPrice(PriceManager.PostPechLacerPath);   
             DataContext = this;
        }

		/// <summary>
		/// Выбранный пользователем мотериал в СВ
		/// </summary>
        public PostPechPrice PriceSelectedItem { get; set; }

		/// <summary>
        /// прайс лист
        /// </summary>
        public List<PostPechPrice> PriceList { get; set; }

	    /// <summary>
		/// заполнение ТВ цен лазерки и имени материала
		/// </summary>
		private void CB_Materials_SelectionChangedLam(object sender, SelectionChangedEventArgs e)
		{   

			if (CB_Materials.SelectedIndex == -1) return;

			//цикл по всем элементам грида очищение ТВ
			foreach (var control in RedactPostPech_Grid.Children)    
			  { 
				 if (control is TextBox textBox) textBox.Clear();
			  }

		    TB_Price.Text = PriceSelectedItem.PostPech_Price.ToString();
         
			//Заполнение ТВ именем материала
			TB_NameMat.Text = PriceSelectedItem.Measure;
		}

		/// <summary>
		/// сохранение цен
		/// </summary>
		private void But_Price_Click(object sender, RoutedEventArgs e)
		{
			decimal parsedValue;

            // запись в цен из ТВ List Price_4_0
			if (Decimal.TryParse(TB_Price.Text, out parsedValue)) PriceSelectedItem.PostPech_Price = parsedValue;
			
           int index = PriceList.FindIndex(p => p.Measure == PriceSelectedItem.Measure);

		   if (PriceSelectedItem.Measure != null) { PriceSelectedItem.Measure = TB_NameMat.Text;}

			PriceList[index] = PriceSelectedItem; 

			Logic.Saver.Save(PriceManager.PostPechLacerPath,PriceList);
          
				//цикл по всем элементам грида очищение ТВ
				foreach (var control in RedactPostPech_Grid.Children)    
				{ 
				    if (control is TextBox textBox) textBox.Clear();
	
			    }
		 //обнуляем СВ с материалами
         CB_Materials.SelectedIndex = -1;

		 // перезапиь СВ         
		 CB_Materials.DisplayMemberPath = null;
         CB_Materials.DisplayMemberPath = "Measure";
					
		}

    }
}
