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
        private bool _isChanged = false;
        private SortingTreeMemento _state;

        [ObservableProperty]
        public ObservableCollection<CheckBox> checkBoxes = new ObservableCollection<CheckBox>();

        public void SetParameters(SortingTreeMemento memento)
        {
            _state = memento;
            RestoreState();
        }

        private void RestoreState()
        {
            for (int i = 0; i < _state.Content.Count; i++)
            {
                CheckBoxes.Add(new CheckBox() { Content = _state.Content[i], IsChecked = _state.States[i] });
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
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(_isChanged));
        }

        public void OnOkButtonClick()
        {
            SaveState();
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(_isChanged));
        }

        private void SaveState()
        {
            _state.EnabledParameters = SaveStateImpl();
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
            if (!CheckBoxes[i].IsChecked == _state.States[i])
            {
                _isChanged = true;
                _state.States[i] = CheckBoxes[i].IsChecked;
            }
        }
    }
}
