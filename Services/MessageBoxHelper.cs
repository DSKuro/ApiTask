using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using System;
using System.Threading.Tasks;

namespace ApiTask.Services
{
    public static class MessageBoxHelper
    {
        public static Task<ButtonResult?> ShowMessageBoxAsync(this object? context, string? title, string? content,
            ButtonEnum buttons)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            TopLevel? topLevel = DialogueManager.GetTopLevelForContext(context);

            if (topLevel == null)
            {
                throw new ArgumentNullException(nameof(topLevel));
            }

            return ShowMessageBoxImpl(topLevel, title, content, buttons);
        }

        private static async Task<ButtonResult?> ShowMessageBoxImpl(TopLevel topLevel, string? title, string? content,
            ButtonEnum buttons)
        {
            // возможно условие излишне
            if (topLevel is Avalonia.Controls.Window window)
            {
                IMsBox<ButtonResult> box = MessageBoxManager.
                GetMessageBoxStandard(title, content,
                   buttons);

                await box.ShowWindowDialogAsync(window);
            }

            throw new ArgumentNullException(nameof(topLevel));
        }
    }
}
