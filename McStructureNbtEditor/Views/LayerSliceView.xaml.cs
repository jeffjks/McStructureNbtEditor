using McStructureNbtEditor.Models;
using McStructureNbtEditor.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace McStructureNbtEditor.Views
{
    public partial class LayerSliceView : UserControl
    {
        public LayerSliceView()
        {
            InitializeComponent();
        }

        private LayerSliceViewModel? ViewModel => DataContext as LayerSliceViewModel;

        private void Cell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border border)
                return;

            if (border.Tag is not BlockCellModel cell)
                return;

            if (ViewModel == null)
                return;

            bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            ViewModel.BeginSelection(cell, isCtrlPressed, isShiftPressed);

            Mouse.Capture(this);
            e.Handled = true;
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is not Border border)
                return;

            if (border.Tag is not BlockCellModel cell)
                return;

            if (ViewModel == null)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            ViewModel.UpdateSelectionDrag(cell, isCtrlPressed, isShiftPressed);
        }

        private void Root_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel == null)
                return;

            ViewModel.EndSelection();
            Mouse.Capture(null);
            e.Handled = true;
        }
    }
}