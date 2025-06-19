using ApiTask.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;

namespace ApiTask.Services.Messages
{
    public class TreeDialogueMessage : AsyncRequestMessage<bool>;
    public class TreeDialogueCloseMessage(bool isChanged)
    {
        public bool IsChanged { get; set; } = isChanged;
    }
}
