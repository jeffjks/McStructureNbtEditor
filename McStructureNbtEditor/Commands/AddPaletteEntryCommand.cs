using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public sealed class AddPaletteEntryCommand : IEditorCommand
    {
        private readonly string _paletteName;
        private PaletteEntry? _createdEntry;
        private PaletteEntry? _previousSelection;

        public AddPaletteEntryCommand(string paletteName)
        {
            _paletteName = paletteName;
        }

        public string Description => $"팔레트 추가: {_paletteName}";

        public bool CanExecute(EditorSession session)
        {
            if (session.CurrentStructure == null)
                return false;

            return !string.IsNullOrWhiteSpace(_paletteName);
        }

        public void Execute(EditorSession session)
        {
            var structure = session.CurrentStructure!;
            _previousSelection = session.SelectedPaletteEntry;

            if (_createdEntry == null)
            {
                _createdEntry = new PaletteEntry
                {
                    Index = structure.Palette.Count(),
                    Name = _paletteName
                };
            }

            structure.Palette.Add(_createdEntry);
            session.SelectedPaletteEntry = _createdEntry;
            session.RaiseDocumentChanged();
        }

        public void Undo(EditorSession session)
        {
            if (_createdEntry == null || session.CurrentStructure == null)
                return;

            session.CurrentStructure.Palette.Remove(_createdEntry);
            session.SelectedPaletteEntry = _previousSelection;
            session.RaiseDocumentChanged();
        }
    }
}
