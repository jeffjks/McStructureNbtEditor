using fNbt;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.ViewModels
{
    public class NbtTreeViewModel : INotifyPropertyChanged
    {
        private readonly EditorSession _session;

        public RelayCommand JumpToTreeSelectedBlockCommand { get; }

        private NbtTreeNode? _selectedTreeNode;
        public NbtTreeNode? SelectedTreeNode
        {
            get => _selectedTreeNode;
            set
            {
                if (_selectedTreeNode == value)
                    return;

                _selectedTreeNode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanJumpToTreeSelectedBlock));
                JumpToTreeSelectedBlockCommand.RaiseCanExecuteChanged();
                _session.SelectedInspectable = _selectedTreeNode;
            }
        }

        public NbtTreeViewModel(EditorSession session)
        {
            _session = session;

            JumpToTreeSelectedBlockCommand = new RelayCommand(
                JumpToTreeSelectedBlock,
                () => CanJumpToTreeSelectedBlock);
        }

        private void JumpToTreeSelectedBlock()
        {
            if (!TryGetBlockPositionFromTreeNode(SelectedTreeNode, out int x, out int y, out int z))
                return;

            _session.RequestedCellSelection = new BlockPosition(x, y, z);
        }

        public bool CanJumpToTreeSelectedBlock =>
            TryGetBlockPositionFromTreeNode(SelectedTreeNode, out _, out _, out _);

        private bool TryGetBlockPositionFromTreeNode(NbtTreeNode? node, out int x, out int y, out int z)
        {
            x = y = z = 0;

            if (node?.Tag is not NbtCompound compound)
                return false;

            if (node?.IsBlockNode == false)
                return false;

            if (!compound.Contains("pos"))
                return false;

            if (compound["pos"] is not NbtList posList)
                return false;

            if (posList.Count < 3)
                return false;

            try
            {
                x = ((NbtInt)posList[0]).Value;
                y = ((NbtInt)posList[1]).Value;
                z = ((NbtInt)posList[2]).Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
