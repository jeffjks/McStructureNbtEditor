using McStructureNbtEditor.Services.DialogResults;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor.Services
{
    public interface IDialogService
    {
        AddPaletteDialogResult ShowAddPaletteDialog();
        bool ShowCommonDialog(string title, string message);
        HasChangesDialogResult ShowHasChangesDialog();
    }
}
