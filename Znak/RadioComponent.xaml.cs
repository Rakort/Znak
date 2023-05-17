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
        
        public CollectionView ItemSource
        {
            get { return (CollectionView)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemSourceProperty =
            DependencyProperty.Register("ItemSource", typeof(CollectionView), typeof(RadioComponent), 
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
                new FrameworkPropertyMetadata(null)
                {
                    BindsTwoWayByDefault = true,
                });

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

            if (ItemSource == null)
                return;

            // Создаем колонки в гриде пока их меньше чем элементов в списке
            while(ItemSource.Count > MainGrid.ColumnDefinitions.Count())
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            int i = 0;
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
                Grid.SetColumn(rb, i);
                MainGrid.Children.Add(rb);
                i++;
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
                    // Активируем событие
                    Checked?.Invoke(sender, e);
                }
            }
        }
    }
}
