using McStructureNbtEditor.Services.DialogResults;
using McStructureNbtEditor.ViewModels;
using McStructureNbtEditor.Views;


namespace McStructureNbtEditor.Services
{
    public sealed class DialogService : IDialogService
    {
        public AddPaletteDialogResult ShowAddPaletteDialog(string? initialName = null)
        {
            var vm = new AddPaletteDialogViewModel
            {
                PaletteName = initialName ?? ""
            };

            var view = new AddPaletteDialog
            {
                DataContext = vm
            };

            var result = view.ShowDialog();

            return new AddPaletteDialogResult
            {
                Confirmed = result == true,
                PaletteName = vm.PaletteName?.Trim() ?? ""
            };
        }
    }
}
