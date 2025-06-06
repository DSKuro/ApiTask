using ApiTask.ViewModels;
using Avalonia.Controls;
using System;

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
            dataGrid.RefreshData();
        }

        private void OnOpenedForm(object sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ClosingRequest += (sender, e) => this.Close();
            }
        }
    }
}