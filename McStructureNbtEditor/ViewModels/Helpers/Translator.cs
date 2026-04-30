using System.Windows;

namespace McStructureNbtEditor.ViewModels.Helpers
{
    public static class Translator
    {
        public static string GetTranslation(string key)
        {
            var resource = Application.Current.TryFindResource(key);
            return resource as string ?? $"[Missing String: {key}]";
        }
        public static string GetTranslation(string key, params object?[] args)
        {
            string format = GetTranslation(key);
            return string.Format(format, args);
        }
    }
}
