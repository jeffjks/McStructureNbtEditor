using McStructureNbtEditor.Models;
using fNbt;

// / 읽은 NBT를 “마인크래프트 구조물” 관점으로 해석 (size, palletes, block개수, 좌표 해석 등)

namespace McStructureNbtEditor.Services
{
    public class StructureParser
    {
        public StructureSummary ParseSummary(NbtFile file, string filePath)
        {
            var summary = new StructureSummary
            {
                FilePath = filePath
            };

            if (file.RootTag is not NbtCompound root)
                return summary;

            if (TryGetList(root, "size", out var sizeList) && sizeList.Count >= 3)
            {
                summary.SizeX = GetIntTagValue(sizeList[0]);
                summary.SizeY = GetIntTagValue(sizeList[1]);
                summary.SizeZ = GetIntTagValue(sizeList[2]);
            }

            if (TryGetList(root, "palette", out var paletteList))
                summary.PaletteCount = paletteList.Count;

            if (TryGetList(root, "blocks", out var blocksList))
                summary.BlocksCount = blocksList.Count;

            if (TryGetList(root, "entities", out var entitiesList))
                summary.EntitiesCount = entitiesList.Count;

            return summary;
        }

        public StructureFileModel ParseStructure(NbtFile file, string filePath)
        {
            var model = new StructureFileModel
            {
                FilePath = filePath
            };

            if (file.RootTag is not NbtCompound root)
                return model;

            if (TryGetList(root, "size", out var sizeList) && sizeList.Count >= 3)
            {
                model.SizeX = GetIntTagValue(sizeList[0]);
                model.SizeY = GetIntTagValue(sizeList[1]);
                model.SizeZ = GetIntTagValue(sizeList[2]);
            }

            if (TryGetList(root, "palette", out var paletteList))
            {
                for (int i = 0; i < paletteList.Count; i++)
                {
                    if (paletteList[i] is not NbtCompound paletteCompound)
                        continue;

                    var entry = new PaletteEntry
                    {
                        Index = i,
                        Name = GetStringFromCompound(paletteCompound, "Name")
                    };

                    if (TryGetCompound(paletteCompound, "Properties", out var propsCompound))
                    {
                        foreach (var child in propsCompound.Tags)
                        {
                            var childName = child.Name ?? "<unnamed>";
                            if (child is NbtString str)
                            {
                                entry.Properties[childName] = str.Value;
                            }
                            else
                            {
                                entry.Properties[childName] = child.ToString();
                            }
                        }
                    }

                    model.Palette.Add(entry);
                }
            }

            if (TryGetList(root, "blocks", out var blocksList))
            {
                for (int i = 0; i < blocksList.Count; i++)
                {
                    if (blocksList[i] is not NbtCompound blockCompound)
                        continue;

                    if (!TryGetList(blockCompound, "pos", out var posList) || posList.Count < 3)
                        continue;

                    var block = new StructureBlock
                    {
                        X = GetIntTagValue(posList[0]),
                        Y = GetIntTagValue(posList[1]),
                        Z = GetIntTagValue(posList[2]),
                        State = GetIntFromCompound(blockCompound, "state"),
                        HasBlockEntityData = blockCompound.Contains("nbt")
                    };

                    model.Blocks.Add(block);
                }
            }

            return model;
        }

        private bool TryGetList(NbtCompound compound, string name, out NbtList list)
        {
            list = null!;
            if (!compound.Contains(name))
                return false;

            if (compound[name] is not NbtList nbtList)
                return false;

            list = nbtList;
            return true;
        }

        private bool TryGetCompound(NbtCompound compound, string name, out NbtCompound childCompound)
        {
            childCompound = null!;
            if (!compound.Contains(name))
                return false;

            if (compound[name] is not NbtCompound c)
                return false;

            childCompound = c;
            return true;
        }

        private int GetIntFromCompound(NbtCompound compound, string name)
        {
            if (!compound.Contains(name))
                return 0;

            var value = compound[name];
            if (value == null)
                return 0;
            return GetIntTagValue(value);
        }

        private string GetStringFromCompound(NbtCompound compound, string name)
        {
            if (!compound.Contains(name))
                return "";

            var value = compound[name];
            if (value == null)
                return "";

            return value is NbtString s ? s.Value : value.ToString();
        }

        private int GetIntTagValue(NbtTag tag)
        {
            return tag switch
            {
                NbtByte b => b.ByteValue,
                NbtShort s => s.ShortValue,
                NbtInt i => i.IntValue,
                NbtLong l => (int)l.LongValue,
                _ => 0
            };
        }
    }
}