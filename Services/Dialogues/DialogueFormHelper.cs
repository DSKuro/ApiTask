using Avalonia.Controls;
using Eremex.AvaloniaUI.Controls.Common;
using System;
using System.Threading.Tasks;

namespace ApiTask.Services.Dialogues
{
    public static class DialogueFormHelper
    {
        public static async Task ShowFormAsDialogue(this object? context, MxWindow secondWindow)
        {
            TopLevel topLevel = context.GetTopLevelForAnyDialogue();
            await ShowFormAsDialogueImpl(topLevel, secondWindow);
        }

        private static async Task ShowFormAsDialogueImpl(TopLevel topLevel, MxWindow secondWindow)
        {
            // возможно условие излишне
            if (topLevel is Window window)
            {
               await secondWindow.ShowDialog(window);
            }

            throw new ArgumentNullException(nameof(topLevel));
        }
    }
}
