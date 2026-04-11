using System.Windows;


namespace McStructureNbtEditor.Views
{
    public partial class AddPaletteDialog : Window
    {
        public AddPaletteDialog()
        {
            InitializeComponent();
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