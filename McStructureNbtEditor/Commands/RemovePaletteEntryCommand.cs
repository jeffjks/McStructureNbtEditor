using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public class RemovePaletteEntryCommand : IEditorCommand
    {
        private int _paletteIndex;
        private PaletteEntry? _removedEntry;
        private List<(StructureBlock Block, int Index)> _removedBlocks = new();

        public string Description => $"팔레트 삭제됨: {_removedEntry?.Name}. 제거된 블록 {_removedBlocks.Count}개.";
        public ReloadScope ChangeType => ReloadScope.ReloadAll;

        public RemovePaletteEntryCommand(int paletteIndex)
        {
            _paletteIndex = paletteIndex;
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure!;

            if (structure == null)
                return false;
            if (_paletteIndex < 0 || _paletteIndex >= structure.Palette.Count)
                return false;

            _removedEntry = structure.Palette[_paletteIndex];

            _removedBlocks = structure.Blocks
                .Select((block, index) => new { block, index })
                .Where(x => x.block.State == _paletteIndex)
                .Select(x => (x.block, x.index))
                .ToList();

            foreach (var item in _removedBlocks.OrderByDescending(x => x.Index))
            {
                structure.Blocks.RemoveAt(item.Index);
            }

            structure.Palette.RemoveAt(_paletteIndex);

            foreach (var block in structure.Blocks)
            {
                if (block.State > _paletteIndex)
                    block.State--;
            }
            foreach (var palette in structure.Palette)
            {
                if (palette.Index > _paletteIndex)
                    palette.Index--;
            }

            // 삭제 후 SelectedPaletteEntry 처리
            if (structure.Palette.Count > 0)
            {
                int newIndex = Math.Min(_paletteIndex, structure.Palette.Count - 1);
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
            var structure = session.CurrentStructure!;

            if (_removedEntry == null || structure == null)
                return;

            structure.Palette.Insert(_paletteIndex, _removedEntry);
            foreach (var palette in structure.Palette)
            {
                if (palette.Index >= _paletteIndex)
                    palette.Index++;
            }
            foreach (var block in structure.Blocks)
            {
                if (block.State >= _paletteIndex)
                    block.State++;
            }
            foreach (var item in _removedBlocks.OrderBy(x => x.Index))
            {
                structure.Blocks.Insert(item.Index, item.Block);
            }
        }
    }
}
