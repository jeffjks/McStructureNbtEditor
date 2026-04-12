using McStructureNbtEditor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace McStructureNbtEditor.Services.DialogResults
{
    public sealed class AddPaletteDialogResult
    {
        public bool Confirmed { get; init; }
        public PaletteEntryDraft? Draft { get; set; }
    }
}
