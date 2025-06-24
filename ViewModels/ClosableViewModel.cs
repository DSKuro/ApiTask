using ApiTask.Services.Dialogues;
using MsBox.Avalonia.Enums;
using System;
using System.Threading.Tasks;

namespace ApiTask.ViewModels
{
    public abstract class ClosableViewModel : ViewModelBase
    {
        public event EventHandler? ClosingRequest;

        private static readonly string ErrorTitle = "Ошибка";

        protected void OnClosingRequest()
        {
            if (ClosingRequest != null)
            {
                ClosingRequest(this, EventArgs.Empty);
            }
        }

        protected async Task MessageBoxHelper(string content, Action callback)
        {
            try
            {
                await this.ShowMessageBoxAsync(ErrorTitle, content, ButtonEnum.Ok);
            }
            finally
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
            }
        }
    }
}
