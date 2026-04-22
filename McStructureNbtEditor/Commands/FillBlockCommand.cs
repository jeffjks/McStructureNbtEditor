using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor.Commands
{
    public sealed class FillBlockCommand : IEditorCommand
    {
        private readonly PaletteEntry _paletteEntry;
        private readonly IReadOnlySet<SelectedCell> _filledCells;
        private readonly Dictionary<BlockPosition, StructureBlock?> _previousBlocks = new();
        private bool _snapshotCaptured = false;

        public string CommandStatusMessage => $"블럭 채우기: {_paletteEntry.Name}. 채워진 블록: {_filledCells.Count}개";

        public ReloadScope ChangeType => ReloadScope.ReloadBlock;

        public FillBlockCommand(IReadOnlySet<SelectedCell> selectedCells, PaletteEntry paletteEntry)
        {
            _filledCells = new HashSet<SelectedCell>(selectedCells);
            _paletteEntry = new PaletteEntry(paletteEntry);
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return false;

            var paletteIndex = _paletteEntry.Index;
            if (paletteIndex < 0 || structure.Palette.Count <= paletteIndex)
                return false;

            foreach (var cell in _filledCells)
            {
                if (cell.HasBlock)
                {
                    int blockIndex = cell.BlockIndex!.Value;
                    if (blockIndex < 0 || blockIndex >= structure.Blocks.Count)
                        return false;
                    if (!structure.Blocks[blockIndex].BlockPos.Equals(cell.Position))
                        return false;
                }
            }

            foreach (var cell in _filledCells)
            {
                if (cell.HasBlock)
                {
                    int blockIndex = cell.BlockIndex!.Value;
                    if (!_snapshotCaptured)
                        _previousBlocks.TryAdd(cell.Position, new StructureBlock(structure.Blocks[blockIndex]));
                    structure.Blocks[blockIndex].SetState(paletteIndex);
                }
                else
                {
                    var newBlockIndex = structure.Blocks.Count;
                    var newBlock = new StructureBlock(newBlockIndex, cell.Position, paletteIndex);
                    if (!_snapshotCaptured)
                    {
                        _previousBlocks.TryAdd(cell.Position, null);
                    }
                    structure.Blocks.Add(newBlock);
                }
            }       
            
            structure.ReindexBlocks();
            session.StructureInfo?.SetBlocksCount(structure.Blocks.Count);
            _snapshotCaptured = true;

            return true;
        }

        public void Undo(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return;

            var currentBlockMap = new Dictionary<BlockPosition, StructureBlock>();
            foreach (var block in structure.Blocks)
            {
                currentBlockMap.TryAdd(block.BlockPos, block);
            }

            foreach (var (blockPos, previousBlock) in _previousBlocks)
            {
                if (previousBlock == null)
                {
                    if (currentBlockMap.TryGetValue(blockPos, out var currentBlock))
                    {
                        structure.Blocks.Remove(currentBlock);
                    }
                }
                else
                {
                    if (currentBlockMap.TryGetValue(blockPos, out var currentBlock))
                    {
                        currentBlock.SetState(previousBlock.State, previousBlock.Nbt);
                    }
                    else
                    {
                        var newBlockIndex = structure.Blocks.Count;
                        var newBlock = new StructureBlock(newBlockIndex, previousBlock.BlockPos, previousBlock.State, previousBlock.Nbt);
                        structure.Blocks.Add(newBlock);
                    }
                }
            }

            structure.ReindexBlocks();
            session.StructureInfo?.SetBlocksCount(structure.Blocks.Count);
        }
    }
}
