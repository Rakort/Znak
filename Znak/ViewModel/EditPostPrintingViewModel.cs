using Logic.Model;
using Logic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Znak.Common;

namespace Znak.ViewModel
{
    public class EditPostPrintingViewModel : INotifyPropertyChanged
    {
        public EditPostPrintingViewModel()
        {
            PriceList = new ObservableCollection<PostPrintingPrice>(PriceManager.GetPostPrintingPrice());
            CurrentPostPrintingPrice = PriceList.FirstOrDefault();
        }
        /// <summary>
		/// Выбранный материал
		/// </summary>
        public PostPrintingPrice CurrentPostPrintingPrice { get; set; }
        /// <summary>
        /// Редактируемый материал
        /// </summary>
        public PostPrintingPrice EditPostPrintingPrice { get; set; }
        /// <summary>
        /// Прайс лист
        /// </summary>
        public ObservableCollection<PostPrintingPrice> PriceList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Изменение выбранного материала
        /// </summary>
        public void OnCurrentPostPrintingPriceChanged()
        {
            if (CurrentPostPrintingPrice != null)
                EditPostPrintingPrice = (PostPrintingPrice)CurrentPostPrintingPrice.Clone();
        }

        /// <summary>
        /// Сохранение цен
        /// </summary>
        public ICommand SaveCommand => new SimpleCommand(() =>
        {
            if (CurrentPostPrintingPrice != null)
            {
                var index = PriceList.IndexOf(CurrentPostPrintingPrice);
                PriceList.Remove(CurrentPostPrintingPrice);
                PriceList.Insert(index, EditPostPrintingPrice);
            }
            else
            {
                PriceList.Add(EditPostPrintingPrice);
            }
            CurrentPostPrintingPrice = EditPostPrintingPrice;
            PriceManager.Save(PriceList);
        });

        /// <summary>
        /// Удаление материала
        /// </summary>
        public ICommand DeleteCommand => new SimpleCommand(() =>
        {
            PriceList.Remove(CurrentPostPrintingPrice);
            PriceManager.Save(PriceList);
        }, () => CurrentPostPrintingPrice != null);
    }
}
