using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Logic;
using Logic.Model;

namespace Znak
{
    /// <summary>
    /// Логика взаимодействия для WindowPloter.xaml
    /// </summary>
    public partial class WindowRedactLaminat : UserControl, INotifyPropertyChanged
    {
        public WindowRedactLaminat()
        {
             InitializeComponent();        
			 PriceList = PriceManager.GetLaminationPrice(PriceManager.LaminationPath);   
             DataContext = this;
        }

		/// <summary>
		/// Выбранный пользователем мотериал в СВ
		/// </summary>
        public LaminationPrice PriceSelectedItem { get; set; }

		/// <summary>
        /// прайс лист
        /// </summary>
        public List<LaminationPrice> PriceList { get; set; }

	    /// <summary>
		/// заполнение ТВ цен лазерки и имени материала
		/// </summary>
		private void CB_Materials_SelectionChangedLam(object sender, SelectionChangedEventArgs e)
		{   

			if (CB_Materials.SelectedIndex == -1) return;

			//цикл по всем элементам грида очищение ТВ
			foreach (var control in RedactLaminat_Grid.Children)    
			  { 
				 if (control is TextBox textBox) textBox.Clear();
			  }

		    TB_Price.Text = PriceSelectedItem.LamPrice.ToString();
         
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
			if (Decimal.TryParse(TB_Price.Text, out parsedValue)) PriceSelectedItem.LamPrice = parsedValue;
			
           int index = PriceList.FindIndex(p => p.Measure == PriceSelectedItem.Measure);

		   if (PriceSelectedItem.Measure != null) { PriceSelectedItem.Measure = TB_NameMat.Text;}

			PriceList[index] = PriceSelectedItem; 

			Logic.Saver.Save(PriceManager.LaminationPath,PriceList);
          
				//цикл по всем элементам грида очищение ТВ
				foreach (var control in RedactLaminat_Grid.Children)    
				{ 
				    if (control is TextBox textBox) textBox.Clear();
	
			    }
		 //обнуляем СВ с материалами
         CB_Materials.SelectedIndex = -1;

		 // перезапиь СВ         
		 CB_Materials.DisplayMemberPath = null;
         CB_Materials.DisplayMemberPath = "Measure";
					
		}

		/// <summary>
		/// добавление материала
		/// </summary>
		private void But_Price_Copy_Click(object sender, RoutedEventArgs e)
		{ 
               //обнуляем СВ с материалами
               CB_Materials.SelectedIndex = -1;

			   //цикл по всем элементам грида очищение ТВ
				foreach (var control in RedactLaminat_Grid.Children) {  if (control is TextBox textBox) textBox.Clear();}

			PriceSelectedItem = new LaminationPrice
			{
				Measure = "введите название и цены, затем нажмите сохранить",
				LamPrice = 0
			};

			PriceList.Add(PriceSelectedItem);

			CB_Materials.SelectedIndex = PriceList.Count - 1;
		}

	    //удаление материала
		private void But_Price_Delete_Click(object sender, RoutedEventArgs e)
		{
			int index = PriceList.FindIndex(p => p.Measure == PriceSelectedItem.Measure);
			PriceList.RemoveAt(index);
			Logic.Saver.Save(PriceManager.LaminationPath,PriceList);

	    //цикл по всем элементам грида очищение ТВ
		foreach (var control in RedactLaminat_Grid.Children)    
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
