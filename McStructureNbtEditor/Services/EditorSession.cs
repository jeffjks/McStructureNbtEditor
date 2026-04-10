using McStructureNbtEditor.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.Services
{
    public interface ISnbtInspectable
    {
        string GetSnbtText();
    }

    public sealed class EditorSession : INotifyPropertyChanged
    {
        private StructureFileModel? _currentStructure;
        public StructureFileModel? CurrentStructure
        {
            get => _currentStructure;
            set
            {
                if (_currentStructure == value)
                    return;
                _currentStructure = value;
                OnPropertyChanged(nameof(CurrentStructure));
                SelectedPaletteEntry = CurrentStructure?.GetPaletteEntry(0);
            }
        }

        private BlockPosition _requestedCellSelection;
        public BlockPosition RequestedCellSelection
        {
            get => _requestedCellSelection;
            set
            {
                if (_requestedCellSelection == value)
                    return;
                _requestedCellSelection = value;
                OnPropertyChanged();
            }
        }

        private ISnbtInspectable? _selectedInspectable;
        public ISnbtInspectable? SelectedInspectable
        {
            get => _selectedInspectable;
            set
            {
                if (_selectedInspectable == value)
                    return;
                _selectedInspectable = value;
                OnPropertyChanged(nameof(SelectedInspectable));
            }
        }

        private PaletteEntry? _selectedPaletteEntry;
        public PaletteEntry? SelectedPaletteEntry
        {
            get => _selectedPaletteEntry;
            set
            {
                if (_selectedPaletteEntry == value)
                    return;
                _selectedPaletteEntry = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage == value)
                    return;
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}