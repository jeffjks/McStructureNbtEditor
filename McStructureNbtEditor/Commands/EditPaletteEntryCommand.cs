using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public sealed class EditPaletteEntryCommand : IEditorCommand
    {
        private readonly PaletteEntry _beforeEntry;
        private readonly PaletteEntry _afterEntry;

        public string CommandStatusMessage => $"팔레트 수정: {_beforeEntry.Name} -> {_afterEntry.Name}";

        public ReloadScope ChangeType => ReloadScope.ReloadAll;

        public EditPaletteEntryCommand(PaletteEntry beforeEntry, PaletteEntry afterEntry)
        {
            _beforeEntry = beforeEntry;
            _afterEntry = new PaletteEntry(afterEntry);
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure;
            var paletteIndex = _beforeEntry.Index;

            if (structure == null)
                return false;
            if (paletteIndex < 0 || structure.Palette.Count <= paletteIndex)
                return false;

            structure.Palette[paletteIndex] = _afterEntry;
            session.SelectedPaletteEntry = _afterEntry;

            return true;
        }

        public void Undo(EditorSession session)
        {
            var structure = session.CurrentStructure;
            var paletteIndex = _beforeEntry.Index;

            if (structure == null || _beforeEntry == null)
                return;
            if (paletteIndex < 0 || structure.Palette.Count <= paletteIndex)
                return;

            structure.Palette[paletteIndex] = _beforeEntry;
        }
    }
}
