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

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effects = files.Any(f => f.EndsWith(".nbt", StringComparison.OrdinalIgnoreCase))
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var nbtFile = files.FirstOrDefault(f => f.EndsWith(".nbt", StringComparison.OrdinalIgnoreCase));

            if (nbtFile == null)
                return;

            if (DataContext is FileMenuViewModel vm)
            {
                vm.OpenFile(nbtFile);
            }
        }
    }
}