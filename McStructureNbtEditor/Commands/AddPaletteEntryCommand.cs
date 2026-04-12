using fNbt;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public sealed class AddPaletteEntryCommand : IEditorCommand
    {
        private PaletteEntry _createdEntry;
        private PaletteEntry? _previousSelection;

        public AddPaletteEntryCommand(PaletteEntry paletteEntry)
        {
            _createdEntry = paletteEntry;
        }

        public string Description => $"팔레트 추가: {_createdEntry.Name}";

        public bool CanExecute(EditorSession session)
        {
            if (session.CurrentStructure == null)
                return false;

            return !string.IsNullOrWhiteSpace(_createdEntry.Name);
        }

        public void Execute(EditorSession session)
        {
            var structure = session.CurrentStructure!;

            structure.Palette.Add(_createdEntry);
            RefreshPaletteIndices(structure);

            session.SelectedPaletteEntry = _createdEntry;
            session.RaiseDocumentChanged();
        }

        public void Undo(EditorSession session)
        {
            if (_createdEntry == null || session.CurrentStructure == null)
                return;

            var structure = session.CurrentStructure!;
            session.CurrentStructure.Palette.Remove(_createdEntry);
            RefreshPaletteIndices(structure);

            session.SelectedPaletteEntry = _previousSelection;
            session.RaiseDocumentChanged();
        }

        private static void RefreshPaletteIndices(StructureFileModel structure)
        {
            for (int i = 0; i < structure.Palette.Count; i++)
            {
                structure.Palette[i].Index = i;
            }
        }
    }
}
