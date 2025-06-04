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
            if (this.DataContext is ClosableViewModel model)
            {
                ViewModel = model;
                this.Opened += OnOpenedForm;
            }
            else
            {
                this.Close();
                Environment.Exit(1);
            }
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