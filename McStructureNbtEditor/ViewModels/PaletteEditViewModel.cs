using McStructureNbtEditor.Commands;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.ViewModels
{
    public class PaletteEditViewModel : INotifyPropertyChanged
    {
        private readonly EditorSession _session;
        private readonly IDialogService _dialogService;

        public IReadOnlyList<PaletteEntry> PaletteEntries
            => _session.CurrentStructure?.Palette ?? new ObservableCollection<PaletteEntry>();

        public PaletteEntry? SelectedPaletteEntry
        {
            get => _session.SelectedPaletteEntry;
            set
            {
                if (_session.SelectedPaletteEntry == value)
                    return;

                _session.SelectedPaletteEntry = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddPaletteCommand { get; }
        public RelayCommand DeletePaletteCommand { get; }
        public RelayCommand ModifyPaletteCommand { get; }
        public RelayCommand DuplicatePaletteCommand { get; }
        public RelayCommand UndoCommand { get; }
        public RelayCommand RedoCommand { get; }

        public PaletteEditViewModel(EditorSession session, IDialogService dialogService)
        {
            _session = session;
            _dialogService = dialogService;
            _session.PropertyChanged += OnSessionPropertyChanged;
            _session.DocumentChanged += OnSessionDocumentChanged;


            AddPaletteCommand = new RelayCommand(AddPalette, CanAddPalette);
            DeletePaletteCommand = new RelayCommand(DeletePalette);
            ModifyPaletteCommand = new RelayCommand(ModifyPalette);
            DuplicatePaletteCommand = new RelayCommand(DuplicatePalette);

            UndoCommand = new RelayCommand(Undo, () => _session.CanUndo);
            RedoCommand = new RelayCommand(Redo, () => _session.CanRedo);
        }

        private bool CanAddPalette()
        {
            return _session.CurrentStructure != null;
        }

        private void AddPalette()
        {
            var result = _dialogService.ShowAddPaletteDialog();

            if (!result.Confirmed || result.Draft == null)
                return;

            if (string.IsNullOrWhiteSpace(result.Draft.Name))
            {
                _session.StatusMessage = "팔레트 이름이 비어 있습니다.";
                return;
            }

            PaletteEntry entry;
            try
            {
                entry = PaletteEntryFactory.CreateFromDraft(
                    result.Draft,
                    _session.CurrentStructure!.Palette.Count);
            }
            catch (Exception ex)
            {
                _session.StatusMessage = $"팔레트 데이터가 올바르지 않습니다: {ex.Message}";
                return;
            }

            var command = new AddPaletteEntryCommand(entry);
            if (!_session.ExecuteCommand(command))
            {
                _session.StatusMessage = "팔레트 추가에 실패했습니다.";
                return;
            }

            _session.StatusMessage = $"팔레트 '{entry.Name}' 추가됨.";
        }

        private void DeletePalette() { }
        private void ModifyPalette() { }
        private void DuplicatePalette() { }

        private void Undo()
        {
            _session.Undo();
        }

        private void Redo()
        {
            _session.Redo();
        }

        private void OnSessionDocumentChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(PaletteEntries));
            OnPropertyChanged(nameof(SelectedPaletteEntry));
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_session.CurrentStructure))
            {
                OnPropertyChanged(nameof(PaletteEntries));
                OnPropertyChanged(nameof(SelectedPaletteEntry));
                AddPaletteCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(_session.SelectedPaletteEntry))
            {
                OnPropertyChanged(nameof(SelectedPaletteEntry));
                RemovePaletteCommand.RaiseCanExecuteChanged();
                ModifyPaletteCommand.RaiseCanExecuteChanged();
                DuplicatePaletteCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(_session.CanUndo))
            {
                UndoCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(_session.CanRedo))
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
