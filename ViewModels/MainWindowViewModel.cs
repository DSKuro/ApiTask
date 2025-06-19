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

        private List<List<string>> MainData;
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
            bool isChanged = await WeakReferenceMessenger.Default.Send(new TreeDialogueMessage());
            Categories.Clear();
            List<string> nonCat = new List<string>();
            List<string> cat = new List<string>();
            List<List<string>> partial = new List<List<string>>();
            for (int i = 0; i < SortingTreeState.ChangedParameters.Count; i++)
            {
                partial.Add(new List<string>());
            }
            if (isChanged)
            {
                Categories.Clear();
                int param = 0;
                for (int i = 0; i < Params.Count; i++)
                {
                   for (int j = 0; j < SortingTreeState.ChangedParameters.Count; j++)
                   {
                      
                        if (Params[i].Contains(SortingTreeState.ChangedParameters[j]))
                        {
                            param++;
                        }
                   }
                   if (param == 0)
                   {
                        nonCat.Add(MainData[0][i]);
                   }
                   else if (param == SortingTreeState.ChangedParameters.Count)
                   {
                        cat.Add(MainData[0][i]);
                   }
                   else
                    {
                        partial[param].Add(MainData[0][i]);
                    }
                        param = 0;
                   
                }
                CodeCategory nonCategory = new CodeCategory("Без категории");
                CodeCategory allCategory = new CodeCategory("Все параметры");
                List<CodeCategory> categories = new List<CodeCategory>();
                foreach (string code in cat)
                {
                    allCategory.Codes.Add(new Codes(code));
                }

                foreach (string code in nonCat)
                {
                    nonCategory.Codes.Add(new Codes(code));
                }

                for (int i =0; i < partial.Count; i++) 
                {
                    CodeCategory t = new CodeCategory($"Без параметра: {SortingTreeState.ChangedParameters[i]}");
                    foreach (string code in partial[i])
                    {
                        t.Codes.Add(new Codes(code)); 
                    }
                    Categories.Add(t);
                }


                Categories.Add(nonCategory);
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
            await ApplyToken();
            await OpenDbConnection();
            await GetCodes();
            await GetParameters();
        }

        private async Task GetParameters()
        {
            List<List<string>> data = await DbConnection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(ParametersSqlKey), 1);
            SortingTreeState = new SortingTreeMemento(data[0]);
        }

        private async Task GetCodes()
        {
            MainData = await DbConnection.GetData
                (ReadConfiguration.GetValueByKeyFromConfiguration(CodesSqlKey), 2);

            for (int i = 0; i < MainData[1].Count; i++) 
            {
                Params.Add(MainData[1][i].Split(',').ToList<string>());
            }
            foreach (string testitem in MainData[0])
            {
                 Categories.Add(new Codes(testitem));
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
