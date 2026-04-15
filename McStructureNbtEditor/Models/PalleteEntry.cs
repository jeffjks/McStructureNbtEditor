using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace McStructureNbtEditor.Models
{
    public class PaletteEntry : INotifyPropertyChanged
    {
        private int _index;
        private string _name = "";
        private Dictionary<string, string> _properties = new();

        public int Index
        {
            get => _index;
            set
            {
                if (_index == value)
                    return;

                _index = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ItemTemplate));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ItemTemplate));
            }
        }

        public Dictionary<string, string> Properties
        {
            get => _properties;
            set
            {
                if (_properties == value)
                    return;

                _properties = value;
                OnPropertyChanged();
            }
        }

        public string ItemTemplate => $"[{Index}] {Name}";

        public string DisplayName
        {
            get
            {
                if (Properties.Count == 0)
                    return Name;

                return $"{Name} [{string.Join(", ", Properties.Select(kv => $"{kv.Key}={kv.Value}"))}]";
            }
        }

        public PaletteEntry() { }

        public PaletteEntry(PaletteEntry other)
        {
            _index = other._index;
            _name = other._name;

            _properties = new Dictionary<string, string>(other._properties);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}