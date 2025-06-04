using ApiTask.Models;
using ApiTask.Services;
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
        public string Greeting { get; set; } = "Welcome to Avalonia";

        [RelayCommand]
        private async Task Click()
        {
            IEnumerable<IStorageFile?> selectFiles = await this.OpenFileDialogueAsync("test");
            string path;
            try
            {
                path = selectFiles.First().Path.AbsolutePath;
            }
            catch (Exception ex)
            {
                return;
            }
            ExcelParser.GetDataFromFile(path);
            Console.WriteLine();
        }

        [RelayCommand]
        private async Task OpenForm()
        {
            (string? apiKey, string? authKey) = await GetKeys();

            Dictionary<string, string?>? data =
                (Dictionary<string, string?>?) await GetToken(authKey, apiKey, typeof(Dictionary<string, string>));

            Token.AccessToken = data[JsonKey];
            Console.WriteLine();
        }

        private async Task<(string?, string?)> GetKeys()
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
                token = await Http.GetDataWithJSON(path, param, type);
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
