using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace McStructureNbtEditor.ViewModels
{
    public class ChangeLanguageViewModel : INotifyPropertyChanged
    {
        private readonly ISettingsService _settingService;
        public bool IsKoreanSelected => _settingService.CurrentLanguage == AppLanguage.Korean;
        public bool IsEnglishSelected => _settingService.CurrentLanguage == AppLanguage.English;

        public RelayCommand<AppLanguage> ChangeLanguageCommand { get; }

        public ChangeLanguageViewModel(ISettingsService settingService)
        {
            _settingService = settingService;

            ChangeLanguageCommand = new RelayCommand<AppLanguage>(ChangeLanguage);

            ChangeLanguage(_settingService.CurrentLanguage);
        }

        private void ChangeLanguage(AppLanguage lang)
        {
            string fileName = lang switch
            {
                AppLanguage.English => "en-US",
                AppLanguage.Korean => "ko-KR",
                _ => "en-US"
            };

            var dict = new ResourceDictionary();
            dict.Source = new Uri($"Resources/Lang/{fileName}.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);

            _settingService.CurrentLanguage = lang;

            OnPropertyChanged(nameof(IsKoreanSelected));
            OnPropertyChanged(nameof(IsEnglishSelected));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
