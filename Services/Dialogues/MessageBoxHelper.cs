using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using System;
using System.Threading.Tasks;

namespace ApiTask.Services.Dialogues
{
    public static class MessageBoxHelper
    {
        public static Task<ButtonResult?> ShowMessageBoxAsync(this object? context, string? title, string? content,
            ButtonEnum buttons)
        {
            TopLevel topLevel = context.GetTopLevelForAnyDialogue();
            return ShowMessageBoxImpl(topLevel, title, content, buttons);
        }

        private static async Task<ButtonResult?> ShowMessageBoxImpl(TopLevel topLevel, string? title, string? content,
            ButtonEnum buttons)
        {
            // возможно условие излишне
            if (topLevel is Window window)
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
