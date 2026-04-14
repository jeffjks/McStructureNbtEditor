using McStructureNbtEditor.ViewModels;
using System.Windows;

namespace McStructureNbtEditor.Views
{
    public partial class HasChangesDialogView : Window
    {
        public HasChangesDialogView(HasChangesDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
