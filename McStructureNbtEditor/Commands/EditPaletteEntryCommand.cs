using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public sealed class EditPaletteEntryCommand : IEditorCommand
    {
        private readonly int _paletteIndex;
        private PaletteEntry? _beforeEntry;
        private readonly PaletteEntry _afterEntry;

        public string CommandStatusMessage => $"팔레트 수정: {_beforeEntry?.Name} -> {_afterEntry?.Name}";

        public ReloadScope ChangeType => ReloadScope.ReloadAll;

        public EditPaletteEntryCommand(int paletteIndex, PaletteEntry afterEntry)
        {
            _paletteIndex = paletteIndex;
            _afterEntry = new PaletteEntry(afterEntry);
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return false;
            if (_paletteIndex < 0 || structure.Palette.Count <= _paletteIndex)
                return false;

            _beforeEntry ??= new PaletteEntry(structure.Palette[_paletteIndex]);

            structure.Palette[_paletteIndex] = _afterEntry;
            session.SelectedPaletteEntry = _afterEntry;

            return true;
        }

        public void Undo(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null || _beforeEntry == null)
                return;
            if (_paletteIndex < 0 || structure.Palette.Count <= _paletteIndex)
                return;

            structure.Palette[_paletteIndex] = _beforeEntry;
        }
    }
}
