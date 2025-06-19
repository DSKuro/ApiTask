using ApiTask.Models;
using ApiTask.Services;
using ApiTask.Services.Dialogues;
using ApiTask.Services.Exceptions;
using ApiTask.Services.Messages;
using Avalonia;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Eremex.AvaloniaUI.Controls.DataGrid;
using Microsoft.Data.SqlClient;
using System;  
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTask.ViewModels
{
    public partial class MainWindowViewModel : ClosableViewModel
    {
        private static readonly string FileDialogueTitle = "Выберите файл:";
        private static readonly string Explorer = "explorer.exe";
        private static readonly string SiteError = "Ошибка при открытии страницы товара";
        private static readonly string DatabaseKey = "db";
        private static readonly string KeyMessage = "Неправильно задан ключ";
        private static readonly string CodesSqlKey = "codes";
        private static readonly string ParametersSqlKey = "parameters";

        public int SelectedRowIndex { get; set; }

        public SortingTreeMemento SortingTreeState { get; private set; }

        [ObservableProperty]
        ObservableCollection<SelectedMaterial> selectedMaterials =
            new ObservableCollection<SelectedMaterial>(new List<SelectedMaterial>());

        [ObservableProperty]
        string url;

        [ObservableProperty]
        ObservableCollection<string> details = new ObservableCollection<string>();

        [ObservableProperty]
        ObservableCollection<City> cities = new ObservableCollection<City>();

        public async Task OnSortingButtonClick()
        {
            var sortingTree = await WeakReferenceMessenger.Default.Send(new TreeDialogueMessage());
        }

        public void OnSelectionPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == DataGridControl.FocusedRowIndexProperty && e.OldValue != e.NewValue
                && (int) e.NewValue >= 0)
            {
                ClearDetails();
                SetNewDetails(SelectedMaterials[SelectedRowIndex].Attributes,
                   SelectedMaterials[SelectedRowIndex].ThumbnailUrl);
            }
        }

        private void SetNewDetails(List<string> attributes, string imageUrl)
        {
            foreach (string attribute in attributes)
            {
                Details.Add(attribute);
            }
            Url = imageUrl;
        }

        [RelayCommand]
        public async Task OnTransitionButtonClicked()
        {
            await TransitionButtonClickedImpl();
        }

        private async Task TransitionButtonClickedImpl()
        {
            try
            {
                Process.Start(Explorer, SelectedMaterials[SelectedRowIndex].Url);
            }
            catch (Exception ex) {
                await MessageBoxHelper(SiteError, ErrorCallback);
            }
        }

        [RelayCommand]
        private async Task OpenFile()
        {
            await GetMaterialData();
        }

        private async Task GetMaterialData()
        {
            try
            {
                await GetMaterialDataAuxiliary();
            }
            catch (HttpException ex)
            {
                await MessageBoxHelper(ex.Message, ErrorCallback);
            }
            catch (ArgumentNullException e)
            {
                await MessageBoxHelper("Неправильно задан ключ", ErrorCallback);
            }
            catch (IOException ex)
            {
                await MessageBoxHelper("Неправильный формат файла Excel", null);
            }
            catch (InvalidOperationException ex)
            {
                return;
            }
        }

        private async Task GetMaterialDataAuxiliary()
        {
            ClearDetails();
            SelectedMaterials.Clear();
            string path = await GetAbsolutePathFile();
            await MaterialsProcess.GetMaterialDataImpl(SelectedMaterials, path);
        }

        private void ClearDetails()
        {
            Details.Clear();
            Url = null;
        }

        private async Task<string?> GetAbsolutePathFile()
        {
            IEnumerable<IStorageFile?> selectFiles = await this.OpenFileDialogueAsync(FileDialogueTitle);
            string path = selectFiles.First().Path.AbsolutePath;
            return path;
        }

        [RelayCommand]
        private async Task CloseForm()
        {
            await DbConnection.CloseConnection();
        }

        [RelayCommand]
        private async Task OpenForm()
        {
            await OnOpenForm();
        }
        
        private async Task OnOpenForm()
        {
            await ApplyToken();
            await OpenDbConnection();
            //await GetCodes();
            await GetParameters();
        }

        private async Task GetParameters()
        {
            SortingTreeState = new SortingTreeMemento(await DbConnection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(ParametersSqlKey)));
        }

        private async Task GetCodes()
        {
            List<string> test = await DbConnection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(CodesSqlKey));
            foreach (string testitem in test)
            {
                Cities.Add(new City(testitem));
            }
        }

        private async Task OpenDbConnection()
        {
            try
            {
                await DbConnection.OpenConnection(ReadConfiguration.GetValueByKeyFromSecrets(DatabaseKey));
            }
            catch (SqlException e)
            {
                await MessageBoxHelper(e.Message, ErrorCallback);
            }
            catch (ArgumentNullException e)
            {
                await MessageBoxHelper(KeyMessage, ErrorCallback);
            }
        }

        private async Task ApplyToken()
        {
            try
            {
                await AccessTokenProcess.ApplyTokenImpl();
            }
            catch (HttpException ex)
            {
                await MessageBoxHelper(ex.Message, ErrorCallback);
            }
            catch (ArgumentNullException e)
            {
                await MessageBoxHelper(KeyMessage, ErrorCallback);
            }
        }

        private void ErrorCallback()
        {
            this.OnClosingRequest();
            Environment.Exit(1);
        }
    }

    public class City
    {
        public City(string name) { Name = name; }
        public string Name { get; set; }
    }

}
