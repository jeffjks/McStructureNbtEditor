using fNbt;

namespace McStructureNbtEditor.Models
{
    public class StructureBlock
    {
        public BlockPosition BlockPos { get; set; }

        public int State { get; set; }
        public NbtTag? Tag { get; set; }
        public NbtCompound? Nbt { get; set; }
    }
}