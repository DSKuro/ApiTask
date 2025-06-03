using ApiTask.Services;
using ApiTask.Views;
using Avalonia.Utilities;
using Avalonia.Xaml.Interactions.Custom;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Threading.Tasks;

namespace ApiTask.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; set; } = "Welcome to Avalonia";

        [RelayCommand]
        private async Task Click()
        {
            //await MessageBox.Show(null, "test", "test", MessageBox.MessageBoxButtons.Ok);
            await this.ShowMessageBoxAsync("Title", "Content");
        }

        [RelayCommand]
        private void OpenForm()
        {
           // var box = MessageBoxManager
           //.GetMessageBoxStandard("Caption", "test",
           //    ButtonEnum.YesNo);

           // var result = box.ShowAsync();
        }
    }
}
