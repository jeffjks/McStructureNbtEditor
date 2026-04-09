using fNbt;
using System.Collections.ObjectModel;

// WPF TreeView에 표시하기 위한 뷰 모델용 노드
// fNbt 태그를 UI에 바로 바인딩하지 않고 한 번 감싸는 용도

namespace McStructureNbtEditor.Models
{
    public class NbtTreeNode
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public string ValuePreview { get; set; } = "";

        public ObservableCollection<NbtTreeNode> Children { get; set; } = new();

        public NbtTag? Tag { get; set; }
        public bool IsBlockNode { get; set; }

        public string DisplayText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ValuePreview))
                    return $"{Name} ({Type})";

                return $"{Name} ({Type}) : {ValuePreview}";
            }
        }
    }
}