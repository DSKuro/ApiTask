using ApiTask.Models;
using ApiTask.Services.Messages;
using ApiTask.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using Eremex.AvaloniaUI.Controls.Common;
using Eremex.AvaloniaUI.Controls.TreeList;
using System;
using System.Collections;

namespace ApiTask.Views
{
    public partial class MainWindow : MxWindow
    {
        private MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            RegisterMessages();
        }

        private void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<MainWindow, TreeDialogueMessage>(this,
                (w, m) => ViewModel.RegisterOpenSortingWindow(w, m));
            WeakReferenceMessenger.Default.Register<MainWindow, MainModelEnableButtonsMessage>(this,
                (w, m) => EnableButtons());
        }

        private void EnableButtons()
        {
            btnOpenFile.IsEnabled = true;
            btnSort.IsEnabled = true;
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

    public class MainTreeListChildrenSelector : ITreeListChildrenSelector
    {
        public bool HasChildren(object item)
        {
            if (item is CodeCategory category)
            {
                return category.Codes.Count > 0;
            }
            return false;
        }

        public IEnumerable? SelectChildren(object item)
        {
            if (item is CodeCategory category)
            {
                return category.Codes;
            }
            return null;
        }
    }
}