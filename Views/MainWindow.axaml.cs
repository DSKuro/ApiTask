using ApiTask.Services.Messages;
using ApiTask.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Eremex.AvaloniaUI.Controls.Common;
using System;
using System.Collections.Generic;

namespace ApiTask.Views
{
    public partial class MainWindow : MxWindow
    {
        private MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            RegisterMessage();
        }

        private void RegisterMessage()
        {
            WeakReferenceMessenger.Default.Register<MainWindow, TreeDialogueMessage>(this, (w, m) =>
            {
                SortingTreeWindow dialogue = new SortingTreeWindow
                {
                    DataContext = new SortingTreeWindowViewModel()
                };
                m.Reply(dialogue.ShowDialog<List<string>>(w));
            });
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
            mainGrid.DataContext = model;
            mainGrid.PropertyChanged += model.OnSelectionPropertyChanged;
        }
    }
}