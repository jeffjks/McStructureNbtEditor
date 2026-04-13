using McStructureNbtEditor.Commands;
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
        private readonly List<IEditorCommand> _commandHistory = new(MaxHistory);
        private int _currentIndex = 0;

        private const int MaxHistory = 128;

        public bool CanUndo => _currentIndex > 0;
        public bool CanRedo => _currentIndex < _commandHistory.Count;


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

        public bool ExecuteCommand(IEditorCommand command)
        {
            if (_currentIndex < _commandHistory.Count)
            {
                _commandHistory.RemoveRange(_currentIndex, _commandHistory.Count - _currentIndex);
            }

            if (_commandHistory.Count >= MaxHistory)
            {
                _commandHistory.RemoveAt(0);
                _currentIndex--;
            }

            if (!command.Execute(this))
                return false;

            _commandHistory.Add(command);
            _currentIndex++;
            StatusMessage = command.CommandStatusMessage;

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            RaiseDocumentChanged(command.ChangeType);
            return true;
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            _currentIndex--;

            var command = _commandHistory[_currentIndex];
            command.Undo(this);

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            RaiseDocumentChanged(command.ChangeType);

            StatusMessage = $"다음 작업이 실행 취소됨: ({command.CommandStatusMessage})";
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            var command = _commandHistory[_currentIndex];
            command.Execute(this);

            _currentIndex++;

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
            RaiseDocumentChanged(command.ChangeType);

            StatusMessage = $"다음 작업이 다시 실행됨: ({command.CommandStatusMessage})";
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