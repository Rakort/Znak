using Logic.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using static Znak.CalcViewModel;

namespace Znak
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window, INotifyPropertyChanged
    {
        public ObservableCollection<CalcDto> SavedCalc { get; set; } = new ObservableCollection<CalcDto>();
        public CalcDto CurrentCalc { get; set; }
        public bool IsSelectSave => CurrentCalc != null;

        private CalcViewModel _calcViewModel;
        public MainView()
        {
            InitializeComponent();
            _calcViewModel = new CalcViewModel();
            CalcView.DataContext = _calcViewModel;
            DataContext = this;
        }

        public void Load(CalcDto dto)
        {
            _calcViewModel.Bleeds = dto.Bleeds;
            _calcViewModel.CurrentPrice = dto.CurrentPrice;
            _calcViewModel.Diler = dto.Diler;
            _calcViewModel.PriceSheet = dto.PriceSheet;
            _calcViewModel.Tirag = dto.Tirag;
            _calcViewModel.CurrentColorPaperSource = dto.CurrentColorPaperSource;
            _calcViewModel.CurrentFormatPaperSource = dto.CurrentFormatPaperSource;
            _calcViewModel.CurrentFormatProductSource = dto.CurrentFormatProductSource;

            if (_calcViewModel.CalcPrice.CanExecute(null))
                _calcViewModel.CalcPrice.Execute(null);
        }

        private CalcDto Save(CalcViewModel viewModel)
        {
            return new CalcDto
            {
                Bleeds = viewModel.Bleeds,
                CurrentColorPaperSource = viewModel.CurrentColorPaperSource,
                CurrentFormatPaperSource = viewModel.CurrentFormatPaperSource,
                CurrentFormatProductSource = viewModel.CurrentFormatProductSource,
                CurrentPrice = viewModel.CurrentPrice,
                Diler = viewModel.Diler,
                PriceSheet = viewModel.PriceSheet,
                Tirag = viewModel.Tirag
            };
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SavedCalc.Add(Save(_calcViewModel));
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            Load(CurrentCalc);
        }
    }

    public class CalcDto
    {
        public bool Diler { get; set; }
        /// <summary>
        /// наличие блидов
        /// </summary>
        public bool Bleeds { get; set; }
        /// <summary>
        /// колличество изделий в тираже
        /// </summary>
        public int Tirag { get; set; }
        /// <summary>
        /// цена за лист    
        /// </summary>
        public decimal PriceSheet { get; set; }
        /// <summary>
        /// обьект класса прайс выбранный в ComboBox
        /// </summary>
        public Price CurrentPrice { get; set; }
        public FormatProductSource CurrentFormatProductSource { get; set; }
        public FormatPaperSource CurrentFormatPaperSource { get; set; }
        public ColorPaperSource CurrentColorPaperSource { get; set; }
    }
}
