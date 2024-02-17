using Logic.Model;
using Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Znak.Common;

namespace Znak.ViewModel
{
    public class EditLaminateViewModel : INotifyPropertyChanged
    {
        public EditLaminateViewModel()
        {
            PriceList = new ObservableCollection<LaminatePrice>(PriceManager.GetLaminatePrice());
        }

        /// <summary>
        /// Выбранный пользователем материал
        /// </summary>
        public LaminatePrice CurrentLaminatePrice { get; set; }
        public LaminatePrice EditLaminatePrice { get; set; }

        /// <summary>
        /// Прайс лист
        /// </summary>
        public ObservableCollection<LaminatePrice> PriceList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void OnCurrentLaminatePriceChanged()
        {
            if (CurrentLaminatePrice != null)
                EditLaminatePrice = new LaminatePrice() { LamPrice = CurrentLaminatePrice.LamPrice, Measure = CurrentLaminatePrice.Measure };
        }

        /// <summary>
        /// Сохранение цен
        /// </summary>
        public ICommand SaveCommand => new SimpleCommand(() =>
        {
            if (CurrentLaminatePrice != null)
            {
                var index = PriceList.IndexOf(CurrentLaminatePrice);
                PriceList.Remove(CurrentLaminatePrice);
                PriceList.Insert(index, EditLaminatePrice);
            }
            else
            {
                PriceList.Add(EditLaminatePrice);
            }
            CurrentLaminatePrice = EditLaminatePrice;
            PriceManager.Save(PriceList);
        }, () => EditLaminatePrice != null && !string.IsNullOrWhiteSpace(EditLaminatePrice.Measure) && EditLaminatePrice.LamPrice > 0);


        /// <summary>
        /// Добавление материала
        /// </summary>
        public ICommand AddCommand => new SimpleCommand(() =>
        {
            var newItem = new LaminatePrice
            {
                Measure = "Введите название и цены, затем нажмите сохранить",
                LamPrice = 0
            };

            CurrentLaminatePrice = null;
            EditLaminatePrice = newItem;
        });

        /// <summary>
        /// Удаление материала
        /// </summary>	
        public ICommand DeleteCommand => new SimpleCommand(() =>
        {
            PriceList.Remove(CurrentLaminatePrice);
            PriceManager.Save(PriceList);
            CurrentLaminatePrice = null;
            EditLaminatePrice = null;
        }, () => CurrentLaminatePrice != null);
    }
}
