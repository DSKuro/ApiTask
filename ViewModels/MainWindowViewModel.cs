using ApiTask.Models;
using ApiTask.Services;
using ApiTask.Services.Dialogues;
using Avalonia.Controls.Shapes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTask.ViewModels
{
    public partial class MainWindowViewModel : ClosableViewModel
    {

        private static readonly string Key = "api";
        private static readonly string AuthKey = "auth";
        private static readonly string JsonKey = "access_token";

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
                GetMaterialDataImpl();
            }
            catch (UriFormatException ex)
            {
                await ErrorHelper("Неправильный Uri адрес");

            }
            catch (HttpRequestException ex)
            {
                await ErrorHelper("Ошибка ответа сайта");
            }
            catch (OperationCanceledException ex)
            {
                await ErrorHelper("Ошибка чтения");
            }
            catch (ArgumentNullException ex)
            {
                await ErrorHelper("Данных нет");
            }
            catch (InvalidOperationException ex)
            {
                await ErrorHelper("Недопустимая операция");
            }
            catch (Exception ex)
            {
                await ErrorHelper("Ошибка подключения");
            }
        }

        private async void GetMaterialDataImpl()
        {
            string path = null;
            try
            {
                path = await GetAbsolutePathFile();
            }
            catch (Exception)
            {
                await ErrorHelper("test");
            }

            ExcelParser.GetDataFromFile(path);
            Materials material = (Materials)await Http.GetDataWithJSON("https://api.dkc.ru/v1/catalog/materil", null, "code=R5NFPB80",
                typeof(Materials));
        }

        private async Task<string?> GetAbsolutePathFile()
        {
            IEnumerable<IStorageFile?> selectFiles = await this.OpenFileDialogueAsync("test");
            string path;
            path = selectFiles.First().Path.AbsolutePath;
            return path;
        }

        [RelayCommand]
        private async Task OpenForm()
        {
            await GetKey();
            Http.SetAuthorizationHeader(AccessToken.AccessToken);
        }

        private async Task GetKey()
        {
            (string? apiKey, string? authKey) = await GetKeysFromConfig();

            AccessToken =
                (Token)await GetToken(authKey, apiKey, typeof(Token));

            if (AccessToken == null)
            {
                await ErrorHelper("Пустой токен");
            }
        }

        private async Task<(string?, string?)> GetKeysFromConfig()
        {
            string? apiKey = ReadConfiguration.getValueByKey(Key);
            if (apiKey == null)
            {
                await ErrorHelper("Api ключ не задан");
            }

            string? authKey = ReadConfiguration.getValueByKey(AuthKey);
            if (authKey == null)
            {
                await ErrorHelper("Сайт авторизации не задан");
            }

            return (apiKey, authKey);
        }

        private async Task<object?> GetToken(string path, string? param, Type type)
        {
            object? token = null;
            try
            {
                token = await Http.GetDataWithJSON(path, param, null, type);
            }
            catch (UriFormatException ex)
            {
                await ErrorHelper("Неправильный Uri адрес");

            }
            catch (HttpRequestException ex)
            {
                await ErrorHelper("Ошибка ответа сайта");
            }
            catch (OperationCanceledException ex)
            {
                await ErrorHelper("Ошибка чтения");
            }
            catch (ArgumentNullException ex)
            {
                await ErrorHelper("Данных нет");
            }
            catch (Exception ex)
            {
                await ErrorHelper("Ошибка подключения");
            }

            return token;
        }

        private async Task ErrorHelper(string content)
        {
            try
            {
                await this.ShowMessageBoxAsync("Ошибка", content, ButtonEnum.Ok);
            }
            finally
            {
                this.OnClosingRequest();
                Environment.Exit(1);
            }
        }
    }
}
