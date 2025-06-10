using ApiTask.ViewModels;
using Eremex.AvaloniaUI.Controls.Common;
using System;

namespace ApiTask.Views
{
    public partial class MainWindow : MxWindow
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
                InitializeModels(model);
            }
            else
            {
                this.Close();
                Environment.Exit(1);
            }
        }

        private void InitializeModels(MainWindowViewModel model)
        {
            ViewModel = model;
            this.Opened += OnOpenedForm;
            mainGrid.DataContext = model;
            mainGrid.PropertyChanged += model.OnSelectionPropertyChanged;
        }

        private void OnOpenedForm(object? sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.ClosingRequest += (sender, e) => this.Close();
            }
        }
    }
}