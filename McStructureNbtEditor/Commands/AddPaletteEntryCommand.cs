using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public sealed class AddPaletteEntryCommand : IEditorCommand
    {
        private readonly PaletteEntry _addedEntry;

        public string CommandStatusMessage => $"팔레트 추가: {_addedEntry.Name}.";

        public ReloadScope ChangeType => ReloadScope.ReloadAll;

        public AddPaletteEntryCommand(PaletteEntry paletteEntry)
        {
            _addedEntry = paletteEntry;
        }

        public bool Execute(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (structure == null)
                return false;

            if (string.IsNullOrWhiteSpace(_addedEntry.Name))
                return false;

            structure.Palette.Add(_addedEntry);

            return true;
        }

        public void Undo(EditorSession session)
        {
            var structure = session.CurrentStructure;

            if (_addedEntry == null || structure == null)
                return;

            structure.Palette.Remove(_addedEntry);
        }
    }
}
