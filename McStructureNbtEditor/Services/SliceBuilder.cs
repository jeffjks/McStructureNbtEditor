using McStructureNbtEditor.Models;
using System.Collections.ObjectModel;

namespace McStructureNbtEditor.Services
{
    public class SliceBuilder
    {
        private const string AIR_BLOCK = "minecraft:air";
        private readonly Dictionary<(int, int), StructureBlock> _structureBlockMap = new();

        public ObservableCollection<BlockCellModel> BuildSlice(StructureFileModel? structure, int y)
        {
            var result = new ObservableCollection<BlockCellModel>();

            if (structure == null || structure.SizeX <= 0 || structure.SizeZ <= 0)
                return result;

            _structureBlockMap.Clear();

            foreach (var block in structure.Blocks.Where(b => b.BlockPos.Y == y))
            {
                _structureBlockMap[(block.BlockPos.X, block.BlockPos.Z)] = block;
            }

            for (int z = 0; z < structure.SizeZ; z++)
            {
                for (int x = 0; x < structure.SizeX; x++)
                {
                    if (_structureBlockMap.TryGetValue((x, z), out var block))
                    {
                        var palette = structure.GetPaletteEntry(block.State);
                        var blockName = palette?.Name ?? $"<state:{block.State}>";

                        result.Add(new BlockCellModel
                        {
                            BlockIndex = block.Index,
                            BlockPos = new BlockPosition(x, y, z),
                            Tag = block.Nbt,
                            BlockName = blockName,
                            State = block.State,
                            IsEmpty = false,
                            CellText = ToShortBlockText(blockName)
                        });
                    }
                    else
                    {
                        result.Add(new BlockCellModel
                        {
                            BlockIndex = -1,
                            BlockPos = new BlockPosition(x, y, z),
                            Tag = null,
                            BlockName = string.Empty,
                            State = -1,
                            IsEmpty = true,
                            CellText = "",
                        });
                    }
                }
            }

            return result;
        }

        public void UpdateSlice(StructureFileModel? structure, ObservableCollection<BlockCellModel> cells, int y)
        {
            if (structure == null || structure.SizeX <= 0 || structure.SizeZ <= 0)
                return;

            _structureBlockMap.Clear();

            foreach (var block in structure.Blocks.Where(b => b.BlockPos.Y == y))
            {
                _structureBlockMap[(block.BlockPos.X, block.BlockPos.Z)] = block;
            }

            var cellIndex = 0;
            for (int z = 0; z < structure.SizeZ; z++)
            {
                for (int x = 0; x < structure.SizeX; x++)
                {
                    var curCell = cells[cellIndex];

                    if (_structureBlockMap.TryGetValue((x, z), out var block))
                    {
                        var palette = structure.GetPaletteEntry(block.State);
                        var blockName = palette?.Name ?? $"<state:{block.State}>";

                        curCell.BlockIndex = block.Index;
                        curCell.BlockPos = new BlockPosition(x, y, z);
                        curCell.Tag = block.Nbt;
                        curCell.BlockName = blockName;
                        curCell.State = block.State;
                        curCell.IsEmpty = false;
                        curCell.CellText = ToShortBlockText(blockName);
                    }
                    else
                    {
                        curCell.BlockIndex = -1;
                        curCell.BlockPos = new BlockPosition(x, y, z);
                        curCell.Tag = null;
                        curCell.BlockName = string.Empty;
                        curCell.State = -1;
                        curCell.IsEmpty = true;
                        curCell.CellText = "";
                    }

                    cellIndex++;
                }
            }
        }

        private string ToShortBlockText(string blockName)
        {
            if (string.IsNullOrWhiteSpace(blockName) || blockName == AIR_BLOCK)
                return "";

            const string prefix = "minecraft:";
            if (blockName.StartsWith(prefix))
                blockName = blockName.Substring(prefix.Length);

            return GetRepresentativeString(blockName);
        }

        private string GetRepresentativeString(string rawBlockName)
        {
            if (rawBlockName.Contains("_"))
            {
                string[] parts = rawBlockName.Split('_');

                char first = char.ToUpper(parts[0][0]);
                char last = char.ToUpper(parts[^1][0]);

                return string.Concat(first, last);
            }
            else
            {
                if (rawBlockName.Length < 2) return rawBlockName.ToUpper();

                return ToCustomTitleCase(rawBlockName[0], rawBlockName[1]);
            }
        }

        private string ToCustomTitleCase(char c1, char c2)
        {
            return char.ToUpper(c1).ToString() + char.ToLower(c2).ToString();
        }
    }
}