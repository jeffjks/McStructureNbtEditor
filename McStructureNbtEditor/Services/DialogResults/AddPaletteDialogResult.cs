using System;
using System.Collections.Generic;
using System.Text;

namespace McStructureNbtEditor.Services.DialogResults
{
    public sealed class AddPaletteDialogResult
    {
        public bool Confirmed { get; init; }
        public string PaletteName { get; init; } = "";
    }
}
