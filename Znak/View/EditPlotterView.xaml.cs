using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Logic;
using Logic.Model;
using Znak.ViewModel;

namespace Znak.View
{
	public partial class EditPlotterView : UserControl, INotifyPropertyChanged
    {
        public EditPlotterView()
        {
             InitializeComponent();
             DataContext = new EditPlotterViewModel();
        }
    }
}
