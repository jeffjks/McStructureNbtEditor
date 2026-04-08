namespace McStructureNbtEditor.Models
{
    public class StructureBlock
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int State { get; set; }

        // 블록 엔티티 nbt가 있으면 나중에 확장용으로 보관 가능
        public bool HasBlockEntityData { get; set; }
    }
}