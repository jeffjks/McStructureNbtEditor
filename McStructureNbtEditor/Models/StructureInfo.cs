// 우측 패널에 보여줄 요약 정보
// 파일 경로, 크기, 블록 수, 팔레트 수, 엔티티 수 등

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.Models
{
    public record StructureSize(int X, int Y, int Z);

    public record StructureInfoData(
        string FilePath,
        StructureSize Size,
        int EntitiesCount,
        int BlocksCount,
        int PaletteCount);

    public class StructureInfo : INotifyPropertyChanged
    {
        private readonly string _filePath = "";
        private StructureSize _size;
        private int _entitiesCount;
        private int _blocksCount;
        private int _paletteCount;

        public StructureInfo(StructureSize size)
        {
            _size = size;
        }

        public StructureInfo(StructureInfoData data)
        {
            _filePath = data.FilePath;
            _size = data.Size;
            _entitiesCount = data.EntitiesCount;
            _blocksCount = data.BlocksCount;
            _paletteCount = data.PaletteCount;
        }

        public void SetSize(StructureSize value)
        {
            if (_size == value)
                return;

            _size = value;
            OnPropertyChanged(nameof(DisplayText));
        }

        public void SetEntitiesCount(int value)
        {
            if (_entitiesCount == value) return;

            _entitiesCount = value;
            OnPropertyChanged(nameof(DisplayText));
        }

        public void SetBlocksCount(int value)
        {
            if (_blocksCount == value) return;

            _blocksCount = value;
            OnPropertyChanged(nameof(DisplayText));
        }

        public void SetPaletteCount(int value)
        {
            if (_paletteCount == value) return;

            _paletteCount = value;
            OnPropertyChanged(nameof(DisplayText));
        }

        public string DisplayText
        {
            get
            {
                var displayFilePath = string.IsNullOrEmpty(_filePath)
                   ? "<noname.nbt>"
                   : _filePath;
                return
                    $"Structure Size: {_size.X} x {_size.Y} x {_size.Z}\n" +
                    $"Total Palettes: {_paletteCount}\n" +
                    $"Total Blocks: {_blocksCount}\n" +
                    $"Total Entities: {_entitiesCount}\n" +
                    $"File: {displayFilePath}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}