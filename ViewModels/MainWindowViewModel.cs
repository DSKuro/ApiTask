using ApiTask.Models;
using ApiTask.Services;
using ApiTask.Services.Dialogues;
using ApiTask.Services.Exceptions;
using Avalonia.Data.Converters;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Binding;
using Eremex.AvaloniaUI.Controls.DataControl.Visuals;
using System;  
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTask.ViewModels
{
    public partial class MainWindowViewModel : ClosableViewModel
    {
        private static readonly string Key = "api";
        private static readonly string AuthSite = "auth";
        private static readonly string AuthHeader = "header";
        private static readonly string MaterialSite = "material";
        private static readonly string FileDialogueTitle = "Выберите файл:";
        private static readonly string Explorer = "explorer.exe";
        private static readonly string SiteError = "Ошибка при открытии страницы товара";

        private Token AccessToken;

        public int SelectedRowIndex { get; set; }
        public event EventHandler? DataGridChanged;

        [ObservableProperty]
        ObservableCollection<SelectedMaterial> selectedMaterials =
            new ObservableCollection<SelectedMaterial>(new List<SelectedMaterial>());

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
                await GetMaterialDataImpl();
            }
            catch (HttpException ex)
            {
                await MessageBoxHelper(ex.Message, ErrorCallback);
            }
            catch (ArgumentNullException e)
            {
                await MessageBoxHelper("Неправильно задан ключ", ErrorCallback);
            }
            catch (InvalidOperationException ex)
            {
                return;
            }
        }

        private async Task GetMaterialDataImpl()
        {
            string path = await GetAbsolutePathFile();
            List<string> codes = ExcelParser.GetDataFromFile(path);
            await GetAllMaterials(codes);
        }

        private async Task GetAllMaterials(List<string> codes) 
        {
            SelectedMaterials.Clear();
            string site = ReadConfiguration.GetValueByKeyFromConfiguration(MaterialSite);
            foreach (string code in codes)
            {
                Materials material = (Materials)await Http.GetDataWithJSON(GetMaterialPath(site, code),
                typeof(Materials));
                SelectedMaterials.Add(new SelectedMaterial(material));
            }
        }

        private Http.QueryBuilder GetMaterialPath(string uri, string code)
        {
            Http.QueryBuilder path = new Http.QueryBuilder(uri);
            path.SetQuery("code", code);
            return path;
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
            await ApplyToken();
        }

        private async Task ApplyToken()
        {
            try
            {
                await ApplyTokenImpl();
            }
            catch (HttpException ex)
            {
                await MessageBoxHelper(ex.Message, ErrorCallback);
            }
            catch (ArgumentNullException e)
            {
                await MessageBoxHelper("Неправильно задан ключ", ErrorCallback);
            }
        }

        private async Task ApplyTokenImpl()
        {
            await GetToken();
            Http.AddHeader(ReadConfiguration.GetValueByKeyFromSecrets(AuthHeader), AccessToken.AccessToken);
        }

        private async Task GetToken()
        {
            (string apiKey, string authKey) = GetKeysFromConfig();
            AccessToken = (Token)await Http.GetDataWithJSON(GetFullTokenPath(authKey, apiKey), typeof(Token));
        }

        private Http.QueryBuilder GetFullTokenPath(string uri, string param)
        {
            Http.QueryBuilder path = new Http.QueryBuilder(uri);
            path.SetParam(param);
            return path;
        }

        private (string, string) GetKeysFromConfig()
        {
            string apiKey = ReadConfiguration.GetValueByKeyFromSecrets(Key);
            string authKey = ReadConfiguration.GetValueByKeyFromConfiguration(AuthSite);
            return (apiKey, authKey);
        }

        private void ErrorCallback()
        {
            this.OnClosingRequest();
            Environment.Exit(1);
        }
    }
}
