using ApiTask.Services.Messages;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ApiTask.ViewModels
{
    public partial class SortingTreeWindowViewModel : ClosableViewModel
    {
        private static List<bool?> SavedValues = new List<bool?>();
        private bool IsSave;

        [ObservableProperty]
        public static ObservableCollection<CheckBox> checkBoxes = new ObservableCollection<CheckBox>();

        [RelayCommand]
        public void CloseForm()
        {
            if (!IsSave)
            {
                RestoreStates();
            }
        }

        public static void SetParameters(List<string> parameters)
        {
            foreach (string parameter in parameters)
            {
                checkBoxes.Add(new CheckBox() { Content = parameter });
                SavedValues.Add(false);
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
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(new List<string>()));
        }

        private void RestoreStates()
        {
            for (int i = 0; i < CheckBoxes.Count; i++) 
            {
                CheckBoxes[i].IsChecked = SavedValues[i];
            }
        }

        public void OnOkButtonClick()
        {
            IsSave = true;
            WeakReferenceMessenger.Default.Send(new TreeDialogueCloseMessage(GetStates()));
        }

        private List<string> GetStates()
        {
            List<string> states = new List<string>();
            for (int i = 0; i < CheckBoxes.Count; i++)
            {
                if (!CheckBoxes[i].IsChecked == SavedValues[i])
                {
                    SavedValues[i] = CheckBoxes[i].IsChecked;
                    states.Add((string)CheckBoxes[i].Content);
                } 
            }
            return states;
        }
    }
}
