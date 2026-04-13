using fNbt;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

// 열기/저장 명령, 현재 파일 상태, 트리 노드, 요약 정보

namespace McStructureNbtEditor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly NbtFileService _nbtFileService;
        private readonly StructureParser _structureParser = new();

        private StructureSummary? _summary;
        private NbtFile? _currentFile;
        private string _currentFileName = "";
        private string _currentFilePath = "";

        public EditorSession Session { get; }
        public LayerSliceViewModel LayerSlice { get; }
        public NbtTreeViewModel NbtTree { get; }
        public SnbtFieldViewModel SnbtField { get; }
        public PaletteEditViewModel PaletteEdit { get; }
        public BlockEditViewModel BlockEdit { get; }

        public StructureSummary? Summary
        {
            get => _summary;
            set
            {
                _summary = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SummaryText));
            }
        }

        public string SummaryText => Summary?.Description ?? "구조물 정보 없음";

        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand SaveAsFileCommand { get; }
        public RelayCommand ExitCommand { get; }
        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }

        public MainViewModel()
        {
            _nbtFileService = new NbtFileService(new SettingsService());

            var dialogService = new DialogService();
            var serializer = new StructureNbtSerializer();
            var treeBuilder = new NbtTreeBuilder();

            Session = new EditorSession();

            LayerSlice = new LayerSliceViewModel(Session);
            NbtTree = new NbtTreeViewModel(Session, serializer, treeBuilder);
            SnbtField = new SnbtFieldViewModel(Session);
            PaletteEdit = new PaletteEditViewModel(Session, dialogService);
            BlockEdit = new BlockEditViewModel(Session);

            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(SaveFile);
            SaveAsFileCommand = new RelayCommand(SaveAsFile);
            ExitCommand = new RelayCommand(Exit);

            UndoCommand = new RelayCommand(Undo, () => Session.CanUndo);
            RedoCommand = new RelayCommand(Redo, () => Session.CanRedo);

            NbtTree.TreeViewSelectionChanged += OnTreeSelectedNodeChanged;
            Session.PropertyChanged += OnSessionPropertyChanged;
        }

        private void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "NBT Files (*.nbt)|*.nbt|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            try
            {
                _currentFile = _nbtFileService.Load(dialog.FileName);
                Session.CurrentStructure = _structureParser.ParseStructure(_currentFile, dialog.SafeFileName, dialog.FileName);
                Summary = _structureParser.ParseSummary(_currentFile, dialog.FileName);
                _currentFilePath = dialog.FileName;
                _currentFileName = dialog.SafeFileName;
                Session.StatusMessage = "파일 로드 완료";
            }
            catch (Exception ex)
            {
                Session.StatusMessage = $"파일을 로드하는 중 오류가 발생했습니다: {ex.Message}";
            }
        }

        private void SaveFile()
        {
            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                var nbtFile = GetNbtFile();
                _nbtFileService.Save(nbtFile, _currentFilePath);
                return;
            }

            SaveAsFile();
        }

        private void SaveAsFile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "NBT Files (*.nbt)|*.nbt",
                DefaultExt = ".nbt",
                AddExtension = true,
                FileName = $"{_currentFileName}.nbt"
            };

            bool? result = dialog.ShowDialog();
            if (result != true)
                return;

            var path = dialog.FileName;

            if (!path.EndsWith(".nbt", StringComparison.OrdinalIgnoreCase))
            {
                path += ".nbt";
            }

            var nbtFile = GetNbtFile();
            _nbtFileService.Save(nbtFile, path);
            _currentFilePath = path;
        }

        private void Exit()
        {
            // TODO. 저장 여부 묻기
            Application.Current.Shutdown();
        }

        private void Undo()
        {
            Session.Undo();
        }

        private void Redo()
        {
            Session.Redo();
        }

        private NbtFile GetNbtFile()
        {
            var tag = NbtTree.RootNodes.FirstOrDefault()?.Tag;
            tag?.Name = "root";

            if (tag is not NbtCompound root)
                throw new InvalidOperationException("RootNode가 NbtCompound가 아닙니다.");

            var nbtFile = new NbtFile(root);
            return nbtFile;
        }

        private void ChangeLanguage(AppLanguage lang)
        {
            string fileName = lang switch
            {
                AppLanguage.English => "en_US",
                AppLanguage.Korean => "ko-KR",
                _ => "en_US.xaml"
            };

            var dict = new ResourceDictionary();
            dict.Source = new Uri($"Lang/{fileName}.xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        private void OnTreeSelectedNodeChanged()
        {
            LayerSlice.ClearSelection();
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Session.CanUndo))
            {
                UndoCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(Session.CanRedo))
            {
                RedoCommand.RaiseCanExecuteChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}