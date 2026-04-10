namespace McStructureNbtEditor.Models
{
    public readonly record struct BlockPosition(int X, int Y, int Z)
    {
        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}
