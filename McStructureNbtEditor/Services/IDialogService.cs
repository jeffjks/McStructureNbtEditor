using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services.DialogResults;
using McStructureNbtEditor.ViewModels;

namespace McStructureNbtEditor.Services
{
    public interface IDialogService
    {
        PaletteDialogResult ShowPaletteDialog(PaletteMode mode, PaletteEntry? defaultPaletteEntry = null);
        bool ShowCommonDialog(string title, string message);
        HasChangesDialogResult ShowHasChangesDialog();
    }
}
