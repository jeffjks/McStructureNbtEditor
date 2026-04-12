using System.Windows.Input;

namespace McStructureNbtEditor.ViewModels
{
    public class CommonDialogViewModel
    {
        public string Title { get; }
        public string Message { get; }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }


        public Action<bool?>? CloseAction { get; set; }

        public CommonDialogViewModel(string title, string message)
        {
            Title = title;
            Message = message;
            
            ConfirmCommand = new RelayCommand(() => CloseAction?.Invoke(true));
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
        }
    }
}
