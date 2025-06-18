using ApiTask.ViewModels;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;

namespace ApiTask.Services.Messages
{
    public class TreeDialogueMessage : AsyncRequestMessage<List<string>>;
    public class TreeDialogueCloseMessage(List<string> parameters)
    {
        public List<string> NewParameters { get; } = parameters; 
    }
}
