using fNbt;
using System.Collections.ObjectModel;

namespace McStructureNbtEditor.Models
{
    public class StructureFileModel
    {
        public string FileName { get; private set; } = "";
        public string FilePath { get; private set; } = "";

        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        public ObservableCollection<NbtTag> Entities { get; set; } = new();
        public ObservableCollection<PaletteEntry> Palette { get; set; } = new();
        public ObservableCollection<StructureBlock> Blocks { get; set; } = new();

        public int DataVersion { get; set; }

        public bool IsNewFile => string.IsNullOrWhiteSpace(FilePath);

        public static StructureFileModel CreateNew(int sizeX, int sizeY, int sizeZ)
        {
            return new StructureFileModel("", "", sizeX, sizeY, sizeZ);
        }

        public static StructureFileModel OpenFromFile(string fileName, string filePath, int sizeX, int sizeY, int sizeZ)
        {
            return new StructureFileModel(fileName, filePath, sizeX, sizeY, sizeZ);
        }

        private StructureFileModel(string fileName, string filePath, int sizeX, int sizeY, int sizeZ)
        {
            FileName = fileName;
            FilePath = filePath;
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public void SetFileName(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }

        public PaletteEntry? GetPaletteEntry(int index)
        {
            if (index < 0 || index >= Palette.Count)
                return null;

            return Palette[index];
        }

        public void ReindexPalette()
        {
            for (int i = 0; i < Palette.Count; i++)
            {
                Palette[i].Index = i;
            }
        }

        public void ReindexBlocks()
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                Blocks[i].Index = i;
            }
        }
    }
}