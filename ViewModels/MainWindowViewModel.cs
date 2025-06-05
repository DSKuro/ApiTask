using ApiTask.Models;
using ApiTask.Services;
using ApiTask.Services.Dialogues;
using ApiTask.Services.Exceptions;
using Avalonia.Controls.Shapes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Runtime.ExceptionServices;
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

        private Token AccessToken;

        public string Greeting { get; set; } = "Welcome to Avalonia";

        [RelayCommand]
        private async Task Click()
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
            string site = ReadConfiguration.getValueByKey(MaterialSite);
            foreach (string code in codes)
            {
                Materials material = (Materials)await Http.GetDataWithJSON(GetMaterialPath(site, code),
                typeof(Materials));
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
            Http.AddHeader(ReadConfiguration.getValueByKey(AuthHeader), AccessToken.AccessToken);
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
            string apiKey = ReadConfiguration.getValueByKey(Key);
            string authKey = ReadConfiguration.getValueByKey(AuthSite);
            return (apiKey, authKey);
        }

        private void ErrorCallback()
        {
            this.OnClosingRequest();
            Environment.Exit(1);
        }
    }
}
