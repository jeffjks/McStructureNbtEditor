using fNbt;
using McStructureNbtEditor.Models;
using Microsoft.Win32;

namespace McStructureNbtEditor.Services
{
    public interface ISettingsService
    {
        NbtCompression SaveCompression { get; set; }
        AppLanguage CurrentLanguage { get; set; }
    }

    public class SettingsService : ISettingsService
    {
        private const string RegPath = @"Software\McStructureNbtEditor";
        public NbtCompression SaveCompression { get; set; } = NbtCompression.GZip;

        public AppLanguage CurrentLanguage
        {
            get
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegPath);
                return (AppLanguage)(key?.GetValue("Language") ?? (int)AppLanguage.English);
            }
            set
            {
                using var key = Registry.CurrentUser.CreateSubKey(RegPath);
                key.SetValue("Language", (int)value);
            }
        }
    }
}
