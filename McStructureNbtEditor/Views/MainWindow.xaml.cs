using System.Windows;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor.Views
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