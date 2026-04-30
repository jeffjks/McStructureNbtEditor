using System.Windows;
using System.Windows.Input;

namespace McStructureNbtEditor.ViewModels.Dialog
{
    public enum HasChangesDialogResult
    {
        Save,
        Ignore,
        Cancel
    }

    public class HasChangesDialogViewModel
    {
        public string Message { get; }

        public ICommand SaveCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand CancelCommand { get; }

        public HasChangesDialogResult Result { get; private set; } = HasChangesDialogResult.Cancel;


        public HasChangesDialogViewModel(string message)
        {
            Message = message;
            SaveCommand = new RelayCommand<Window>(OnSave);
            ExitCommand = new RelayCommand<Window>(OnExit);
            CancelCommand = new RelayCommand<Window>(OnCancel);
        }

        private void OnSave(Window? window)
        {
            if (window == null)
                return;
            Result = HasChangesDialogResult.Save;
            Close(window, true);
        }

        private void OnExit(Window? window)
        {
            if (window == null)
                return;
            Result = HasChangesDialogResult.Ignore;
            Close(window, true);
        }

        private void OnCancel(Window? window)
        {
            if (window == null)
                return;
            Result = HasChangesDialogResult.Cancel;
            Close(window, false);
        }

        private void Close(Window? window, bool dialogResult)
        {
            if (window == null)
                return;
            window.DialogResult = dialogResult;
        }
    }
}
