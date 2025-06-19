using ApiTask.Services;
using ApiTask.Services.Messages;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ApiTask.ViewModels
{
    public partial class SortingTreeWindowViewModel : ClosableViewModel
    {
        private bool IsChanged = false;
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
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(IsChanged));
        }

        public void OnOkButtonClick()
        {
            SaveState();
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(IsChanged));
        }

        private void SaveState()
        {
            State.EnabledParameters = SaveStateImpl();
        }

        private List<string> SaveStateImpl()
        {
            List<string> changedParameters = new List<string>();
            for (int i = 0; i < CheckBoxes.Count; i++)
            {
                SaveParameters(changedParameters, i);
            }
            return changedParameters;
        }

        private void SaveParameters(List<string> changedParameters, int i)
        {
            if ((bool)CheckBoxes[i].IsChecked)
            {
                changedParameters.Add((string)CheckBoxes[i].Content);
            }
            if (!CheckBoxes[i].IsChecked == State.States[i])
            {
                IsChanged = true;
                State.States[i] = CheckBoxes[i].IsChecked;
            }
        }
    }
}
