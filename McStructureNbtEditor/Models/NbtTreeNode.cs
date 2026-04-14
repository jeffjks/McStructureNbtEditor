using fNbt;
using McStructureNbtEditor.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// WPF TreeView에 표시하기 위한 뷰 모델용 노드
// fNbt 태그를 UI에 바로 바인딩하지 않고 한 번 감싸는 용도

namespace McStructureNbtEditor.Models
{
    public class NbtTreeNode : ISnbtInspectable, INotifyPropertyChanged
    {

        private string _name = "";
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged();
            }
        }

        public string Type { get; set; } = "";
        public string ValuePreview { get; set; } = "";

        public ObservableCollection<NbtTreeNode> Children { get; set; } = new();

        public NbtTag? Tag { get; set; }
        public bool IsBlockNode { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public string DisplayText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ValuePreview))
                    return $"{Name} ({Type})";

                return $"{Name} ({Type}) : {ValuePreview}";
            }
        }


        public string GetSnbtText()
        {
            if (Tag == null)
                return "{}";
            return NbtSnbtConverter.ToSnbt(Tag);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}