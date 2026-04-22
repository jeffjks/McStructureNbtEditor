using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services.DialogResults;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace McStructureNbtEditor.ViewModels.Dialog
{
    public class NewFileDialogViewModel : INotifyPropertyChanged
    {
        private const int DefaultSize = 16;
        private const int MinSize = 1;
        private const int MaxSize = 48;

        private int _sizeX = DefaultSize, _sizeY = DefaultSize, _sizeZ = DefaultSize;
        public int SizeX
        {
            get => _sizeX;
            set
            {
                int clampedValue = Math.Max(MinSize, Math.Min(MaxSize, value));
                _sizeX = clampedValue;
                OnPropertyChanged();
            }
        }
        public int SizeY
        {
            get => _sizeY;
            set
            {
                int clampedValue = Math.Max(MinSize, Math.Min(MaxSize, value));
                _sizeY = clampedValue;
                OnPropertyChanged();
            }
        }
        public int SizeZ
        {
            get => _sizeZ;
            set
            {
                int clampedValue = Math.Max(MinSize, Math.Min(MaxSize, value));
                _sizeZ = clampedValue;
                OnPropertyChanged();
            }
        }

        private int _dataVersion;
        public int DataVersion
        {
            get => _dataVersion;
            set
            {
                int clampedValue = Math.Max(1, value);
                _dataVersion = clampedValue;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<bool?>? CloseAction { get; set; }

        public NewFileDialogResult Result => new NewFileDialogResult(new StructureSize(_sizeX, _sizeY, _sizeZ), _dataVersion);

        public NewFileDialogViewModel()
        {
            ConfirmCommand = new RelayCommand(() => CloseAction?.Invoke(true));
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke(false));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
