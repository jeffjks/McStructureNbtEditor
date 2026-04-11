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
            // TODO. 추가 Dialog 및 조건
            var result = _dialogService.ShowAddPaletteDialog();

            if (!result.Confirmed)
                return;

            if (string.IsNullOrWhiteSpace(result.PaletteName))
            {
                _session.StatusMessage = "팔레트 이름이 비어 있습니다.";
                return;
            }

            var command = new AddPaletteEntryCommand(result.PaletteName, string.Empty);
            if (!_session.ExecuteCommand(command))
            {
                _session.StatusMessage = "팔레트 추가에 실패했습니다.";
            }
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
            if (e.PropertyName == nameof(EditorSession.CurrentStructure))
            {
                OnPropertyChanged(nameof(PaletteEntries));
                OnPropertyChanged(nameof(SelectedPaletteEntry));
                AddPaletteCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(EditorSession.SelectedPaletteEntry))
            {
                OnPropertyChanged(nameof(SelectedPaletteEntry));
            }
            else if (e.PropertyName == nameof(EditorSession.CanUndo))
            {
                UndoCommand.RaiseCanExecuteChanged();
            }
            else if (e.PropertyName == nameof(EditorSession.CanRedo))
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
