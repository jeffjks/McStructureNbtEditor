using McStructureNbtEditor.Commands;
using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

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
        public RelayCommand RemovePaletteCommand { get; }
        public RelayCommand EditPaletteCommand { get; }
        public RelayCommand DuplicatePaletteCommand { get; }

        public PaletteEditViewModel(EditorSession session, IDialogService dialogService)
        {
            _session = session;
            _dialogService = dialogService;
            _session.PropertyChanged += OnSessionPropertyChanged;
            _session.DocumentChanged += OnDocumentChanged;

            AddPaletteCommand = new RelayCommand(AddPalette, CanAddPalette);
            RemovePaletteCommand = new RelayCommand(RemovePalette, HasSelectedPaletteEntry);
            EditPaletteCommand = new RelayCommand(EditPalette, HasSelectedPaletteEntry);
            DuplicatePaletteCommand = new RelayCommand(DuplicatePalette, () => {return false; }); // HasSelectedPaletteEntry
        }

        private bool CanAddPalette()
        {
            return _session.CurrentStructure != null;
        }

        private bool HasSelectedPaletteEntry()
        {
            return (_session.CurrentStructure != null) && (SelectedPaletteEntry != null);
        }

        private void AddPalette()
        {
            var structure = _session.CurrentStructure;
            var result = _dialogService.ShowPaletteDialog(PaletteMode.Add);

            if (!result.Confirmed || result.Draft == null || structure == null)
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
                    structure.Palette.Count
                );
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
            }
        }

        private void RemovePalette()
        {
            var structure = _session.CurrentStructure;

            if (structure == null || SelectedPaletteEntry == null)
                return;

            int paletteIndex = SelectedPaletteEntry.Index;
            if (paletteIndex < 0 || structure.Palette.Count <= paletteIndex)
            {
                _session.StatusMessage = "삭제할 팔레트를 찾을 수 없습니다.";
                return;
            }

            int removedBlockCount = structure.Blocks.Count(b => b.State == paletteIndex);

            var title = "팔레트 삭제 확인";
            var message =
                $"팔레트 '{SelectedPaletteEntry.Name}' 을(를) 삭제하시겠습니까?\n" +
                $"이 작업으로 해당 팔레트를 가진 블록 {removedBlockCount}개가 함께 삭제됩니다.";

            bool confirmed = _dialogService.ShowCommonDialog(title, message);
            if (!confirmed)
                return;

            var command = new RemovePaletteEntryCommand(paletteIndex);
            if (!_session.ExecuteCommand(command))
            {
                _session.StatusMessage = "팔레트 삭제에 실패했습니다.";
            }
        }

        private void EditPalette()
        {
            var structure = _session.CurrentStructure;

            if (structure == null || SelectedPaletteEntry == null)
                return;

            int paletteIndex = SelectedPaletteEntry.Index;
            if (paletteIndex < 0 || structure.Palette.Count <= paletteIndex)
            {
                _session.StatusMessage = "수정할 팔레트를 찾을 수 없습니다.";
                return;
            }

            var result = _dialogService.ShowPaletteDialog(PaletteMode.Edit, SelectedPaletteEntry);

            if (!result.Confirmed || result.Draft == null || structure == null)
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
                    paletteIndex
                );
            }
            catch (Exception ex)
            {
                _session.StatusMessage = $"팔레트 데이터가 올바르지 않습니다: {ex.Message}";
                return;
            }

            var command = new EditPaletteEntryCommand(paletteIndex, entry);
            if (!_session.ExecuteCommand(command))
            {
                _session.StatusMessage = "팔레트 수정에 실패했습니다.";
            }
        }
        private void DuplicatePalette() { }

        private void OnDocumentChanged(object? sender, DocumentChangedEventArgs e)
        {
            if (e.ChangeType == ReloadScope.ReloadAll)
            {
                OnPropertyChanged(nameof(PaletteEntries));
                OnPropertyChanged(nameof(SelectedPaletteEntry));
            }
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
                EditPaletteCommand.RaiseCanExecuteChanged();
                DuplicatePaletteCommand.RaiseCanExecuteChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
