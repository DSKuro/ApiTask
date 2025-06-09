using ApiTask.ViewModels;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Eremex.AvaloniaUI.Controls.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ApiTask.Views
{
    public partial class MainWindow : Window
    {
        private ClosableViewModel? ViewModel;

        public MainWindow()
        {
            InitializeComponent();

        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            if (this.DataContext is MainWindowViewModel model)
            {
                ViewModel = model;
                this.Opened += OnOpenedForm;
                model.DataGridChanged += OnGridChanged;
            }
            else
            {
                this.Close();
                Environment.Exit(1);
            }
        }

        private void OnGridChanged(object? sender, EventArgs e)
        {
            //dataGrid.RefreshData();
        }

        private void OnOpenedForm(object sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ClosingRequest += (sender, e) => this.Close();
            }
        }
    }
    public static class DebugConverters
    {
        public static readonly IValueConverter TypeConverter = new FuncValueConverter<object, string>(
            x => x?.GetType().FullName ?? "NULL");

        public static readonly IValueConverter PropertiesConverter = new FuncValueConverter<object, IEnumerable<string>>(
            x => x?.GetType().GetProperties().Select(p => $"{p.Name} ({p.PropertyType.Name})") ?? Enumerable.Empty<string>());
    }
}