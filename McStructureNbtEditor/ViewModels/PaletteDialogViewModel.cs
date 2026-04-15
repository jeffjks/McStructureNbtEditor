using McStructureNbtEditor.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace McStructureNbtEditor.ViewModels
{
    public class AddPaletteDialogViewModel : INotifyPropertyChanged
    {
        private string _name = "";
        private PalettePropertiesItemDraft? _selectedProperty;
        private string _errorMessage = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanConfirm));
                ((RelayCommand)ConfirmCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<PalettePropertiesItemDraft> Properties { get; } = new();

        public PalettePropertiesItemDraft? SelectedProperty
        {
            get => _selectedProperty;
            set
            {
                if (_selectedProperty == value)
                    return;

                _selectedProperty = value;
                OnPropertyChanged();
                ((RelayCommand)RemovePropertyCommand).RaiseCanExecuteChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage == value)
                    return;

                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public bool CanConfirm => !string.IsNullOrWhiteSpace(Name);

        public PaletteEntryDraft? Result { get; private set; }

        public ICommand AddPropertyCommand { get; }
        public ICommand RemovePropertyCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public Action<bool?>? CloseAction { get; set; }

        public AddPaletteDialogViewModel()
        {
            AddPropertyCommand = new RelayCommand(AddProperty);
            RemovePropertyCommand = new RelayCommand(RemoveProperty, () => SelectedProperty != null);
            ConfirmCommand = new RelayCommand(Confirm, () => CanConfirm);
            CancelCommand = new RelayCommand(Cancel);

            Properties.Add(new PalettePropertiesItemDraft());
        }

        private void AddProperty()
        {
            var item = new PalettePropertiesItemDraft();
            Properties.Add(item);
            SelectedProperty = item;
        }

        private void RemoveProperty()
        {
            if (SelectedProperty == null)
                return;

            Properties.Remove(SelectedProperty);
            SelectedProperty = null;
        }

        private void Confirm()
        {
            ErrorMessage = "";

            string trimmedName = Name.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                ErrorMessage = "블록 이름을 입력하세요.";
                return;
            }

            var duplicateKey = Properties
                .Where(p => !string.IsNullOrWhiteSpace(p.Key))
                .GroupBy(p => p.Key.Trim(), StringComparer.Ordinal)
                .FirstOrDefault(g => g.Count() > 1);

            if (duplicateKey != null)
            {
                ErrorMessage = $"중복된 속성 키가 있습니다: {duplicateKey.Key}";
                return;
            }

            var draft = new PaletteEntryDraft
            {
                Name = trimmedName
            };

            foreach (var property in Properties)
            {
                var key = property.Key?.Trim() ?? "";
                var value = property.Value?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                draft.Properties.Add(new PalettePropertiesItemDraft
                {
                    Key = key,
                    Value = value
                });
            }

            Result = draft;
            CloseAction?.Invoke(true);
        }

        private void Cancel()
        {
            Result = null;
            CloseAction?.Invoke(false);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}