using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor.Commands
{
    public sealed class FillBlockCommand : IEditorCommand
    {
        private readonly int _paletteIndex;
        private PaletteEntry? _paletteEntry;
        private readonly Dictionary<int, StructureBlock> _previousBlocks = new();
        private readonly IReadOnlySet<SelectedCell> _filledCells;
        private readonly List<StructureBlock> _createdBlocks = new();

        public string CommandStatusMessage => $"블럭 채우기: {_paletteEntry?.Name}. 채워진 블록: {_filledCells.Count}개";

        public ReloadScope ChangeType => ReloadScope.ReloadBlock;

        public FillBlockCommand(IReadOnlySet<SelectedCell> selectedCells, int paletteIndex)
        {
            _filledCells = selectedCells;
            _paletteIndex = paletteIndex;
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return false;
            if (_paletteIndex < 0 || structure.Palette.Count <= _paletteIndex)
                return false;

            foreach (var cell in _filledCells)
            {
                if (cell.HasBlock)
                {
                    int blockIndex = cell.BlockIndex!.Value;
                    if (blockIndex < 0 || blockIndex >= structure.Blocks.Count)
                        return false;
                }
            }
            _paletteEntry = structure.Palette[_paletteIndex];

            foreach (var cell in _filledCells)
            {
                if (cell.HasBlock)
                {
                    int blockIndex = cell.BlockIndex!.Value;
                    _previousBlocks.TryAdd(blockIndex, structure.Blocks[blockIndex]);
                    structure.Blocks[blockIndex] = new StructureBlock(blockIndex, cell.Position, _paletteIndex);
                }
                else
                {
                    var newBlockIndex = structure.Blocks.Count;
                    var newBlock = new StructureBlock(newBlockIndex, cell.Position, _paletteIndex);
                    _createdBlocks.Add(newBlock);
                    structure.Blocks.Add(newBlock);
                }
            }
            structure.ReindexBlocks();

            return true;
        }

        public void Undo(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return;

            foreach (var (blockIndex, previousBlock) in _previousBlocks)
            {
                if (blockIndex >= 0 && blockIndex < structure.Blocks.Count)
                {
                    structure.Blocks[blockIndex] = previousBlock;
                }
            }

            foreach (var block in _createdBlocks)
            {
                structure.Blocks.Remove(block);
            }

            structure.ReindexBlocks();
        }
    }
}
