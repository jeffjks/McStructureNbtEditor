using McStructureNbtEditor.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.ViewModels
{
    public class SnbtFieldViewModel : INotifyPropertyChanged
    {
        private readonly EditorSession _session;
        private string _displayedSnbt = string.Empty;
        public string DisplayedSnbt
        {
            get => _displayedSnbt;
            set { _displayedSnbt = value; OnPropertyChanged(); }
        }

        public SnbtFieldViewModel(EditorSession session)
        {
            _session = session;
            _session.PropertyChanged += OnSessionPropertyChanged;
            RefreshFromSelection();
        }

        private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditorSession.SelectedInspectable))
                RefreshFromSelection();
        }

        private void RefreshFromSelection()
        {
            DisplayedSnbt = _session.SelectedInspectable?.GetSnbtText() ?? string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
