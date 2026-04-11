using fNbt;
using System.Collections.ObjectModel;

namespace McStructureNbtEditor.Models
{
    public class StructureFileModel
    {
        public string FileName { get; set; } = "<noname>.nbt";
        public string FilePath { get; set; } = "";

        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        public ObservableCollection<NbtTag> Entities { get; set; } = new();
        public ObservableCollection<PaletteEntry> Palette { get; set; } = new();
        public ObservableCollection<StructureBlock> Blocks { get; set; } = new();

        public int DataVersion { get; set; }

        public PaletteEntry? GetPaletteEntry(int index)
        {
            if (index < 0 || index >= Palette.Count)
                return null;

            return Palette[index];
        }
    }
}