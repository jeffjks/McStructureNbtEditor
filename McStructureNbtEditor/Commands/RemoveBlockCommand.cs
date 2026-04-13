using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor.Commands
{
    public sealed class RemoveBlockCommand : IEditorCommand
    {
        private readonly IReadOnlySet<SelectedCell> _removedCells;
        private List<(StructureBlock Block, int Index)> _removedBlocks = new();

        public string CommandStatusMessage => $"블럭 {_removedCells.Count}개 삭제";

        public ReloadScope ChangeType => ReloadScope.ReloadBlock;

        public RemoveBlockCommand(IReadOnlySet<SelectedCell> cells)
        {
            _removedCells = cells;
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return false;
            if (_removedCells.Any(i => i.BlockIndex < 0 || i.BlockIndex >= structure.Blocks.Count))
                return false;

            var removedBlockIndexSet = _removedCells
                .Where(cell => cell.HasBlock)
                .Select(cell => cell.BlockIndex!.Value)
                .ToHashSet();

            _removedBlocks = structure.Blocks
                .Select((block, index) => (Block: block, Index: index))
                .Where(x => removedBlockIndexSet.Contains(x.Index))
                .ToList();

            foreach (var index in removedBlockIndexSet.OrderByDescending(x => x))
            {
                structure.Blocks.RemoveAt(index);
            }
            structure.ReindexBlocks();

            return true;
        }

        public void Undo(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return;

            foreach (var item in _removedBlocks.OrderBy(x => x.Index))
            {
                structure.Blocks.Insert(item.Index, item.Block);
            }
            structure.ReindexBlocks();
        }
    }
}
