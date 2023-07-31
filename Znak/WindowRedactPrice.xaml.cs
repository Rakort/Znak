using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Logic;
using Logic.Model;

namespace Znak
{
	public partial class WindowRedactPrice : UserControl, INotifyPropertyChanged
    {
        public WindowRedactPrice()
        {
            InitializeComponent();        
            PriceList = PriceManager.GetPrices(PriceManager.pricesPath);		 
            DataContext = this;			
        }	
		/// <summary>
		/// Выбранный пользователем мотериал в СВ
		/// </summary>
        public Price PriceSelectedItem { get; set; }

		/// <summary>
        /// прайс лист
        /// </summary>
        public List<Price> PriceList { get; set; }

		/// <summary>
		/// заполнение ТВ цен лазерки и имени материала
		/// </summary>
		private void CB_Materials_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{   

				if (CB_Materials.SelectedIndex == -1) return;

				//цикл по всем элементам грида очищение ТВ
				foreach (var control in LaserRedactPrice_Grid.Children)    
				{ 
				    if (control is TextBox textBox) textBox.Clear();
			    }

         foreach (var data in PriceSelectedItem.Price_4_0) 
			{			
				string name = "TB_4_0_";
       
				name += PriceSelectedItem.Price_4_0.IndexOf(data).ToString();

				TB_Add(name, data.ToString(), LaserRedactPrice_Grid);
			}

		foreach (var data in PriceSelectedItem.Price_4_4) 
			{			
				string name = "TB_4_4_";
       
				name += PriceSelectedItem.Price_4_4.IndexOf(data).ToString();

				TB_Add(name, data.ToString(), LaserRedactPrice_Grid);
			}

			//Ззаполнение ТВ именем материала
			TB_NameMat.Text = PriceSelectedItem.NamePaper;
		}

		/// <summary>
		/// запись данных в Тб
		/// </summary>
		public void TB_Add(string tb, string data, Grid grid_name) 
		{ 
               //цикл по всем элементам грида
				foreach (var control in grid_name.Children)    
				{ 
				    if (control is TextBox textBox)
				       {
                         if (textBox.Name == tb)
							{
                              textBox.Text = data;
							}
                       } 											
				}
        }

		/// <summary>
		/// сохранение цен
		/// </summary>
		private void But_Price_Click(object sender, RoutedEventArgs e)
		{
			decimal parsedValue;

            // запись в цен из ТВ List Price_4_0
			if (Decimal.TryParse(TB_4_0_1.Text, out parsedValue)) PriceSelectedItem.Price_4_0[1] = parsedValue;
			if (Decimal.TryParse(TB_4_0_2.Text, out parsedValue)) PriceSelectedItem.Price_4_0[2] = parsedValue;
			if (Decimal.TryParse(TB_4_0_3.Text, out parsedValue)) PriceSelectedItem.Price_4_0[3] = parsedValue;
			if (Decimal.TryParse(TB_4_0_4.Text, out parsedValue)) PriceSelectedItem.Price_4_0[4] = parsedValue;
			if (Decimal.TryParse(TB_4_0_5.Text, out parsedValue)) PriceSelectedItem.Price_4_0[5] = parsedValue;
			if (Decimal.TryParse(TB_4_0_6.Text, out parsedValue)) PriceSelectedItem.Price_4_0[6] = parsedValue;

		    // запись в цен из ТВ List Price_4_4
			if (Decimal.TryParse(TB_4_4_1.Text, out parsedValue)) PriceSelectedItem.Price_4_4[1] = parsedValue;
			if (Decimal.TryParse(TB_4_4_2.Text, out parsedValue)) PriceSelectedItem.Price_4_4[2] = parsedValue;
			if (Decimal.TryParse(TB_4_4_3.Text, out parsedValue)) PriceSelectedItem.Price_4_4[3] = parsedValue;
			if (Decimal.TryParse(TB_4_4_4.Text, out parsedValue)) PriceSelectedItem.Price_4_4[4] = parsedValue;
			if (Decimal.TryParse(TB_4_4_5.Text, out parsedValue)) PriceSelectedItem.Price_4_4[5] = parsedValue;
			if (Decimal.TryParse(TB_4_4_6.Text, out parsedValue)) PriceSelectedItem.Price_4_4[6] = parsedValue;

			
           
           int index = PriceList.FindIndex(p => p.NamePaper == PriceSelectedItem.NamePaper);

		  if (PriceSelectedItem.NamePaper != null) { PriceSelectedItem.NamePaper = TB_NameMat.Text;}

			PriceList[index] = PriceSelectedItem; 

			Logic.Saver.Save(PriceManager.pricesPath,PriceList);
          
				//цикл по всем элементам грида очищение ТВ
				foreach (var control in LaserRedactPrice_Grid.Children)    
				{ 
				    if (control is TextBox textBox) textBox.Clear();
	
			    }
		 //обнуляем СВ с материалами
         CB_Materials.SelectedIndex = -1;

		 // перезапиь СВ         
		 CB_Materials.DisplayMemberPath = null;
         CB_Materials.DisplayMemberPath = "NamePaper";
					
		}

		/// <summary>
		/// добавление материала
		/// </summary>
		private void But_Price_Copy_Click(object sender, RoutedEventArgs e)
		{ 
               //обнуляем СВ с материалами
               CB_Materials.SelectedIndex = -1;

			   //цикл по всем элементам грида очищение ТВ
				foreach (var control in LaserRedactPrice_Grid.Children) {  if (control is TextBox textBox) textBox.Clear();}

			PriceSelectedItem = new Price
			{
				NamePaper = "введите название и цены, затем нажмите сохранить",
				Price_4_0 = new List<decimal> {0,0,0,0,0,0,0},
				Price_4_4 = new List<decimal> {0,0,0,0,0,0,0},
			};

			PriceList.Add(PriceSelectedItem);

			CB_Materials.SelectedIndex = PriceList.Count - 1;
		}

	    //удаление материала
		private void But_Price_Delete_Click(object sender, RoutedEventArgs e)
		{
			int index = PriceList.FindIndex(p => p.NamePaper == PriceSelectedItem.NamePaper);
			PriceList.RemoveAt(index);
			Logic.Saver.Save(PriceManager.pricesPath,PriceList);

	    //цикл по всем элементам грида очищение ТВ
		foreach (var control in LaserRedactPrice_Grid.Children)    
			{ 
			    if (control is TextBox textBox) textBox.Clear();
	
			}
		//обнуляем СВ с материалами
        CB_Materials.SelectedIndex = -1;

		 // перезапиь СВ         
		 CB_Materials.DisplayMemberPath = null;
         CB_Materials.DisplayMemberPath = "NamePaper";
		}
	}
}
