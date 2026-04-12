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
        private readonly EditorSession _session;
        private readonly SliceBuilder _sliceBuilder = new();

        private StructureFileModel? _structure;
        private int _minY;
        private int _maxY;
        private int _currentY;
        private string _currentYText = "0";
        private string _selectionText = "";

        private BlockCellModel? _dragStartCell;
        private BlockCellModel? _dragCurrentCell;
        private bool _isDragging;

        private readonly HashSet<BlockCellModel> _selectionSnapshot = new();
        private bool _dragSelectionStarted;
        private bool _dragCtrlMode;
        private bool _dragShiftMode;

        public ObservableCollection<BlockCellModel> SliceCells { get; } = new();
        public ObservableCollection<BlockCellModel> SelectedCells { get; } = new();

        public bool IsDragging
        {
            get => _isDragging;
            private set
            {
                if (_isDragging == value)
                    return;

                _isDragging = value;
                OnPropertyChanged();
            }
        }

        public string MinLayerLabel { get; set; } = "0";
        public string MaxLayerLabel { get; set; } = "0";

        public int MinY
        {
            get => _minY;
            set
            {
                if (_minY == value) return;
                _minY = value;
                MinLayerLabel = value.ToString();
                OnPropertyChanged();
                OnPropertyChanged(nameof(MinLayerLabel));
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
                OnPropertyChanged(nameof(MaxLayerLabel));
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

        public string SelectionText
        {
            get => _selectionText;
            set
            {
                if (_selectionText == value) return;
                _selectionText = value;
                OnPropertyChanged();
            }
        }

        public int SliceWidth => _structure?.SizeX ?? 0;
        public int SliceHeight => _structure?.SizeZ ?? 0;

        public RelayCommand ApplyYTextCommand { get; }
        public RelayCommand IncreaseYCommand { get; }
        public RelayCommand DecreaseYCommand { get; }

        public LayerSliceViewModel(EditorSession session)
        {
            _session = session;
            ApplyYTextCommand = new RelayCommand(ApplyYFromText, () => _structure != null);
            IncreaseYCommand = new RelayCommand(() => CurrentY += 1, () => _structure != null && CurrentY < MaxY);
            DecreaseYCommand = new RelayCommand(() => CurrentY -= 1, () => _structure != null && CurrentY > MinY);

            SelectedCells.CollectionChanged += (s, e) => UpdateSingleSelection();
            _session.PropertyChanged += OnSessionPropertyChanged;
            _session.DocumentChanged += OnDocumentChanged;
        }

        public void LoadStructure(StructureFileModel? structure)
        {
            bool isNewStructure = _structure != structure;

            if (isNewStructure)
                _structure = structure;

            if (_structure == null || _structure.SizeY <= 0)
            {
                SliceCells.Clear();
                ClearSelection();
                MinY = 0;
                MaxY = 0;
                _currentY = 0;
                OnPropertyChanged(nameof(CurrentY));
                CurrentYText = "0";
                _session.StatusMessage = "구조물 없음";
                NotifyLayoutChanged();
                RaiseCommands();
                return;
            }

            if (isNewStructure) {
                MinY = 0;
                MaxY = _structure.SizeY - 1;

                _currentY = 0;
                OnPropertyChanged(nameof(CurrentY));
                CurrentYText = "0";

                RebuildSlice();
            }

            _session.StatusMessage = $"구조물 로드됨. Y 범위: {MinY}~{MaxY}";
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
                _session.StatusMessage = "Y 값은 정수여야 합니다.";
                return;
            }

            CurrentY = parsed;
            _session.StatusMessage = $"Y 높이 변경: {CurrentY}";
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
            ClearSelection();

            if (_structure != null)
            {
                var cells = _sliceBuilder.BuildSlice(_structure, CurrentY);
                foreach (var cell in cells)
                    SliceCells.Add(cell);
            }

            RaiseCommands();
        }

        public bool TrySelectCellAt(BlockPosition blockPosition)
        {
            if (_structure == null)
                return false;

            CurrentY = blockPosition.Y;

            var cell = SliceCells.FirstOrDefault(c => c.BlockPos == blockPosition);
            if (cell == null)
                return false;

            SelectSingle(cell);
            return true;
        }

        // Selection ==========================================================================
        private void AddToSelection(BlockCellModel cell)
        {
            if (cell == null || cell.IsSelected)
                return;

            cell.IsSelected = true;
            if (!SelectedCells.Contains(cell))
                SelectedCells.Add(cell);
        }

        private void UpdateSingleSelection()
        {
            var selectionCount = SelectedCells.Count;
            if (selectionCount == 1)
            {
                var cell = SelectedCells[0];
                _session.SelectedInspectable = cell;
                
                if (cell.PaletteIndex == -1)
                    SelectionText = $"현재 선택: 없음";
                else
                    SelectionText = $"현재 선택: [{cell.PaletteIndex}] {cell.BlockName}";
            }
            else
            {
                _session.SelectedInspectable = null;

                if (selectionCount == 0)
                    SelectionText = $"현재 선택: 없음";
                else
                    SelectionText = $"현재 선택: {selectionCount}개 셀 선택함";
            }
        }

        private void RemoveFromSelection(BlockCellModel cell)
        {
            if (cell == null || !cell.IsSelected)
                return;

            cell.IsSelected = false;
            SelectedCells.Remove(cell);
        }

        public void SelectSingle(BlockCellModel cell)
        {
            if (cell == null)
                return;

            ClearSelectionInternal();
            AddToSelection(cell);
        }

        public void ToggleSelection(BlockCellModel cell)
        {
            if (cell == null)
                return;

            if (cell.IsSelected)
                RemoveFromSelection(cell);
            else
                AddToSelection(cell);
        }

        private List<BlockCellModel> GetRectangleCells(BlockCellModel start, BlockCellModel end)
        {
            int minX = Math.Min(start.BlockPos.X, end.BlockPos.X);
            int maxX = Math.Max(start.BlockPos.X, end.BlockPos.X);
            int minZ = Math.Min(start.BlockPos.Z, end.BlockPos.Z);
            int maxZ = Math.Max(start.BlockPos.Z, end.BlockPos.Z);

            return SliceCells
                .Where(cell =>
                    cell.BlockPos.X >= minX && cell.BlockPos.X <= maxX &&
                    cell.BlockPos.Z >= minZ && cell.BlockPos.Z <= maxZ)
                .ToList();
        }

        public void SelectRectangle(BlockCellModel start, BlockCellModel end)
        {
            if (start == null || end == null)
                return;

            var rectCells = GetRectangleCells(start, end);

            ClearSelectionInternal();

            foreach (var cell in rectCells)
                AddToSelection(cell);
        }

        public void AddRectangleSelection(BlockCellModel start, BlockCellModel end)
        {
            if (start == null || end == null)
                return;

            var rectCells = GetRectangleCells(start, end);

            foreach (var cell in rectCells)
                AddToSelection(cell);
        }

        public void ToggleRectangleSelection(BlockCellModel start, BlockCellModel end)
        {
            if (start == null || end == null)
                return;

            var rectCells = GetRectangleCells(start, end);
            var rectSet = rectCells.ToHashSet();

            foreach (var cell in SliceCells)
            {
                bool wasSelected = _selectionSnapshot.Contains(cell);
                bool inRect = rectSet.Contains(cell);

                bool shouldSelect = wasSelected;

                if (inRect)
                    shouldSelect = !wasSelected;

                if (shouldSelect)
                    AddToSelection(cell);
                else
                    RemoveFromSelection(cell);
            }
        }

        public void BeginSelection(BlockCellModel cell, bool isCtrlPressed, bool isShiftPressed)
        {
            if (cell == null)
                return;

            _dragStartCell = cell;
            _dragCurrentCell = cell;

            _dragSelectionStarted = false;
            _dragCtrlMode = isCtrlPressed;
            _dragShiftMode = isShiftPressed;

            _selectionSnapshot.Clear();
            foreach (var selected in SelectedCells)
                _selectionSnapshot.Add(selected);

            IsDragging = true;
        }

        public void UpdateSelectionDrag(BlockCellModel cell, bool isCtrlPressed, bool isShiftPressed)
        {
            if (!IsDragging || cell == null || _dragStartCell == null)
                return;

            if (ReferenceEquals(_dragCurrentCell, cell))
                return;

            _dragCurrentCell = cell;
            _dragSelectionStarted = true;

            if (_dragCtrlMode)
            {
                ToggleRectangleSelection(_dragStartCell, cell);
            }
            else if (_dragShiftMode)
            {
                ClearSelectionInternal();

                foreach (var selected in _selectionSnapshot)
                    AddToSelection(selected);

                AddRectangleSelection(_dragStartCell, cell);
            }
            else
            {
                SelectRectangle(_dragStartCell, cell);
            }
        }

        public void EndSelection()
        {
            if (!IsDragging || _dragStartCell == null)
            {
                IsDragging = false;
                _dragStartCell = null;
                _dragCurrentCell = null;
                _selectionSnapshot.Clear();
                return;
            }

            // 드래그 없이 클릭만 한 경우 처리
            if (!_dragSelectionStarted)
            {
                if (_dragCtrlMode)
                {
                    ToggleSelection(_dragStartCell);
                }
                else if (_dragShiftMode)
                {
                    AddToSelection(_dragStartCell);
                }
                else
                {
                    SelectSingle(_dragStartCell);
                }
            }

            IsDragging = false;
            _dragStartCell = null;
            _dragCurrentCell = null;
            _selectionSnapshot.Clear();
        }

        public void ClearSelection()
        {
            ClearSelectionInternal();
        }

        private void ClearSelectionInternal()
        {
            foreach (var cell in SelectedCells.ToList())
                cell.IsSelected = false;

            SelectedCells.Clear();
        }
        // ======================================================================================

        private void NotifyLayoutChanged()
        {
            OnPropertyChanged(nameof(SliceWidth));
            OnPropertyChanged(nameof(SliceHeight));
        }

        private void RaiseCommands()
        {
            ApplyYTextCommand.RaiseCanExecuteChanged();
            IncreaseYCommand.RaiseCanExecuteChanged();
            DecreaseYCommand.RaiseCanExecuteChanged();
        }

        private void OnDocumentChanged(object? sender, EventArgs e)
        {
            Reload();
        }

        private void Reload()
        {
            var structure = _session.CurrentStructure;
            LoadStructure(structure);
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_session.RequestedCellSelection))
            {
                var pos = _session.RequestedCellSelection;

                bool ok = TrySelectCellAt(pos);

                _session.StatusMessage = ok
                    ? $"블록으로 이동: ({pos})"
                    : $"블록을 찾을 수 없음: ({pos})";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}