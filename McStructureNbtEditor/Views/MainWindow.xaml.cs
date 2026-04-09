using McStructureNbtEditor.Models;
using McStructureNbtEditor.ViewModels;
using System.Windows;

namespace McStructureNbtEditor.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel mainVM)
            {
                mainVM.NbtTree.SelectedTreeNode = e.NewValue as NbtTreeNode;
            }
        }
    }
}