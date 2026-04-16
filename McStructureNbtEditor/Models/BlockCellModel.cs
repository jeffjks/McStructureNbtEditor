using fNbt;
using McStructureNbtEditor.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.Models
{
    public class BlockCellModel : INotifyPropertyChanged, ISnbtInspectable
    {
        private string? _tooltipCache;
        private BlockPosition _blockPos;
        private string _blockName = "";
        private string _cellText = "";
        private bool _isEmpty;
        private int _blockIndex = -1;
        private int _state = -1;
        private bool _isSelected = false;

        private readonly NbtCompound _compound;
        private readonly NbtInt _compoundState;
        private readonly NbtList _compoundPosList;
        private readonly NbtInt _compoundPosX;
        private readonly NbtInt _compoundPosY;
        private readonly NbtInt _compoundPosZ;

        public NbtTag? Tag { get; set; }

        public string TooltipText
        {
            get
            {
                if (_tooltipCache == null)
                {
                    string blockname = _isEmpty ? "(No Block)" : _blockName;
                    if (_isEmpty)
                    {
                        _tooltipCache = $"(No Block)\nBlock Index: {_blockIndex}\npos: {_blockPos}";
                    }
                    else
                    {
                        _tooltipCache = $"[{_state}] {blockname}\nBlock Index: {_blockIndex}\npos: {_blockPos}";
                    }
                }
                return _tooltipCache;
            }
        }


        public BlockPosition BlockPos {
            get => _blockPos;
            set
            {
                _blockPos = value;
                OnPropertyChanged();
                OnTooltipChanged();
            }
        }

        public string BlockName
        {
            get => _blockName;
            set
            {
                _blockName = value;
                OnPropertyChanged();
                OnTooltipChanged();
            }
        }

        public string CellText
        {
            get => _cellText;
            set
            {
                _cellText = value;
                OnPropertyChanged();
            }
        }

        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                _isEmpty = value;
                OnPropertyChanged();
                OnTooltipChanged();
            }
        }
        public int BlockIndex
        {
            get => _blockIndex;
            set
            {
                _blockIndex = value;
                OnPropertyChanged();
                OnTooltipChanged();
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
                OnTooltipChanged();
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

            _compoundPosX = new NbtInt(0);
            _compoundPosY = new NbtInt(0);
            _compoundPosZ = new NbtInt(0);

            _compoundPosList = new NbtList("pos")
            {
                _compoundPosX,
                _compoundPosY,
                _compoundPosZ
            };

            _compoundState = new NbtInt("state", 0);

            _compound.Add(_compoundState);
            _compound.Add(_compoundPosList);
        }

        private void OnTooltipChanged()
        {
            _tooltipCache = null;
            OnPropertyChanged(nameof(TooltipText));
        }

        public string GetSnbtText()
        {
            if (_isEmpty)
                return string.Empty;

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
                var clonedTag = (NbtTag)Tag.Clone();
                Tag.Name = "nbt";
                _compound.Add(clonedTag);
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