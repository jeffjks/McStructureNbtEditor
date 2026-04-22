using McStructureNbtEditor.ViewModels.Dialog;
using System.Windows;

namespace McStructureNbtEditor.Views
{
    public partial class CommonDialogView : Window
    {
        public CommonDialogView(CommonDialogViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
            viewModel.CloseAction = result =>
            {
                DialogResult = result;
                Close();
            };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
