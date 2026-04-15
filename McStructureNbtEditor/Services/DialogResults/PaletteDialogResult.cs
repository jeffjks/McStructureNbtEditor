using McStructureNbtEditor.Models;

namespace McStructureNbtEditor.Services.DialogResults
{
    public sealed class PaletteDialogResult
    {
        public bool Confirmed { get; init; }
        public PaletteEntryDraft? Draft { get; set; }
    }
}
