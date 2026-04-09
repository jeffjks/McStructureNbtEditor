using McStructureNbtEditor.Models;
using McStructureNbtEditor.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace McStructureNbtEditor.Views
{
    public partial class LayerSliceView : UserControl
    {
        private bool _isDraggingSelection;

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

            Keyboard.ClearFocus();
            // Focus();

            bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            _isDraggingSelection = true;
            Mouse.Capture(CellItemsControl);

            ViewModel.BeginSelection(cell, isCtrlPressed, isShiftPressed);

            e.Handled = true;
        }

        private void CellItemsControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDraggingSelection)
                return;

            if (ViewModel == null)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var position = e.GetPosition(CellItemsControl);
            var hit = VisualTreeHelper.HitTest(CellItemsControl, position);

            if (hit == null)
                return;

            var border = FindAncestor<Border>(hit.VisualHit);

            if (border?.Tag is not BlockCellModel cell)
                return;

            bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool isShiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

            ViewModel.UpdateSelectionDrag(cell, isCtrlPressed, isShiftPressed);
        }

        private void CellItemsControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndDragSelection();
        }

        private void EndDragSelection()
        {
            if (!_isDraggingSelection)
                return;

            _isDraggingSelection = false;

            if (Mouse.Captured == CellItemsControl)
                Mouse.Capture(null);

            ViewModel?.EndSelection();
        }

        private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T matched)
                    return matched;

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }
    }
}