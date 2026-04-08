using System.Windows;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}