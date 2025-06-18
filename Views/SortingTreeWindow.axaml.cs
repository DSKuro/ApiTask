using ApiTask.Services.Messages;
using CommunityToolkit.Mvvm.Messaging;
using Eremex.AvaloniaUI.Controls.Common;

namespace ApiTask;

public partial class SortingTreeWindow : MxWindow
{
    public SortingTreeWindow()
    {
        InitializeComponent();
        RegisterMessage();
    }

    private void RegisterMessage()
    {
        WeakReferenceMessenger.Default.Register<SortingTreeWindow, TreeDialogueCloseMessage>(this,
            static (window, message) =>
            {
                window.Close(message.NewParameters);
            }
        );
    }
}
