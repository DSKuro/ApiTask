using ApiTask.Services;
using ApiTask.Services.Messages;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace ApiTask.ViewModels
{
    public partial class SortingTreeWindowViewModel : ClosableViewModel
    {
        private SortingTreeMemento State;

        [ObservableProperty]
        public ObservableCollection<CheckBox> checkBoxes = new ObservableCollection<CheckBox>();

        public void SetParameters(SortingTreeMemento memento)
        {
            State = memento;
            RestoreState();
        }

        private void RestoreState()
        {
            for (int i = 0; i < State.Content.Count; i++)
            {
                CheckBoxes.Add(new CheckBox() { Content = State.Content[i], IsChecked = State.States[i] });
            }
        }

        public void CheckAllButtonClick()
        {
            ChangeCheckedState(true);
        }

        public void UnCheckAllButtonClick()
        {
            ChangeCheckedState(false);
        }

        private void ChangeCheckedState(bool value)
        {
            foreach (CheckBox checkBox in CheckBoxes)
            {
                checkBox.IsChecked = value;
            }
        }

        public void OnCancelButtonClick()
        {
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(false));
        }

        public void OnOkButtonClick()
        {
            SaveState();
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(true));
        }

        private void SaveState()
        {
            for (int i = 0; i < CheckBoxes.Count; i++)
            {
                if (!CheckBoxes[i].IsChecked == State.States[i])
                {
                    State.States[i] = CheckBoxes[i].IsChecked;
                }
            }
        }
    }
}
