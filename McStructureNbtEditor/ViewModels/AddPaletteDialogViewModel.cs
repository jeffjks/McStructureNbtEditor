using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.ViewModels
{
    public class AddPaletteDialogViewModel : INotifyPropertyChanged
    {
        private string _paletteName = "";

        public string PaletteName
        {
            get => _paletteName;
            set
            {
                if (_paletteName == value)
                    return;

                _paletteName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
