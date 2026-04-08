using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Win32;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using fNbt;

// 열기/저장 명령, 현재 파일 상태, 트리 노드, 요약 정보

namespace McStructureNbtEditor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly NbtFileService _nbtFileService = new();
        private readonly StructureParser _structureParser = new();
        private readonly NbtTreeBuilder _treeBuilder = new();

        private string _statusText = "파일을 열어주세요.";
        private StructureSummary? _summary;
        private NbtFile? _currentFile;
        private StructureFileModel? _currentStructure;

        public ObservableCollection<NbtTreeNode> RootNodes { get; } = new();
        public LayerSliceViewModel LayerSlice { get; } = new();

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value;
                OnPropertyChanged();
            }
        }

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
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(SaveFile);
            SaveAsFileCommand = new RelayCommand(SaveAsFile);
            ExitCommand = new RelayCommand(Exit);
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
                _currentStructure = _structureParser.ParseStructure(_currentFile, dialog.FileName);

                RootNodes.Clear();
                var fileName = dialog.SafeFileName;
                var rootNode = _treeBuilder.BuildRoot(_currentFile.RootTag, fileName);
                RootNodes.Add(rootNode);

                Summary = _structureParser.ParseSummary(_currentFile, dialog.FileName);
                LayerSlice.LoadStructure(_currentStructure);

                Summary = _structureParser.ParseSummary(_currentFile, dialog.FileName);
                StatusText = "파일 로드 완료";
            }
            catch (Exception ex)
            {
                StatusText = $"파일을 로드하는 중 오류가 발생했습니다: {ex.Message}";
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
            System.Windows.Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}