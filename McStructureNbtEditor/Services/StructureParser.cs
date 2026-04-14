using fNbt;
using McStructureNbtEditor.Models;
using System.IO;

// 읽은 NBT를 StructureFileModel 타입으로 변환 (size, paletes, block개수, 좌표 해석 등)
// 파일 Open 시 최초 1회만 시행

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

        public StructureFileModel ParseStructure(NbtFile file, string fileName, string filePath)
        {
            if (file.RootTag is not NbtCompound root)
                throw new InvalidDataException($"잘못된 NBT 파일입니다. (잘못된 RootTag 타입: {file.RootTag?.GetType().Name ?? "null"})");

            int sizeX, sizeY, sizeZ;
            if (TryGetList(root, "size", out var sizeList) && sizeList.Count >= 3)
            {
                sizeX = GetIntTagValue(sizeList[0]);
                sizeY = GetIntTagValue(sizeList[1]);
                sizeZ = GetIntTagValue(sizeList[2]);
            }
            else
            {
                throw new InvalidDataException($"잘못된 NBT 파일입니다. (잘못된 Size 데이터)");
            }

            var model = StructureFileModel.OpenFromFile(fileName, filePath, sizeX, sizeY, sizeZ);

            if (TryGetList(root, "entities", out var entityList))
            {
                for (int i = 0; i < entityList.Count; i++)
                {
                    if (entityList[i] is not NbtCompound blockCompound)
                        continue;

                    model.Entities.Add(entityList[i]);
                }
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
                            var childName = child.Name ?? "<noname>";
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

                    var blockPos = new BlockPosition(
                        GetIntTagValue(posList[0]),
                        GetIntTagValue(posList[1]),
                        GetIntTagValue(posList[2])
                    );
                    var state = GetIntFromCompound(blockCompound, "state");
                    TryGetCompound(blockCompound, "nbt", out var nbtData);

                    var block = new StructureBlock(i, blockPos, state, nbtData);

                    model.Blocks.Add(block);
                }
            }

            model.DataVersion = GetIntFromCompound(root, "DataVersion");

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