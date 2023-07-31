using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Logic;
using Logic.Model;

namespace Znak
{
	public partial class WindowRedactPloter : UserControl, INotifyPropertyChanged
    {
        public WindowRedactPloter()
        {
             InitializeComponent();    
			 PriceList = PriceManager.GetPricesPlot(PriceManager.pricesPlotPath); 
             DataContext = this;
        }
		/// <summary>
		/// Выбранный пользователем мотериал в СВ
		/// </summary>
        public PricePloter PriceSelectedItem { get; set; }

		/// <summary>
        /// прайс лист
        /// </summary>
        public List<PricePloter> PriceList { get; set; }

	    /// <summary>
		/// заполнение ТВ цен лазерки и имени материала
		/// </summary>
		private void CB_Materials_SelectionChangedLam(object sender, SelectionChangedEventArgs e)
		{   

			if (CB_Materials.SelectedIndex == -1) return;

			//цикл по всем элементам грида очищение ТВ
			foreach (var control in WindowRedactPloter_Grid.Children)    
			  { 
				 if (control is TextBox textBox) textBox.Clear();
			  }

		    TB_Price.Text = PriceSelectedItem.Price.ToString();
			TB_PriceSVP.Text = PriceSelectedItem.FreeFieldPrice.ToString();
			//Заполнение ТВ именем материала
			TB_NameMat.Text = PriceSelectedItem.NamePloter;
		}

		/// <summary>
		/// сохранение цен
		/// </summary>
		private void But_Price_Click(object sender, RoutedEventArgs e)
		{
			decimal parsedValue;

            // запись в цен из ТВ List Price_4_0
			if (Decimal.TryParse(TB_Price.Text, out parsedValue)) PriceSelectedItem.Price = parsedValue;
			if (Decimal.TryParse(TB_PriceSVP.Text, out parsedValue)) PriceSelectedItem.FreeFieldPrice = parsedValue;
			
           int index = PriceList.FindIndex(p => p.NamePloter == PriceSelectedItem.NamePloter);

		   if (PriceSelectedItem.NamePloter != null) { PriceSelectedItem.NamePloter = TB_NameMat.Text;}

			PriceList[index] = PriceSelectedItem; 

			Logic.Saver.Save(PriceManager.pricesPlotPath,PriceList);
          
				//цикл по всем элементам грида очищение ТВ
				foreach (var control in WindowRedactPloter_Grid.Children)    
				{ 
				    if (control is TextBox textBox) textBox.Clear();
	
			    }
		 //обнуляем СВ с материалами
         CB_Materials.SelectedIndex = -1;

		 // перезапиь СВ         
		 CB_Materials.DisplayMemberPath = null;
         CB_Materials.DisplayMemberPath = "NamePloter";
					
		}

		/// <summary>
		/// добавление материала
		/// </summary>
		private void But_Price_Copy_Click(object sender, RoutedEventArgs e)
		{ 
               //обнуляем СВ с материалами
               CB_Materials.SelectedIndex = -1;

			   //цикл по всем элементам грида очищение ТВ
				foreach (var control in WindowRedactPloter_Grid.Children) {  if (control is TextBox textBox) textBox.Clear();}

			PriceSelectedItem = new PricePloter
			{
				NamePloter = "введите название и цены, затем нажмите сохранить",
				Price = 0,
				FreeFieldPrice = 0,
			};

			PriceList.Add(PriceSelectedItem);

			CB_Materials.SelectedIndex = PriceList.Count - 1;
		}

	    /// <summary>
		/// удаление материала
		/// </summary>
		private void But_Price_Delete_Click(object sender, RoutedEventArgs e)
		{
			int index = PriceList.FindIndex(p => p.NamePloter == PriceSelectedItem.NamePloter);
			PriceList.RemoveAt(index);
			Logic.Saver.Save(PriceManager.pricesPlotPath,PriceList);

	    //цикл по всем элементам грида очищение ТВ
		foreach (var control in WindowRedactPloter_Grid.Children)    
			{ 
			    if (control is TextBox textBox) textBox.Clear();
	
			}
		//обнуляем СВ с материалами
        CB_Materials.SelectedIndex = -1;

		 // перезапиь СВ         
		 CB_Materials.DisplayMemberPath = null;
         CB_Materials.DisplayMemberPath = "NamePloter";
		}
    }
}
