namespace McStructureNbtEditor.Models
{
    public class PaletteEntry
    {
        public int Index { get; set; }
        public string Name { get; set; } = "";
        public Dictionary<string, string> Properties { get; set; } = new();

        public string DisplayName
        {
            get
            {
                if (Properties.Count == 0)
                    return Name;

                return $"{Name} [{string.Join(", ", Properties.Select(kv => $"{kv.Key}={kv.Value}"))}]";
            }
        }
    }
}