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
	/// <summary>
	/// Логика взаимодействия для WindowPloter.xaml
	/// </summary>
	public partial class EditWidthPlotterRollView : UserControl, INotifyPropertyChanged
    {
        public EditWidthPlotterRollView()
        {
             InitializeComponent();
             DataContext = new EditWidthPlotterRollViewModel();
        }
    }
}
