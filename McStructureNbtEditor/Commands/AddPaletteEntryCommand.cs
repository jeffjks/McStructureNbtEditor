using fNbt;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public sealed class AddPaletteEntryCommand : IEditorCommand
    {
        private readonly string _paletteName;
        private readonly string _rawProperties;
        private PaletteEntry? _createdEntry;
        private PaletteEntry? _previousSelection;

        public AddPaletteEntryCommand(string paletteName, string rawProperties)
        {
            _paletteName = paletteName;
            _rawProperties = rawProperties;
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

            if (_createdEntry == null)
            {
                var parsedProperties = _rawProperties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Split('='))
                    .Where(p => p.Length == 2)
                    .ToDictionary(
                        p => p[0].Trim(),
                        p => p[1].Trim()
                    );

                _createdEntry = new PaletteEntry
                {
                    Index = structure.Palette.Count,
                    Name = _paletteName,
                    Properties = new Dictionary<string, string>(parsedProperties)
                };
            }

            structure.Palette.Add(_createdEntry);
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
        private Dictionary<string, string> ParsePropertiesFromSnbt(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new Dictionary<string, string>();
            return new Dictionary<string, string>();
            /*
            try
            {
                // SNBT는 {}로 감싸야 compound로 파싱됨
                var wrapped = raw.Trim();

                if (!wrapped.StartsWith("{"))
                    wrapped = "{" + wrapped + "}";

                var compound = (NbtCompound)NbtTag.Parse(wrapped);

                return compound.Tags.ToDictionary(
                    tag => tag.Name,
                    tag => tag.ToString() // 값은 SNBT 문자열로 유지
                );
            }
            catch
            {
                // 파싱 실패 시 fallback (간단 파서)
                return ParseSimpleFallback(raw);
            }*/
        }
        private Dictionary<string, string> ParseSimpleFallback(string raw)
        {
            return raw
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split('=', 2))
                .Where(p => p.Length == 2)
                .ToDictionary(
                    p => p[0].Trim(),
                    p => p[1].Trim()
                );
        }
    }
}
