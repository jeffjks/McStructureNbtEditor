using McStructureNbtEditor.Services.DialogResults;

namespace McStructureNbtEditor.Services
{
    public interface IDialogService
    {
        AddPaletteDialogResult ShowAddPaletteDialog();
        bool ShowCommonDialog(string title, string message);
    }
}
