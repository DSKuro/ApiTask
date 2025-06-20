using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ApiTask.Services.Messages
{
    public class TreeDialogueMessage : AsyncRequestMessage<bool>;
    public class TreeDialogueCloseMessage(bool isChanged)
    {
        public bool IsChanged { get; set; } = isChanged;
    }
}
