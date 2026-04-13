using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using McStructureNbtEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace McStructureNbtEditor.Commands
{
    public sealed class FillBlockCommand : IEditorCommand
    {
        private readonly int _paletteIndex;
        private PaletteEntry? _paletteEntry;
        private readonly Dictionary<int, int> _previousBlockState = new();
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
                    _previousBlockState.TryAdd(blockIndex, structure.Blocks[blockIndex].State);
                    structure.Blocks[blockIndex].State = _paletteIndex;
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

            foreach (var (blockIndex, previousState) in _previousBlockState)
            {
                if (blockIndex >= 0 && blockIndex < structure.Blocks.Count)
                {
                    structure.Blocks[blockIndex].State = previousState;
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
