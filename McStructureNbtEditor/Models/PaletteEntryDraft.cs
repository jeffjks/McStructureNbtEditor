using System.Collections.ObjectModel;

// Palette 입력 상태를 담는 임시 객체

namespace McStructureNbtEditor.Models
{
    public class PaletteEntryDraft
    {
        public string Name { get; set; } = "";

        public ObservableCollection<PalettePropertiesItemDraft> Properties { get; } = new();
    }

    public class PalettePropertiesItemDraft
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
    }
}
