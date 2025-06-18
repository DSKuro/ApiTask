using Avalonia.Controls;
using System;

namespace ApiTask.Services.Dialogues
{
    public static class DialogueHelper
    {
        public static TopLevel GetTopLevelForAnyDialogue(this object? context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            TopLevel? topLevel = DialogueManager.GetTopLevelForContext(context);

            if (topLevel == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return topLevel;
        }
    }
}
