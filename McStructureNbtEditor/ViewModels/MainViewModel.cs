using fNbt;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

// 열기/저장 명령, 현재 파일 상태, 트리 노드, 요약 정보

namespace McStructureNbtEditor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly NbtFileService _nbtFileService = new();
        private readonly StructureParser _structureParser = new();
        private readonly NbtTreeBuilder _treeBuilder = new();

        private StructureSummary? _summary;
        private NbtFile? _currentFile;

        public ObservableCollection<NbtTreeNode> RootNodes { get; } = new();
        public EditorSession Session { get; }
        public LayerSliceViewModel LayerSlice { get; }
        public NbtTreeViewModel NbtTree { get; }
        public SnbtFieldViewModel SnbtField { get; }

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

        public MainViewModel()
        {
            Session = new EditorSession();
            NbtTree = new NbtTreeViewModel(Session);
            LayerSlice = new LayerSliceViewModel(Session);
            SnbtField = new SnbtFieldViewModel(Session);

            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(SaveFile);
            SaveAsFileCommand = new RelayCommand(SaveAsFile);
            ExitCommand = new RelayCommand(Exit);

            NbtTree.TreeViewSelectionChanged += OnTreeSelectedNodeChanged;
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
                Session.CurrentStructure = _structureParser.ParseStructure(_currentFile, dialog.FileName);

                RootNodes.Clear();
                var fileName = dialog.SafeFileName;
                var rootNode = _treeBuilder.BuildRoot(_currentFile.RootTag, fileName);
                RootNodes.Add(rootNode);

                Summary = _structureParser.ParseSummary(_currentFile, dialog.FileName);
                LayerSlice.LoadStructure(Session.CurrentStructure);

                Summary = _structureParser.ParseSummary(_currentFile, dialog.FileName);
                Session.StatusMessage = "파일 로드 완료";
            }
            catch (Exception ex)
            {
                Session.StatusMessage = $"파일을 로드하는 중 오류가 발생했습니다: {ex.Message}";
            }
        }

        private void SaveFile()
        {

        }

        private void SaveAsFile()
        {

        }

        private void Exit()
        {
            // TODO. 저장 여부 묻기
            Application.Current.Shutdown();
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}