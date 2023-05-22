using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Znak
{
    /// <summary>
    /// Логика взаимодействия для RadioComponent.xaml
    /// </summary>
    public partial class RadioComponent : UserControl, INotifyPropertyChanged
    {
        #region Свойства
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(RadioComponent), new PropertyMetadata(""));
        
        public ListCollectionView ItemSource
        {
            get { return (ListCollectionView)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(ListCollectionView), typeof(RadioComponent), 
                // Действие при изменении свойства
                new UIPropertyMetadata(null, (o, args) => {
                    if (o is RadioComponent current)
                        current.Draw();
                }));

        public object CurrentItem
        {
            get { return (object)GetValue(CurrentItemProperty); }
            set { SetValue(CurrentItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem", typeof(object), typeof(RadioComponent), 
                new FrameworkPropertyMetadata(null, (o, args) => {
                    if (o is RadioComponent current)
                    {
                        if (args.NewValue == args.OldValue)
                            return;

                        if (args.NewValue == null)
                            current._radioButtons.ForEach(x => x.IsChecked = false);
                        else
                        {
                            current._radioButtons.ForEach(x => x.IsChecked = false);
                            var rb = current._radioButtons.FirstOrDefault(x => x.Tag.Equals(args.NewValue));
                            if(rb != null)
                                rb.IsChecked = true;
                        }
                    }
                })
                {
                    BindsTwoWayByDefault = true,
                });



        public int MaxColumn
        {
            get { return (int)GetValue(MaxColumnProperty); }
            set { SetValue(MaxColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxColumnProperty =
            DependencyProperty.Register("MaxColumn", typeof(int), typeof(RadioComponent), new PropertyMetadata(0));


        private List<RadioButton> _radioButtons = new List<RadioButton>();

        #endregion

        /// <summary>
        /// Событие выбора RadioButton
        /// </summary>
        public event RoutedEventHandler Checked;

        public RadioComponent()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Создание RadioButton
        /// </summary>
        private void Draw()
        {
            // Очистка грида
            MainGrid.Children.Clear();
            _radioButtons.Clear();

            if (ItemSource == null)
                return;

            var maxColumn = MaxColumn <= 0 ? ItemSource.Count : Math.Min(ItemSource.Count, MaxColumn);

            if (ItemSource.Count > maxColumn)
            {
                var maxRow = (int)Math.Ceiling((decimal)ItemSource.Count / MaxColumn);
                while (maxRow > MainGrid.RowDefinitions.Count())
                    MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            // Создаем колонки в гриде пока их меньше чем элементов в списке
            while (maxColumn > MainGrid.ColumnDefinitions.Count())
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            int column = 0;
            int row = 0;
            foreach (var item in ItemSource)
            {
                // Создаем RadioButton
                var rb = new RadioButton
                {
                    Tag = item,
                    Content = item.GetType().GetProperty(DisplayMemberPath)?.GetValue(item),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,                    
                };
                // Подписываемся на событие выбора RadioButton
                rb.Checked += Rb_Checked;

                // Устанавливаем номер колонки
                Grid.SetColumn(rb, column);
                Grid.SetRow(rb, row);
                MainGrid.Children.Add(rb);
                _radioButtons.Add(rb);
                column++;
                if (column >= maxColumn)
                {
                    row++;
                    column = 0;
                }
                
            }
        }

        public override void EndInit()
        {
            base.EndInit();
            Draw();
        }

        private void Rb_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                if (radioButton.IsChecked == true)
                {
                    CurrentItem = radioButton.Tag;
                    ItemSource.MoveCurrentTo(CurrentItem);
                    // Активируем событие
                    Checked?.Invoke(sender, e);
                }
            }
        }
    }
}
