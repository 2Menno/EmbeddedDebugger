﻿using EmbeddedDebugger.DebugProtocol.Enums;
using EmbeddedDebugger.DebugProtocol.RegisterValues;
using EmbeddedDebugger.Model;
using EmbeddedDebugger.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace EmbeddedDebugger.View.UserControls.ObjectDisplayers
{
    /// <summary>
    /// Interaction logic for RegisterDisplayerUserControl.xaml
    /// </summary>
    public partial class RegisterDisplayerUserControl : UserControl
    {
        private SystemViewModel systemViewModel;
        private RegisterValue currentValue;
        private PlottingViewModel plottingViewModel;

        public static readonly DependencyProperty RegisterProperty = DependencyProperty.Register(
            "Register",
            typeof(Register),
            typeof(RegisterDisplayerUserControl),
            new FrameworkPropertyMetadata(null)
            );
        public static readonly DependencyProperty CollapsedProperty = DependencyProperty.Register(
            "Collapsed",
            typeof(bool),
            typeof(RegisterDisplayerUserControl),
            new FrameworkPropertyMetadata(null)
            );

        public bool Collapsed
        {
            get => (bool)this.GetValue(CollapsedProperty);
            set => this.SetValue(CollapsedProperty, value);
        }

        public Register Register
        {
            get => (Register)this.GetValue(RegisterProperty);
            set => this.SetValue(RegisterProperty, value);
        }

        public RegisterDisplayerUserControl()
        {
            this.InitializeComponent();
        }

        private void PlotCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            this.plottingViewModel.RegistersToPlot.Add(this.Register);
        }

        private void PlotCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.plottingViewModel.RegistersToPlot.Remove(this.Register);
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.systemViewModel.RequestNewValue(this.Register);
        }

        private void CollapseButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Collapsed = !this.Collapsed;
            if (this.Register.HasChildren && this.ChildrenStackPanel.Children.Count == 0)
            {
                foreach (Register r in this.Register.ChildRegisters)
                {
                    this.ChildrenStackPanel.Children.Add(new RegisterDisplayerUserControl() { Register = r });
                }
            }
        }

        public void Refresh(bool force = false)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (this.Register != null && (this.Register.RegisterValue != this.currentValue || force && this.Register.RegisterValue != null))
                {
                    this.currentValue = this.Register.RegisterValue;
                    this.ValueTextBox.Text = this.Register.RegisterValue.ValueAsFormattedString((ValueDisplayFormat)this.ValueDisplayFormatComboBox.SelectedItem);
                }
            });
        }

        public void Update(object o, EventArgs e)
        {
            this.Refresh();
        }

        private void RegisterDisplayerUserControl_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ViewModelManager vmmOld)
            {
                vmmOld.RefreshMedium -= this.Update;
            }
            if (e.NewValue is ViewModelManager vmm)
            {
                this.systemViewModel = vmm.SystemViewModel;
                this.plottingViewModel = vmm.PlottingViewModel;
                vmm.RefreshMedium += this.Update;
            }
        }

        public static IEnumerable<KnownColor> LineColors
        {
            get => new List<KnownColor>()
                {
                    KnownColor.Black,
                    KnownColor.Blue,
                    KnownColor.BlueViolet,
                    KnownColor.Brown,
                    KnownColor.CadetBlue,
                    KnownColor.Chartreuse,
                    KnownColor.Chocolate,
                    KnownColor.CornflowerBlue,
                    KnownColor.Crimson,
                    KnownColor.DarkBlue,
                    KnownColor.DarkGreen,
                    KnownColor.DarkMagenta,
                    KnownColor.DarkRed,
                    KnownColor.DarkViolet,
                    KnownColor.ForestGreen,
                    KnownColor.Green,
                    KnownColor.Indigo,
                    KnownColor.MediumBlue,
                    KnownColor.Navy,
                    KnownColor.Purple,
                    KnownColor.Red,
                };
        }

        private void ValueDisplayFormatComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Refresh(true);
        }

        private void ChannelModeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.systemViewModel?.UpdateChannelMode(this.Register, (ChannelMode)this.ChannelModeComboBox.SelectedItem);
        }
    }
}
