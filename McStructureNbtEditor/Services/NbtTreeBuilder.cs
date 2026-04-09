using McStructureNbtEditor.Models;
using fNbt;

// NBT 태그를 WPF 트리용 노드로 바꾸는 코드

namespace McStructureNbtEditor.Services
{
    public class NbtTreeBuilder
    {
        public NbtTreeNode Build(NbtTag tag)
        {
            var node = new NbtTreeNode
            {
                Name = string.IsNullOrEmpty(tag.Name) ? "<unnamed>" : tag.Name,
                Type = tag.TagType.ToString(),
                ValuePreview = GetValuePreview(tag),
                Tag = tag
            };

            switch (tag)
            {
                case NbtCompound compound:
                    foreach (var child in compound.Tags)
                    {
                        node.Children.Add(Build(child));
                    }
                    break;

                case NbtList list:
                    bool isBlocksList = string.Equals(tag.Name, "blocks", StringComparison.Ordinal);

                    for (int i = 0; i < list.Count; i++)
                    {
                        var child = list[i];
                        var childNode = Build(child);
                        childNode.Name = $"[{i}]";

                        if (isBlocksList && child is NbtCompound)
                        {
                            childNode.IsBlockNode = true;
                        }
                        node.Children.Add(childNode);
                    }
                    break;
            }

            return node;
        }

        public NbtTreeNode BuildRoot(NbtTag rootTag, string fileName)
        {
            var rootNode = Build(rootTag);
            rootNode.Name = fileName;
            return rootNode;
        }

        private void CheckIfBlockNode()
        {

        }

        private string GetValuePreview(NbtTag tag)
        {
            return tag switch
            {
                NbtString s => s.Value,
                NbtByte b => b.ByteValue.ToString(),
                NbtShort s => s.ShortValue.ToString(),
                NbtInt i => i.IntValue.ToString(),
                NbtLong l => l.LongValue.ToString(),
                NbtFloat f => f.FloatValue.ToString(),
                NbtDouble d => d.DoubleValue.ToString(),
                NbtByteArray ba => $"ByteArray[{ba.Value.Length}]",
                NbtIntArray ia => $"IntArray[{ia.Value.Length}]",
                NbtLongArray la => $"LongArray[{la.Value.Length}]",
                NbtCompound c => $"Children={c.Tags.Count()}",
                NbtList l => $"Count={l.Count}",
                _ => ""
            };
        }
    }
}