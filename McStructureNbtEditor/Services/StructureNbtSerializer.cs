using fNbt;
using McStructureNbtEditor.Models;

// StructureFileModel을 NbtCompound(Root)로 변환하고, 저장 시 NbtFile로 변환하여 저장

namespace McStructureNbtEditor.Services
{
    public interface IStructureNbtSerializer
    {
        NbtCompound BuildRootTag(StructureFileModel structure);
        NbtFile BuildFile(StructureFileModel structure);
    }

    public sealed class StructureNbtSerializer : IStructureNbtSerializer
    {
        public NbtCompound BuildRootTag(StructureFileModel structure)
        {
            if (structure == null)
                throw new ArgumentNullException(nameof(structure));

            ValidateStructure(structure);

            var root = new NbtCompound();

            WriteEntities(root, structure);
            WriteSize(root, structure);
            WriteBlocks(root, structure);
            WritePalette(root, structure);
            WriteDataVersion(root, structure);

            return root;
        }

        public NbtFile BuildFile(StructureFileModel structure)
        {
            var root = BuildRootTag(structure);
            root.Name = string.Empty;
            return new NbtFile(root);
        }

        private static void ValidateStructure(StructureFileModel structure)
        {
            if (structure.SizeX <= 0 || structure.SizeY <= 0 || structure.SizeZ <= 0)
                throw new InvalidOperationException("구조물 크기가 올바르지 않습니다.");

            if (structure.Palette == null)
                throw new InvalidOperationException("팔레트가 비어 있습니다.");

            if (structure.Blocks == null)
                throw new InvalidOperationException("블록 목록이 없습니다.");

            if (structure.Entities == null)
                throw new InvalidOperationException("엔티티 목록이 없습니다.");
        }

        private static void WriteDataVersion(NbtCompound root, StructureFileModel structure)
        {
            root.Add(new NbtInt("DataVersion", structure.DataVersion));
        }

        private static void WriteSize(NbtCompound root, StructureFileModel structure)
        {
            var size = new NbtList("size", NbtTagType.Int)
            {
                new NbtInt(structure.SizeX),
                new NbtInt(structure.SizeY),
                new NbtInt(structure.SizeZ)
            };

            root.Add(size);
        }

        private static void WritePalette(NbtCompound root, StructureFileModel structure)
        {
            var paletteList = new NbtList("palette", NbtTagType.Compound);

            foreach (var entry in structure.Palette)
            {
                paletteList.Add(BuildPaletteEntryTag(entry));
            }

            root.Add(paletteList);
        }

        private static NbtCompound BuildPaletteEntryTag(PaletteEntry entry)
        {
            var paletteTag = new NbtCompound
            {
                new NbtString("Name", entry.Name)
            };

            if (entry.Properties != null && entry.Properties.Count > 0)
            {
                var props = new NbtCompound("Properties");

                foreach (var pair in entry.Properties)
                {
                    props.Add(new NbtString(pair.Key, pair.Value));
                }

                paletteTag.Add(props);
            }

            return paletteTag;
        }

        private static void WriteBlocks(NbtCompound root, StructureFileModel structure)
        {
            var blocksList = new NbtList("blocks", NbtTagType.Compound);

            foreach (var block in structure.Blocks)
            {
                blocksList.Add(BuildBlockTag(block));
            }

            root.Add(blocksList);
        }

        private static NbtCompound BuildBlockTag(StructureBlock block)
        {
            var blockTag = new NbtCompound
            {
                new NbtList("pos", NbtTagType.Int)
                {
                    new NbtInt(block.BlockPos.X),
                    new NbtInt(block.BlockPos.Y),
                    new NbtInt(block.BlockPos.Z)
                },
                new NbtInt("state", block.State)
            };

            if (block.Nbt != null)
            {
                blockTag.Add(CloneCompoundWithName(block.Nbt, "nbt"));
            }

            return blockTag;
        }

        private static void WriteEntities(NbtCompound root, StructureFileModel structure)
        {
            var entitiesList = new NbtList("entities", NbtTagType.Compound);

            foreach (var entity in structure.Entities)
            {
                entitiesList.Add((NbtCompound)entity.Clone());
            }

            root.Add(entitiesList);
        }

        private static NbtCompound CloneCompoundWithName(NbtCompound source, string name)
        {
            var clone = (NbtCompound)source.Clone();
            clone.Name = name;
            return clone;
        }
    }
}
