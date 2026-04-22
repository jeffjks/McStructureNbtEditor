using fNbt;
using System.Collections.ObjectModel;

namespace McStructureNbtEditor.Models
{
    public class StructureFileModel
    {
        public string FileName { get; private set; } = "";
        public string FilePath { get; private set; } = "";

        public int SizeX { get; private set; }
        public int SizeY { get; private set; }
        public int SizeZ { get; private set; }

        public ObservableCollection<NbtTag> Entities { get; private set; } = new();
        public ObservableCollection<PaletteEntry> Palette { get; private set; } = new();
        public ObservableCollection<StructureBlock> Blocks { get; private set; } = new();

        public int DataVersion { get; private set; }

        public bool IsNewFile => string.IsNullOrWhiteSpace(FilePath);

        public static StructureFileModel CreateNew(StructureSize size, int dataVersion)
        {
            return new StructureFileModel("", "", size, dataVersion);
        }

        public static StructureFileModel OpenFromFile(string fileName, string filePath, StructureSize size, int dataVersion)
        {
            return new StructureFileModel(fileName, filePath, size, dataVersion);
        }

        private StructureFileModel(string fileName, string filePath, StructureSize size, int dataVersion)
        {
            FileName = fileName;
            FilePath = filePath;
            SizeX = size.X;
            SizeY = size.Y;
            SizeZ = size.Z;
            DataVersion = dataVersion;
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