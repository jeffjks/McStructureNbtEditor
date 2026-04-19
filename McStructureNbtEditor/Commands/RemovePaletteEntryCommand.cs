using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public class RemovePaletteEntryCommand : IEditorCommand
    {
        private readonly PaletteEntry _removedEntry;
        private List<(StructureBlock Block, int Index)> _removedBlocks = new();

        public string CommandStatusMessage => $"팔레트 삭제: {_removedEntry?.Name}. 제거된 블록 {_removedBlocks.Count}개.";
        public ReloadScope ChangeType => ReloadScope.ReloadAll;

        public RemovePaletteEntryCommand(PaletteEntry paletteEntry)
        {
            _removedEntry = new PaletteEntry(paletteEntry);
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return false;

            var paletteIndex = _removedEntry.Index;
            if (paletteIndex < 0 || structure.Palette.Count <= paletteIndex)
                return false;

            _removedBlocks = structure.Blocks
                .Select((block, index) => (Block: block, Index: index))
                .Where(x => x.Block.State == paletteIndex)
                .Select(x => (x.Block, x.Index))
                .ToList();

            foreach (var item in _removedBlocks.OrderByDescending(x => x.Index))
            {
                structure.Blocks.RemoveAt(item.Index);
            }

            structure.Palette.RemoveAt(paletteIndex);
            structure.ReindexPalette();

            foreach (var block in structure.Blocks)
            {
                if (block.State > paletteIndex)
                    block.State--;
            }
            structure.ReindexBlocks();

            // 삭제 후 SelectedPaletteEntry 처리
            if (structure.Palette.Count > 0)
            {
                int newIndex = Math.Min(paletteIndex, structure.Palette.Count - 1);
                session.SelectedPaletteEntry = structure.Palette[newIndex];
            }
            else
            {
                session.SelectedPaletteEntry = null;
            }
            return true;
        }

        public void Undo(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (_removedEntry == null || structure == null)
                return;

            var paletteIndex = _removedEntry.Index;
            if (paletteIndex < 0 || structure.Palette.Count < paletteIndex)
                return;

            foreach (var block in structure.Blocks)
            {
                if (block.State >= paletteIndex)
                    block.State++;
            }
            foreach (var item in _removedBlocks.OrderBy(x => x.Index))
            {
                structure.Blocks.Insert(item.Index, item.Block);
            }
            structure.ReindexBlocks();

            structure.Palette.Insert(paletteIndex, _removedEntry);
            structure.ReindexPalette();
        }
    }
}
