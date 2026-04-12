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
        // TODO. Linked List로 변경
        private readonly Stack<IEditorCommand> _undoStack = new();
        private readonly Stack<IEditorCommand> _redoStack = new();


        public event EventHandler? DocumentChanged;
        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        private StructureFileModel? _currentStructure;
        public StructureFileModel? CurrentStructure
        {
            get => _currentStructure;
            set
            {
                if (_currentStructure == value)
                    return;
                _currentStructure = value;
                _undoStack.Clear();
                _redoStack.Clear();
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasStructure));
                OnPropertyChanged(nameof(CanUndo));
                OnPropertyChanged(nameof(CanRedo));

                OnPropertyChanged(nameof(CurrentStructure));
                SelectedPaletteEntry = CurrentStructure?.GetPaletteEntry(0);
                RaiseDocumentChanged();
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
            if (!command.CanExecute(this))
                return false;

            command.Execute(this);
            _undoStack.Push(command);
            _redoStack.Clear();

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));

            StatusMessage = command.Description;
            return true;
        }

        public bool Undo()
        {
            if (_undoStack.Count == 0)
                return false;

            var command = _undoStack.Pop();
            command.Undo(this);
            _redoStack.Push(command);

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));

            StatusMessage = $"실행 취소: {command.Description}";
            return true;
        }

        public bool Redo()
        {
            if (_redoStack.Count == 0)
                return false;

            var command = _redoStack.Pop(); 
            command.Execute(this);
            _undoStack.Push(command);

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));

            StatusMessage = $"다시 실행: {command.Description}";
            return true;
        }

        public void RaiseDocumentChanged()
        {
            DocumentChanged?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}