using fNbt;

namespace McStructureNbtEditor.Services
{
    public interface ISettingsService
    {
        NbtCompression SaveCompression { get; set; }
        string LanguageCode { get; set; }
    }

    public class SettingsService : ISettingsService
    {
        public NbtCompression SaveCompression { get; set; } = NbtCompression.GZip;
        public string LanguageCode { get; set; } = "en-US";
    }
}
