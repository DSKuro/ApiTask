using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTask.Services
{
    public static class MessageBoxHelper
    {
        public static Task<ButtonResult?> ShowMessageBoxAsync(this object? context, string? title, string? content)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            TopLevel topLevel = DialogueManager.GetTopLevelForContext(context);

            return ShowMessageBoxImpl(topLevel, title, content);
        }

        private static async Task<ButtonResult?> ShowMessageBoxImpl(TopLevel topLevel, string? title, string? content)
        {
            // возможно второе условие излишне
            if (topLevel != null
                && topLevel is Avalonia.Controls.Window)
            {
                IMsBox<ButtonResult> box = MessageBoxManager.
                GetMessageBoxStandard("Caption", "test",
                   ButtonEnum.YesNo);

                await box.ShowWindowDialogAsync((Avalonia.Controls.Window)topLevel);
            }

            return null;
        }
    }
}
