using fNbt;
using McStructureNbtEditor.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.Models
{
    public class BlockCellModel : INotifyPropertyChanged, ISnbtInspectable
    {
        private string _blockName = "";
        private string _displayText = "";
        private string _tooltipBlockNameText = "";
        private bool _isOccupied;
        private int _state = -1;
        private bool _isSelected = false;

        private readonly NbtCompound _compound;
        private readonly NbtInt _compoundState;
        private readonly NbtList _compoundPosList;
        private readonly NbtInt _compoundPosX;
        private readonly NbtInt _compoundPosY;
        private readonly NbtInt _compoundPosZ;

        public BlockPosition BlockPos { get; set; }
        public NbtTag? Tag { get; set; }


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

        // PaletteIndex
        public int State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public BlockCellModel()
        {
            _compound = new NbtCompound("");

            _compoundState = new NbtInt("state", 0);

            _compoundPosX = new NbtInt(0);
            _compoundPosY = new NbtInt(0);
            _compoundPosZ = new NbtInt(0);

            _compoundPosList = new NbtList("pos")
            {
                _compoundPosX,
                _compoundPosY,
                _compoundPosZ
            };

            _compound.Add(_compoundState);
            _compound.Add(_compoundPosList);
        }

        public string GetSnbtText()
        {
            _compoundState.Value = _state;
            _compoundPosX.Value = BlockPos.X;
            _compoundPosY.Value = BlockPos.Y;
            _compoundPosZ.Value = BlockPos.Z;

            if (_compound.Contains("nbt"))
            {
                _compound.Remove("nbt");
            }

            if (Tag != null)
            {
                Tag.Name = "nbt";
                _compound.Add(Tag);
            }

            return NbtSnbtConverter.ToSnbt(_compound);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}