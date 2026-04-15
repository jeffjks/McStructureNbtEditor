using McStructureNbtEditor.Services.DialogResults;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor.Services
{
    public interface IDialogService
    {
        PaletteDialogResult ShowPaletteDialog();
        bool ShowCommonDialog(string title, string message);
        HasChangesDialogResult ShowHasChangesDialog();
    }
}
