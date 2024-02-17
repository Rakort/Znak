using Logic;
using Logic.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Znak.Common;

namespace Znak.ViewModel
{
    public class EditWidthPlotterRollViewModel : INotifyPropertyChanged
    {
        public EditWidthPlotterRollViewModel()
        {
            PriceList = new ObservableCollection<WidthPlotterRoll>(PriceManager.GetWidthPlot());
        }

        /// <summary>
		/// Выбранный материал
		/// </summary>
        public WidthPlotterRoll CurrentWidthPlotterPrice { get; set; }

        /// <summary>
        /// Редактируемый материал
        /// </summary>
        public WidthPlotterRoll EditWidthPlotterPrice { get; set; }

        /// <summary>
        /// Прайс лист
        /// </summary>
        public ObservableCollection<WidthPlotterRoll> PriceList { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnCurrentWidthPlotterPriceChanged()
        {
            if (CurrentWidthPlotterPrice != null)
                EditWidthPlotterPrice = (WidthPlotterRoll)CurrentWidthPlotterPrice.Clone();
        }

        /// <summary>
        /// Сохранение цен
        /// </summary>
        public ICommand SaveCommand => new SimpleCommand(() =>
        {
            if (CurrentWidthPlotterPrice != null)
            {
                var index = PriceList.IndexOf(CurrentWidthPlotterPrice);
                PriceList.Remove(CurrentWidthPlotterPrice);
                PriceList.Insert(index, EditWidthPlotterPrice);
            }
            else
            {
                PriceList.Add(EditWidthPlotterPrice);
            }
            CurrentWidthPlotterPrice = EditWidthPlotterPrice;
            PriceManager.Save(PriceList);
        }, () => EditWidthPlotterPrice != null && !string.IsNullOrWhiteSpace(EditWidthPlotterPrice.Name) && EditWidthPlotterPrice.WidthRoll > 0);


        /// <summary>
        /// Добавление материала
        /// </summary>
        public ICommand AddCommand => new SimpleCommand(() =>
        {
            var newItem = new WidthPlotterRoll
            {
                Name = "введите название и цены, затем нажмите сохранить",
                WidthRoll = 0
            };

            CurrentWidthPlotterPrice = null;
            EditWidthPlotterPrice = newItem;
        });

        /// <summary>
        /// Удаление материала
        /// </summary>	
        public ICommand DeleteCommand => new SimpleCommand(() =>
        {
            PriceList.Remove(CurrentWidthPlotterPrice);
            PriceManager.Save(PriceList);
            CurrentWidthPlotterPrice = null;
            EditWidthPlotterPrice = null;
        }, () => CurrentWidthPlotterPrice != null);
    }
}
