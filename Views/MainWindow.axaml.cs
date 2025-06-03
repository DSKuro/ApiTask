using ApiTask.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace ApiTask.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //connectToApi();
        }

        public void ClickHandler(object sender, RoutedEventArgs args)
        {
            //MessageBox.Show(this, "test", "Title", MessageBox.MessageBoxButtons.Ok);
                var box = MessageBoxManager
               .GetMessageBoxStandard("Caption", "test",
                   ButtonEnum.YesNo);

                var result =  box.ShowWindowDialogAsync(this);
        }
    }
}