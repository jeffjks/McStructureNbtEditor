using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.Models
{
    public class BlockCellModel : INotifyPropertyChanged
    {
        private string _blockName = "";
        private string _displayText = "";
        private string _tooltipBlockNameText = "";
        private bool _isOccupied;
        private int _paletteIndex = -1;

        public int X { get; set; }
        public int Z { get; set; }
        public int Y { get; set; }

        public string BlockName
        {
            get => _blockName;
            set
            {
                _blockName = value;
                OnPropertyChanged();
            }
        }

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public string TooltipBlockNameText
        {
            get => _tooltipBlockNameText;
            set
            {
                _tooltipBlockNameText = value;
                OnPropertyChanged();
            }
        }

        public bool IsOccupied
        {
            get => _isOccupied;
            set
            {
                _isOccupied = value;
                OnPropertyChanged();
            }
        }

        public int PaletteIndex
        {
            get => _paletteIndex;
            set
            {
                _paletteIndex = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}