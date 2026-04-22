using McStructureNbtEditor.ViewModels.Dialog;
using System.Windows;

namespace McStructureNbtEditor.Views
{
    public partial class NewFileDialogView : Window
    {
        public NewFileDialogView(NewFileDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.CloseAction = result =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
}
