using McStructureNbtEditor.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace McStructureNbtEditor.Services
{
    public static class PaletteEntryFactory
    {
        public static PaletteEntry CreateFromDraft(PaletteEntryDraft draft, int index)
        {
            if (draft == null)
                throw new ArgumentNullException(nameof(draft));

            var properties = draft.Properties
                .Where(p => !string.IsNullOrWhiteSpace(p.Key))
                .ToDictionary(
                    p => p.Key.Trim(),
                    p => p.Value?.Trim() ?? "",
                    StringComparer.Ordinal);

            return new PaletteEntry
            {
                Index = index,
                Name = draft.Name.Trim(),
                Properties = properties
            };
        }
    }
}
