using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using McStructureNbtEditor.ViewModels.Dialog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

// 열기/저장 명령, 현재 파일 상태, 트리 노드, 요약 정보

namespace McStructureNbtEditor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IDialogService _dialogService = new DialogService();
        private readonly ISettingsService _settingService = new SettingsService();

        public EditorSession Session { get; }
        public FileMenuViewModel FileMenu { get; }
        public ChangeLanguageViewModel ChangeLanguageMenu { get; }
        public LayerSliceViewModel LayerSlice { get; }
        public NbtTreeViewModel NbtTree { get; }
        public SnbtFieldViewModel SnbtField { get; }
        public PaletteEditViewModel PaletteEdit { get; }
        public BlockEditViewModel BlockEdit { get; }

        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }
        public RelayCommand AboutCommand { get; }
        public RelayCommand ExitCommand { get; }

        public bool IsClosingApproved { get; private set; }

        public MainViewModel()
        {
            var serializer = new StructureNbtSerializer();
            var treeBuilder = new NbtTreeBuilder();

            Session = new EditorSession();

            LayerSlice = new LayerSliceViewModel(Session);
            NbtTree = new NbtTreeViewModel(Session, serializer, treeBuilder);
            SnbtField = new SnbtFieldViewModel(Session);
            PaletteEdit = new PaletteEditViewModel(Session, _dialogService);
            BlockEdit = new BlockEditViewModel(Session);

            FileMenu = new FileMenuViewModel(Session, NbtTree, _dialogService, _settingService);
            ChangeLanguageMenu = new ChangeLanguageViewModel(_settingService);

            UndoCommand = new RelayCommand(Undo, () => Session.CanUndo);
            RedoCommand = new RelayCommand(Redo, () => Session.CanRedo);

            AboutCommand = new RelayCommand(OpenAbout);

            ExitCommand = new RelayCommand(Exit);

            NbtTree.TreeViewSelectionChanged += OnTreeSelectedNodeChanged;
            Session.PropertyChanged += OnSessionPropertyChanged;
        }

        private void Undo()
        {
            Session.Undo();
        }

        private void Redo()
        {
            Session.Redo();
        }

        private void OpenAbout()
        {
            _dialogService.ShowCommonDialog(CommonDialogViewModel.AboutDialog());
        }

        private void Exit()
        {
            if (TryCloseApplication())
                Application.Current.Shutdown();
        }

        public bool TryCloseApplication()
        {
            if (Session.HasChanges)
            {
                var result = _dialogService.ShowHasChangesDialog();

                switch (result)
                {
                    case HasChangesDialogResult.Save:
                        if (FileMenu.RequestSave() == false)
                            return false;
                        break;

                    case HasChangesDialogResult.Ignore:
                        break;

                    case HasChangesDialogResult.Cancel:
                        return false;
                }
            }

            IsClosingApproved = true;
            return true;
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