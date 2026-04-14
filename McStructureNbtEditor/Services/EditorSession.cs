using McStructureNbtEditor.Commands;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.ViewModels;
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
        private readonly List<IEditorCommand> _commandHistory = new(MaxHistory);
        private int _currentHistoryIndex = 0;
        private int _savedHistoryIndex = 0;
        private bool _exceedMaxHistory = false;

        private const int MaxHistory = 128;

        public bool CanUndo => _currentHistoryIndex > 0;
        public bool CanRedo => _currentHistoryIndex < _commandHistory.Count;
        public bool HasChanges => (_currentHistoryIndex != _savedHistoryIndex) || _exceedMaxHistory;


        public event EventHandler<DocumentChangedEventArgs>? DocumentChanged;

        private StructureFileModel? _currentStructure;
        public StructureFileModel? CurrentStructure
        {
            get => _currentStructure;
            set
            {
                if (_currentStructure == value)
                    return;
                _currentStructure = value;
                _commandHistory.Clear();
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasStructure));
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));

                OnPropertyChanged(nameof(CurrentStructure));
                SelectedPaletteEntry = CurrentStructure?.GetPaletteEntry(0);
                RaiseDocumentChanged(ReloadScope.ReloadFile);
            }
        }
        public bool HasStructure => CurrentStructure != null;

        public HashSet<SelectedCell> SelectedCells { get; } = new();

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

                if (value is not NbtTreeNode && _selectedInspectable is NbtTreeNode oldTreeNode)
                    oldTreeNode.IsSelected = false;

                _selectedInspectable = value;
                OnPropertyChanged(nameof(SelectedInspectable));
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

        public bool ExecuteCommand(IEditorCommand command)
        {
            if (!command.Execute(this))
                return false;

            if (_currentHistoryIndex < _commandHistory.Count)
            {
                _commandHistory.RemoveRange(_currentHistoryIndex, _commandHistory.Count - _currentHistoryIndex);
            }

            if (_commandHistory.Count >= MaxHistory)
            {
                _exceedMaxHistory = true;
                _commandHistory.RemoveAt(0);

                if (_currentHistoryIndex > 0)
                    _currentHistoryIndex--;
                if (_savedHistoryIndex > 0)
                    _savedHistoryIndex--;
            }

            _commandHistory.Add(command);
            _currentHistoryIndex++;
            StatusMessage = command.CommandStatusMessage;

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            OnPropertyChanged(nameof(HasChanges));
            RaiseDocumentChanged(command.ChangeType);
            return true;
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            _currentHistoryIndex--;

            var command = _commandHistory[_currentHistoryIndex];
            command.Undo(this);

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            OnPropertyChanged(nameof(HasChanges));
            RaiseDocumentChanged(command.ChangeType);

            StatusMessage = $"다음 작업이 실행 취소됨: ({command.CommandStatusMessage})";
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            var command = _commandHistory[_currentHistoryIndex];
            command.Execute(this);

            _currentHistoryIndex++;

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            OnPropertyChanged(nameof(HasChanges));
            RaiseDocumentChanged(command.ChangeType);

            StatusMessage = $"다음 작업이 다시 실행됨: ({command.CommandStatusMessage})";
        }

        public void SetSavedHistoryIndex()
        {
            _savedHistoryIndex = _currentHistoryIndex;
            _exceedMaxHistory = false;
            OnPropertyChanged(nameof(HasChanges));
        }

        public void NotifySelectedBlockIndicesChanged()
        {
            OnPropertyChanged(nameof(SelectedCells));
        }

        public void RaiseDocumentChanged(ReloadScope type)
        {
            DocumentChanged?.Invoke(this, new DocumentChangedEventArgs { ChangeType = type });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}