using fNbt;

// .nbt 파일 읽기/쓰기
// fNbt와 직접 맞닿는 계층

namespace McStructureNbtEditor.Services
{
    public class NbtFileService
    {
        private readonly ISettingsService _settings;

        public NbtFileService(ISettingsService settings)
        {
            _settings = settings;
        }

        public NbtFile Load(string path)
        {
            var file = new NbtFile();
            file.LoadFromFile(path, NbtCompression.AutoDetect, null);
            return file;
        }

        public void Save(NbtFile file, string path)
        {
            file.SaveToFile(path, _settings.SaveCompression);
        }
    }
}