using fNbt;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.ViewModels
{
    public class NbtTreeViewModel : INotifyPropertyChanged
    {
        private readonly EditorSession _session;
        private readonly IStructureNbtSerializer _serializer;
        private readonly NbtTreeBuilder _treeBuilder;

        public ObservableCollection<NbtTreeNode> RootNodes { get; } = new();


        public event Action? TreeViewSelectionChanged;

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
                TreeViewSelectionChanged?.Invoke();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanJumpToTreeSelectedBlock));
                JumpToTreeSelectedBlockCommand.RaiseCanExecuteChanged();
                _session.SelectedInspectable = value;
            }
        }

        public NbtTreeViewModel(EditorSession session, IStructureNbtSerializer serializer, NbtTreeBuilder treeBuilder)
        {
            _session = session;
            _serializer = serializer;
            _treeBuilder = treeBuilder;

            _session.DocumentChanged += OnDocumentChanged;
            _session.PropertyChanged += OnSessionPropertyChanged;

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

        private void OnDocumentChanged(object? sender, DocumentChangedEventArgs e)
        {
            RebuildTree();
        }

        public void RebuildTree()
        {
            RootNodes.Clear();

            if (_session.CurrentStructure == null)
                return;

            var rootTag = _serializer.BuildRootTag(_session.CurrentStructure);

            RootNodes.Add(_treeBuilder.BuildRoot(rootTag, GetRootName()));
        }

        private void RefreshRootName()
        {
            if (RootNodes.Count == 0)
                return;
            RootNodes[0].Name = GetRootName();
        }

        private string GetRootName()
        {
            if (_session.CurrentStructure == null)
            {
                return "<noname.nbt>";
            }

            var fileName = _session.CurrentStructure.FileName;
            return _session.HasChanges ? "* " + fileName : fileName;
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_session.HasChanges))
            {
                RefreshRootName();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
