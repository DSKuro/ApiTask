using ApiTask.Models;
using ApiTask.Services;
using ApiTask.Services.Dialogues;
using ApiTask.Services.Exceptions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public int SelectedRowIndex { get; set; }

        [ObservableProperty]
        ObservableCollection<SelectedMaterial> selectedMaterials =
            new ObservableCollection<SelectedMaterial>(new List<SelectedMaterial>());

        [ObservableProperty]
        string url;

        [ObservableProperty]
        ObservableCollection<string> details = new ObservableCollection<string>();

        [ObservableProperty]
        ObservableCollection<City> cities = new ObservableCollection<City>();

        [ObservableProperty]
        public ObservableCollection<CheckBox> checkBoxes = new ObservableCollection<CheckBox>();

        public MainWindowViewModel()
        {
            Cities.Add(new City("test1"));
            Cities.Add(new City("test2"));
            Cities.Add(new City("test3"));

            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());
            CheckBoxes.Add(new CheckBox());


        }

        public async Task OnSortingButtonClick()
        {
            await DialogueFormHelper.ShowFormAsDialogue(this, new SortingTreeWindow() { DataContext = this }); 
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
            await ApplyToken();
            await OpenDbConnection();
        }

        private async Task OpenDbConnection()
        {
            try
            {
                await DbConnection.OpenConnection(ReadConfiguration.GetValueByKeyFromConfiguration(DatabaseKey));
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
