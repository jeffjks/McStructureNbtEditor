using McStructureNbtEditor.Models;

namespace McStructureNbtEditor.Services.DialogResults
{
    public sealed class NewFileDialogResult
    {
        public StructureSize StructureSize { get; set; }
        public int DataVersion { get; set; }

        public NewFileDialogResult(StructureSize structureSize, int dataVersion)
        {
            StructureSize = structureSize;
            DataVersion = dataVersion;
        }
    }
}
