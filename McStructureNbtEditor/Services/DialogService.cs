using McStructureNbtEditor.Services.DialogResults;
using McStructureNbtEditor.ViewModels;
using McStructureNbtEditor.Views;


namespace McStructureNbtEditor.Services
{
    public sealed class DialogService : IDialogService
    {
        public AddPaletteDialogResult ShowAddPaletteDialog()
        {
            var vm = new AddPaletteDialogViewModel();

            var view = new AddPaletteDialog(vm);

            var result = view.ShowDialog();

            return new AddPaletteDialogResult
            {
                Confirmed = result == true,
                Draft = vm.Result
            };
        }
    }
}
