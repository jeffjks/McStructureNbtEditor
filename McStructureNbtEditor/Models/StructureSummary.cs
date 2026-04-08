// 우측 패널에 보여줄 요약 정보
// 파일 경로, 크기, 블록 수, 팔레트 수, 엔티티 수 등

namespace McStructureNbtEditor.Models
{
    public class StructureSummary
    {
        public string FilePath { get; set; } = "";
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }

        public int PaletteCount { get; set; }
        public int BlocksCount { get; set; }
        public int EntitiesCount { get; set; }

        public string Description =>
            $"파일: {FilePath}\n" +
            $"크기: {SizeX} x {SizeY} x {SizeZ}\n" +
            $"Palette 개수: {PaletteCount}\n" +
            $"Blocks 개수: {BlocksCount}\n" +
            $"Entities 개수: {EntitiesCount}";
    }
}