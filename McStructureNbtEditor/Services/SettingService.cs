using fNbt;
using McStructureNbtEditor.Models;

namespace McStructureNbtEditor.Services
{
    public interface ISettingsService
    {
        NbtCompression SaveCompression { get; set; }
        AppLanguage CurrentLanguage { get; set; }
    }

    public class SettingsService : ISettingsService
    {
        public NbtCompression SaveCompression { get; set; } = NbtCompression.GZip;
        public AppLanguage CurrentLanguage { get; set; } = AppLanguage.English;
    }
}
