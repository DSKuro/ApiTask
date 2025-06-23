using ApiTask.Models;
using ApiTask.Services;
using ApiTask.Services.Dialogues;
using ApiTask.Services.Exceptions;
using ApiTask.Services.Messages;
using ApiTask.Services.ViewModelSubServices;
using ApiTask.Views;
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

        private UpdateTreeAlgorithm UpdateTreeAlgorithm;
        private SortingTreeMemento SortingTreeState;
        private List<string> CodesData = new List<string>();
        private List<List<string>> Params = new List<List<string>>();

        [ObservableProperty]
        ObservableCollection<SelectedMaterial> selectedMaterials =
            new ObservableCollection<SelectedMaterial>(new List<SelectedMaterial>());
        [ObservableProperty]
        string url;
        [ObservableProperty]
        ObservableCollection<string> details = new ObservableCollection<string>();
        [ObservableProperty]
        SmartCollection<Codes> categories = new SmartCollection<Codes>();

        public int SelectedRowIndex { get; set; }

        public void RegisterOpenSortingWindow(MainWindow w, TreeDialogueMessage m)
        {
            SortingTreeWindowViewModel model = GetModel();
            MessageHandler(model, w, m);
        }

        private SortingTreeWindowViewModel GetModel()
        {
            SortingTreeWindowViewModel model = new SortingTreeWindowViewModel();
            model.SetParameters(SortingTreeState);
            return model;
        }

        private void MessageHandler(SortingTreeWindowViewModel model, MainWindow w, TreeDialogueMessage m)
        {
            SortingTreeWindow dialogue = new SortingTreeWindow
            {
                DataContext = model
            };
            m.Reply(dialogue.ShowDialog<bool>(w));
        }

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
            UpdateTreeAlgorithm.UpdateCategories();
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
        private async Task OpenForm()
        {
            await OnOpenFormImpl();
        }

        private async Task OnOpenFormImpl()
        {
            Task applyToken = Task.Run(ApplyToken);
            Task getCodes = Task.Run(GetCodes);
            Task getParameters = Task.Run(GetParameters);
            Task.WaitAll(applyToken, getCodes, getParameters);
            WeakReferenceMessenger.Default.Send(new MainModelEnableButtonsMessage());
        }

        private async Task GetCodes()
        {
            try
            {
                await GetCodesImpl();
            }
            catch (SqlException e)
            {
                await MessageBoxHelper(e.Message, ErrorCallback);
            }

        }

        private async Task GetCodesImpl()
        {
            DbConnection connection = await OpenDbConnection();
            ParseData(await DbDataProcess.GetCodes(connection, CodesSqlKey));
            SetCategories();
        }

        private async Task GetParameters()
        {
            try
            {
                await GetParametersImpl();
            }
            catch (SqlException e)
            {
                await MessageBoxHelper(e.Message, ErrorCallback);
            }

        }

        private async Task GetParametersImpl()
        {
            DbConnection connection = await OpenDbConnection();
            SortingTreeState = await DbDataProcess.GetParameters(connection, ParametersSqlKey);
            UpdateTreeAlgorithm = new UpdateTreeAlgorithm(SortingTreeState, CodesData, Params, Categories);
        }

        private void ParseData(List<List<string>> data)
        {
            CodesData.AddRange(data[0]);
            for (int i = 0; i < data[1].Count; i++)
            {
                Params.Add(data[1][i].Split(',').ToList<string>());
            }
        }

        private void SetCategories()
        {
            List<Codes> codes = new List<Codes>();
            foreach (string code in CodesData)
            {
                codes.Add(new Codes(code));
            }
            Categories.AddRange(codes);
        }

        private async Task<DbConnection> OpenDbConnection()
        {
            try
            {
                return await GetConnection();
            }
            catch (SqlException e)
            {
                await MessageBoxHelper(e.Message, ErrorCallback);
            }
            catch (ArgumentNullException e)
            {
                await MessageBoxHelper(KeyMessage, ErrorCallback);
            }
            return null;
        }

        private async Task<DbConnection> GetConnection()
        {
            DbConnection connection = new DbConnection(ReadConfiguration.GetValueByKeyFromSecrets(DatabaseKey));
            await connection.OpenConnection();
            return connection;
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