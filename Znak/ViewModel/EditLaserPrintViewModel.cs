using Logic;
using Logic.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Znak.Common;

namespace Znak.ViewModel
{
    public class EditLaserPrintViewModel : INotifyPropertyChanged
    {
        public EditLaserPrintViewModel()
        {
            PriceList = new ObservableCollection<LaserPrice>(PriceManager.GetLaserPrices());
            CurrentLaserPrice = PriceList.FirstOrDefault();
        }
        /// <summary>
		/// Выбранный материал
		/// </summary>
        public LaserPrice CurrentLaserPrice { get; set; }
        /// <summary>
        /// Редактируемый материал
        /// </summary>
        public LaserPrice EditLaserPrice { get; set; }
        /// <summary>
        /// Прайс лист
        /// </summary>
        public ObservableCollection<LaserPrice> PriceList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Изменение выбранного материала
        /// </summary>
        public void OnCurrentLaserPriceChanged()
        {
            if (CurrentLaserPrice != null)
                EditLaserPrice = (LaserPrice)CurrentLaserPrice.Clone();
        }

        /// <summary>
        /// Сохранение цен
        /// </summary>
        public ICommand SaveCommand => new SimpleCommand(() =>
        {
            if (CurrentLaserPrice != null)
            {
                var index = PriceList.IndexOf(CurrentLaserPrice);
                PriceList.Remove(CurrentLaserPrice);
                PriceList.Insert(index, EditLaserPrice);
            }
            else
            {
                PriceList.Add(EditLaserPrice);
            }
            CurrentLaserPrice = EditLaserPrice;
            PriceManager.Save(PriceList);
        });

        /// <summary>
        /// Добавление материала
        /// </summary>
        public ICommand AddCommand => new SimpleCommand(() =>
        {
            //обнуляем СВ с материалами
            CurrentLaserPrice = null;

            EditLaserPrice = new LaserPrice
            {
                Name = "введите название и цены, затем нажмите сохранить",
                Prices = LaserPrice.DefaultPrices
            };
        });

        /// <summary>
        /// Удаление материала
        /// </summary>
        public ICommand DeleteCommand => new SimpleCommand(() =>
        {
            PriceList.Remove(CurrentLaserPrice);
            PriceManager.Save(PriceList);
        }, () => CurrentLaserPrice != null);
    }
}
