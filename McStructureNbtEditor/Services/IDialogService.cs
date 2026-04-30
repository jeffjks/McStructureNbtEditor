using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services.DialogResults;
using McStructureNbtEditor.ViewModels.Dialog;

namespace McStructureNbtEditor.Services
{
    public interface IDialogService
    {
        PaletteDialogResult ShowPaletteDialog(PaletteMode mode, PaletteEntry? defaultPaletteEntry = null);
        bool ShowCommonDialog(CommonDialogViewModel viewModel);
        bool ShowCommonDialog(string title, string message, bool visibleCancel);
        HasChangesDialogResult ShowHasChangesDialog(string messageKey);
        NewFileDialogResult? ShowNewFileDialog();
    }
}
