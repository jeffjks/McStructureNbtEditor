using McStructureNbtEditor.Commands;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace McStructureNbtEditor.ViewModels
{
    public class BlockEditViewModel : INotifyPropertyChanged
    {
        private readonly EditorSession _session;

        public IReadOnlySet<SelectedCell> SelectedCells => _session.SelectedCells;
        public PaletteEntry? SelectedPaletteEntry => _session.SelectedPaletteEntry;

        public RelayCommand FillBlockCommand { get; }
        public RelayCommand RotateBlockCommand { get; }
        public RelayCommand RemoveBlockCommand { get; }

        public BlockEditViewModel(EditorSession session)
        {
            _session = session;

            FillBlockCommand = new RelayCommand(FillBlock, () => CanEditBlock(true));
            RotateBlockCommand = new RelayCommand(RotateBlock, () => { return false; });
            RemoveBlockCommand = new RelayCommand(RemoveBlock, () => CanEditBlock(false));

            PropertyChanged += OnSessionPropertyChanged;
            _session.PropertyChanged += OnSessionPropertyChanged;
        }

        private void FillBlock()
        {
            var structure = _session.CurrentStructure;

            if (structure == null)
                return;
            if (SelectedCells.Count == 0)
                return;
            if (SelectedPaletteEntry == null)
                return;

            var cells = SelectedCells.ToHashSet();

            int paletteIndex = SelectedPaletteEntry.Index;
            if (paletteIndex < 0 || structure.Palette.Count <= paletteIndex)
            {
                _session.StatusMessage = "삭제할 팔레트를 찾을 수 없습니다.";
                return;
            }

            var command = new FillBlockCommand(cells, SelectedPaletteEntry);
            if (!_session.ExecuteCommand(command))
            {
                _session.StatusMessage = "블록 채우기에 실패했습니다.";
            }
        }

        private void RotateBlock()
        {
            throw new NotImplementedException();
        }

        private void RemoveBlock()
        {
            var structure = _session.CurrentStructure;

            if (structure == null)
                return;
            if (SelectedCells.Count == 0)
                return;

            var cells = new HashSet<SelectedCell>(SelectedCells);

            var command = new RemoveBlockCommand(cells);
            if (!_session.ExecuteCommand(command))
            {
                _session.StatusMessage = "블록 삭제에 실패했습니다.";
            }
        }

        private bool CanEditBlock(bool usePalette)
        {
            if (_session.CurrentStructure == null)
                return false;
            if (usePalette && SelectedPaletteEntry == null)
                return false;
            if (SelectedCells.Count == 0)
                return false;
            return true;
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_session.CurrentStructure) ||
                e.PropertyName == nameof(_session.SelectedPaletteEntry) ||
                e.PropertyName == nameof(_session.SelectedCells))
            {
                FillBlockCommand.RaiseCanExecuteChanged();
                RotateBlockCommand.RaiseCanExecuteChanged();
                RemoveBlockCommand.RaiseCanExecuteChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
