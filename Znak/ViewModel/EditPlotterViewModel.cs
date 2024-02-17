using Logic;
using Logic.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Znak.Common;

namespace Znak.ViewModel
{
    public class EditPlotterViewModel : INotifyPropertyChanged
    {
        public EditPlotterViewModel()
        {
            PriceList = new ObservableCollection<PlotterPrice>(PriceManager.GetPricesPlot());
        }

        /// <summary>
		/// Выбранный материал
		/// </summary>
        public PlotterPrice CurrentPlotterPrice { get; set; }

        /// <summary>
        /// Редактируемый материал
        /// </summary>
        public PlotterPrice EditPlotterPrice { get; set; }

        /// <summary>
        /// Прайс лист
        /// </summary>
        public ObservableCollection<PlotterPrice> PriceList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnCurrentPlotterPriceChanged()
        {
            if (CurrentPlotterPrice != null)
                EditPlotterPrice = (PlotterPrice)CurrentPlotterPrice.Clone();
        }

        /// <summary>
        /// Сохранение цен
        /// </summary>
        public ICommand SaveCommand => new SimpleCommand(() =>
        {
            if (CurrentPlotterPrice != null)
            {
                var index = PriceList.IndexOf(CurrentPlotterPrice);
                PriceList.Remove(CurrentPlotterPrice);
                PriceList.Insert(index, EditPlotterPrice);
            }
            else
            {
                PriceList.Add(EditPlotterPrice);
            }
            CurrentPlotterPrice = EditPlotterPrice;
            PriceManager.Save(PriceList);
        }, () => EditPlotterPrice != null && !string.IsNullOrWhiteSpace(EditPlotterPrice.Name) && EditPlotterPrice.Price > 0 && EditPlotterPrice.FreeFieldPrice > 0);


        /// <summary>
        /// Добавление материала
        /// </summary>
        public ICommand AddCommand => new SimpleCommand(() =>
        {
            var newItem = new PlotterPrice
            {
                Name = "введите название и цены, затем нажмите сохранить",
                Price = 0,
                FreeFieldPrice = 0,
            };

            CurrentPlotterPrice = null;
            EditPlotterPrice = newItem;
        });

        /// <summary>
        /// Удаление материала
        /// </summary>	
        public ICommand DeleteCommand => new SimpleCommand(() =>
        {
            PriceList.Remove(CurrentPlotterPrice);
            PriceManager.Save(PriceList);
            CurrentPlotterPrice = null;
            EditPlotterPrice = null;
        }, () => CurrentPlotterPrice != null);
    }
}
