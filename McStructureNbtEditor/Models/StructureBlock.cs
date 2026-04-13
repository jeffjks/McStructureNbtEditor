using fNbt;

namespace McStructureNbtEditor.Models
{
    public class StructureBlock
    {
        public int Index;
        public BlockPosition BlockPos { get; set; }

        public int State { get; set; }
        //public NbtTag? Tag { get; set; }
        public NbtCompound? Nbt { get; set; }

        public StructureBlock(int index, BlockPosition blockPos, int state, NbtCompound? nbt = null)
        {
            Index = index;
            BlockPos = blockPos;
            State = state;
            Nbt = nbt;
        }
    }
}