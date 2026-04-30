using fNbt;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.IO;
using McStructureNbtEditor.ViewModels.Dialog;
using McStructureNbtEditor.Services.DialogResults;

namespace McStructureNbtEditor.ViewModels
{
    public class FileMenuViewModel : INotifyPropertyChanged
    {
        private readonly EditorSession _session;
        private readonly NbtFileService _nbtFileService;
        private readonly NbtTreeViewModel _nbtTree;
        private readonly IDialogService _dialogService;
        private NbtFile? _currentFile;

        private readonly StructureParser _structureParser = new();

        private StructureFileModel? _currentStructure => _session.CurrentStructure;
        private string _currentFileName => _session.CurrentStructure?.FileName ?? "";
        private string _currentFilePath => _session.CurrentStructure?.FilePath ?? "";

        public bool RequestSave() => TrySaveFile();

        public RelayCommand NewFileCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand SaveFileCommand { get; }
        public RelayCommand SaveAsFileCommand { get; }

        public FileMenuViewModel(EditorSession session, NbtTreeViewModel nbtTree, IDialogService dialogService, ISettingsService settingService)
        {
            _session = session;
            _nbtTree = nbtTree;
            _dialogService = dialogService;

            _nbtFileService = new NbtFileService(settingService);

            NewFileCommand = new RelayCommand(NewFile);
            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(() => TrySaveFile());
            SaveAsFileCommand = new RelayCommand(() => TrySaveAsFile());
        }

        private void NewFile()
        {
            if (_session.HasChanges)
            {
                var result = _dialogService.ShowHasChangesDialog("L_Dialog_MessageSaveModifiedOpen");

                switch (result)
                {
                    case HasChangesDialogResult.Save:
                        if (TrySaveFile() == false)
                            return;
                        break;

                    case HasChangesDialogResult.Ignore:
                        break;

                    case HasChangesDialogResult.Cancel:
                        return;
                }
            }

            var dialogResult = _dialogService.ShowNewFileDialog();

            if (dialogResult is NewFileDialogResult newFileResult)
                CreateNewFile(newFileResult);
        }

        private void CreateNewFile(NewFileDialogResult result)
        {
            try
            {
                var newStructureFileModel = StructureFileModel.CreateNew(result.StructureSize, result.DataVersion);
                var nbtFile = _structureParser.CreateNewNbtFile(result.StructureSize, result.DataVersion);

                _session.LoadCurrentStructure(newStructureFileModel);
                _session.StructureInfo = _structureParser.ParseStructureInfo(nbtFile, "");
                _session.StatusMessage = "파일 로드 완료";
            }
            catch (Exception ex)
            {
                _session.StatusMessage = $"새 파일 생성중 오류가 발생했습니다: {ex.Message}";
            }
        }

        private void OpenFile()
        {
            if (_session.HasChanges)
            {
                var result = _dialogService.ShowHasChangesDialog("L_Dialog_MessageSaveModifiedOpen");

                switch (result)
                {
                    case HasChangesDialogResult.Save:
                        if (TrySaveFile() == false)
                            return;
                        break;

                    case HasChangesDialogResult.Ignore:
                        break;

                    case HasChangesDialogResult.Cancel:
                        return;
                }
            }

            TryOpenFile();
        }

        public void OpenFile(string filePath)
        {
            if (_session.HasChanges)
            {
                var result = _dialogService.ShowHasChangesDialog("L_Dialog_MessageSaveModifiedOpen");

                switch (result)
                {
                    case HasChangesDialogResult.Save:
                        if (TrySaveFile() == false)
                            return;
                        break;

                    case HasChangesDialogResult.Ignore:
                        break;

                    case HasChangesDialogResult.Cancel:
                        return;
                }
            }

            TryOpenFile(filePath);
        }

        private bool TryOpenFile(string filePath = "")
        {
            string fileName;

            if (filePath == "")
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "NBT Files (*.nbt)|*.nbt|All Files (*.*)|*.*"
                };

                if (dialog.ShowDialog() != true)
                    return false;

                fileName = dialog.SafeFileName;
                filePath = dialog.FileName;
            }
            else
            {
                fileName = Path.GetFileName(filePath);
            }

            try
            {
                _currentFile = _nbtFileService.Load(filePath);
                var newStructureFileModel = _structureParser.ParseStructure(_currentFile, fileName, filePath);
                _session.LoadCurrentStructure(newStructureFileModel);
                _session.StructureInfo = _structureParser.ParseStructureInfo(_currentFile, filePath);
                _session.StatusMessage = "파일 로드 완료";
            }
            catch (Exception ex)
            {
                _session.StatusMessage = $"파일을 로드하는 중 오류가 발생했습니다: {ex.Message}";
                return false;
            }
            return true;
        }

        private bool TrySaveFile()
        {
            if (_currentStructure == null)
                return false;

            try
            {
                if (_currentStructure.IsNewFile)
                {
                    return TrySaveAsFile();
                }

                return SaveToFile(_currentFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"파일 저장 중 오류가 발생했습니다: {ex.Message}",
                    "오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
        }

        private bool TrySaveAsFile()
        {
            if (_currentStructure == null)
                return false;

            var dialog = new SaveFileDialog
            {
                Filter = "NBT Files (*.nbt)|*.nbt",
                DefaultExt = ".nbt",
                AddExtension = true,
                FileName = _currentStructure.IsNewFile ? "" : _currentFileName
            };

            if (dialog.ShowDialog() != true)
                return false;

            var filePath = dialog.FileName;

            return SaveToFile(filePath);
        }

        private bool SaveToFile(string filePath)
        {
            if (_currentStructure == null)
                return false;

            var nbtFile = GetNbtFile();
            _nbtFileService.Save(nbtFile, filePath);

            var fileName = Path.GetFileName(filePath);
            _currentStructure.SetFileName(fileName, filePath);

            _session.SetSavedHistoryIndex();
            return true;
        }

        private NbtFile GetNbtFile()
        {
            var tag = _nbtTree.RootNodes.FirstOrDefault()?.Tag;
            tag?.Name = "root";

            if (tag is not NbtCompound root)
                throw new InvalidOperationException("RootNode가 NbtCompound가 아닙니다.");

            var nbtFile = new NbtFile(root);
            return nbtFile;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
