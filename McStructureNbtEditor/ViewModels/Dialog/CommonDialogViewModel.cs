using System.Windows.Input;

namespace McStructureNbtEditor.ViewModels.Dialog
{
    public class CommonDialogViewModel
    {
        public string Title { get; }
        public string Message { get; }
        public bool VisibleCancel { get; }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }


        public Action<bool?>? CloseAction { get; set; }

        public static CommonDialogViewModel AboutDialog() => new(
            "프로그램 정보",
            $"Minecraft Structure Editor © 2026\nVersion: {App.Version}\nCreated by: jeffjkf93@gmail.com",
            false
        );

        public CommonDialogViewModel(string title, string message, bool visibleCancel)
        {
            Title = title;
            Message = message;
            VisibleCancel = visibleCancel;

            ConfirmCommand = new RelayCommand(() => CloseAction?.Invoke(true));
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
        }
    }
}
