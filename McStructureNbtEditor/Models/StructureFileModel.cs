namespace McStructureNbtEditor.Models
{
    public class StructureFileModel
    {
        public string FilePath { get; set; } = "";

        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        public List<PaletteEntry> Palette { get; set; } = new();
        public List<StructureBlock> Blocks { get; set; } = new();

        public PaletteEntry? GetPaletteEntry(int index)
        {
            if (index < 0 || index >= Palette.Count)
                return null;

            return Palette[index];
        }
    }
}