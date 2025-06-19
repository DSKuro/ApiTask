using ApiTask.Models;
using ApiTask.Services;
using ApiTask.Services.Dialogues;
using ApiTask.Services.Exceptions;
using ApiTask.Services.Messages;
using ApiTask.Services.ViewModelSubServices;
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

        private List<string> CodesData = new List<string>();
        private List<List<string>> Params = new List<List<string>>();

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
        ObservableCollection<Codes> categories = new ObservableCollection<Codes>();

        public async Task OnSortingButtonClick()
        {
            await ProcessParameterChanging();
        }

        private async Task ProcessParameterChanging()
        {
            bool isChanged = await WeakReferenceMessenger.Default.Send(new TreeDialogueMessage());
            if (isChanged)
            {
                UpdateTreeView();
            }
        }

        private void UpdateTreeView()
        {
            Categories.Clear();
            UpdateCategories();
        }

        private void UpdateCategories()
        {
            (List<string> nonCategorized, List<string> allCategorized,
                List<List<string>> partialCategorized) = InitializeCategories();
            DefineCategories(nonCategorized, allCategorized, partialCategorized);
            UpdateCategoriesInBinding(nonCategorized, allCategorized, partialCategorized);
        }

        private (List<string>, List<string>, List<List<string>>) InitializeCategories()
        {
            List<string> nonCategorized = new List<string>();
            List<string> allCategorized = new List<string>();
            List<List<string>> partialCategorized = new List<List<string>>();
            return (nonCategorized, allCategorized, partialCategorized);
        }

        private void DefineCategories(List<string> nonCategorized, List<string> allCategorized,
            List<List<string>> partialCategorized)
        {
            int param = 0;
            for (int i = 0; i < Params.Count; i++)
            {
                for (int j = 0; j < SortingTreeState.EnabledParameters.Count; j++)
                {

                    if (Params[i].Contains(SortingTreeState.EnabledParameters[j]))
                    {
                        param++;
                    }
                }
                DefineCategoriesImpl(nonCategorized, allCategorized, partialCategorized, param, i);
                param = 0;
            }
        }

        private void DefineCategoriesImpl(List<string> nonCategorized, List<string> allCategorized,
            List<List<string>> partialCategorized, int param, int i)
        {
            if (param == 0)
            {
                nonCategorized.Add(CodesData[i]);
            }
            else if (param == SortingTreeState.EnabledParameters.Count)
            {
                allCategorized.Add(CodesData[i]);
            }
            else
            {
                for (int j = 0; j < param; j++)
                {
                    if (partialCategorized.Count < param)
                    {
                        partialCategorized.Add(new List<string>());
                    }
                    partialCategorized[j].Add(CodesData[i]);
                }

            }
        }

        private void UpdateCategoriesInBinding(List<string> nonCategorized, List<string> allCategorized,
            List<List<string>> partialCategorized)
        {
            if (nonCategorized.Count != 0)
            {
                CodeCategory nonCategory = new CodeCategory("Без категории");

                foreach (string code in nonCategorized)
                {
                    nonCategory.Codes.Add(new Codes(code));
                }
                Categories.Add(nonCategory);
            }

            if (partialCategorized.Count != 0)
            {

                for (int i = 0; i < partialCategorized.Count; i++)
                {
                    CodeCategory category = new CodeCategory($"Без параметра: {SortingTreeState.EnabledParameters[i]}");
                    foreach (string code in partialCategorized[i])
                    {
                        category.Codes.Add(new Codes(code));
                    }
                    Categories.Add(category);
                }
            }

            if (allCategorized.Count != 0)
            {
                CodeCategory allCategory = new CodeCategory("Все параметры");
                foreach (string code in allCategorized)
                {
                    allCategory.Codes.Add(new Codes(code));
                }
                Categories.Add(allCategory);
            }
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
            await OnOpenFormImpl();
        }

        private async Task OnOpenFormImpl()
        {
            await ApplyToken();
            await OpenDbConnection();
            await GetDataFromDb();
            SetCategories();
        }

        private async Task GetDataFromDb()
        {
            try
            {
                await GetDataFromDbImpl();
            }
            catch (SqlException e)
            {
                await MessageBoxHelper(e.Message, ErrorCallback);
            }
        }

        private async Task GetDataFromDbImpl()
        {
            ParseData(await DbDataProcess.GetCodes(CodesSqlKey));
            SortingTreeState = await DbDataProcess.GetParameters(ParametersSqlKey);
        }

        private void ParseData(List<List<string>> data)
        {
            for (int i = 0; i < data[0].Count; i++)
            {
                CodesData.Add(data[0][i]);
                Params.Add(data[1][i].Split(',').ToList<string>());
            }
        }

        private void SetCategories()
        {
            foreach (string code in  CodesData)
            {
                Categories.Add(new Codes(code));
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
}
