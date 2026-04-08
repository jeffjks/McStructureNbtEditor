using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.ViewModels
{
    public class LayerSliceViewModel : INotifyPropertyChanged
    {
        private readonly SliceBuilder _sliceBuilder = new();

        private StructureFileModel? _structure;
        private int _minY;
        private int _maxY;
        private int _currentY;
        private string _currentYText = "0";
        private string _statusText = "";

        public ObservableCollection<BlockCellModel> SliceCells { get; } = new();
        public string MinLayerLabel { get; set; } = "-";
        public string MaxLayerLabel { get; set; } = "-";

        public int MinY
        {
            get => _minY;
            set
            {
                if (_minY == value) return;
                _minY = value;
                MinLayerLabel = value.ToString();
                OnPropertyChanged();
            }
        }

        public int MaxY
        {
            get => _maxY;
            set
            {
                if (_maxY == value) return;
                _maxY = value;
                MaxLayerLabel = value.ToString();
                OnPropertyChanged();
            }
        }

        public int CurrentY
        {
            get => _currentY;
            set
            {
                var clamped = ClampY(value);
                if (_currentY == clamped) return;

                _currentY = clamped;
                OnPropertyChanged();

                CurrentYText = _currentY.ToString(CultureInfo.InvariantCulture);
                RebuildSlice();
            }
        }

        public string CurrentYText
        {
            get => _currentYText;
            set
            {
                if (_currentYText == value) return;
                _currentYText = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText == value) return;
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public int SliceWidth => _structure?.SizeX ?? 0;
        public int SliceHeight => _structure?.SizeZ ?? 0;

        public string SliceInfoText =>
            _structure == null
                ? "단면 정보 없음"
                : $"Y={CurrentY} / 범위 {MinY}~{MaxY} / Size X={SliceWidth}, Z={SliceHeight}";

        public RelayCommand ApplyYTextCommand { get; }
        public RelayCommand IncreaseYCommand { get; }
        public RelayCommand DecreaseYCommand { get; }

        public LayerSliceViewModel()
        {
            ApplyYTextCommand = new RelayCommand(ApplyYFromText, () => _structure != null);
            IncreaseYCommand = new RelayCommand(() => CurrentY += 1, () => _structure != null && CurrentY < MaxY);
            DecreaseYCommand = new RelayCommand(() => CurrentY -= 1, () => _structure != null && CurrentY > MinY);
        }

        public void LoadStructure(StructureFileModel? structure)
        {
            _structure = structure;

            SliceCells.Clear();

            if (_structure == null || _structure.SizeY <= 0)
            {
                MinY = 0;
                MaxY = 0;
                _currentY = 0;
                OnPropertyChanged(nameof(CurrentY));
                CurrentYText = "0";
                StatusText = "구조물 없음";
                NotifyLayoutChanged();
                RaiseCommands();
                return;
            }

            MinY = 0;
            MaxY = _structure.SizeY - 1;

            _currentY = 0;
            OnPropertyChanged(nameof(CurrentY));
            CurrentYText = "0";

            RebuildSlice();

            StatusText = $"구조물 로드됨. Y 범위: {MinY}~{MaxY}";
            NotifyLayoutChanged();
            RaiseCommands();
        }

        public void ApplyYFromText()
        {
            if (_structure == null)
                return;

            if (!int.TryParse(CurrentYText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                CurrentYText = CurrentY.ToString(CultureInfo.InvariantCulture);
                StatusText = "Y 값은 정수여야 합니다.";
                return;
            }

            CurrentY = parsed;
            StatusText = $"Y 층 변경: {CurrentY}";
        }

        private int ClampY(int y)
        {
            if (_structure == null || _structure.SizeY <= 0)
                return 0;

            if (y < MinY) return MinY;
            if (y > MaxY) return MaxY;
            return y;
        }

        private void RebuildSlice()
        {
            SliceCells.Clear();

            if (_structure != null)
            {
                var cells = _sliceBuilder.BuildSlice(_structure, CurrentY);
                foreach (var cell in cells)
                    SliceCells.Add(cell);
            }

            OnPropertyChanged(nameof(SliceInfoText));
            RaiseCommands();
        }

        private void NotifyLayoutChanged()
        {
            OnPropertyChanged(nameof(SliceWidth));
            OnPropertyChanged(nameof(SliceHeight));
            OnPropertyChanged(nameof(SliceInfoText));
        }

        private void RaiseCommands()
        {
            ApplyYTextCommand.RaiseCanExecuteChanged();
            IncreaseYCommand.RaiseCanExecuteChanged();
            DecreaseYCommand.RaiseCanExecuteChanged();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}