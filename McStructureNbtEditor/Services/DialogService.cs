using McStructureNbtEditor.Models;
using McStructureNbtEditor.Services.DialogResults;
using McStructureNbtEditor.ViewModels;
using McStructureNbtEditor.Views;

namespace McStructureNbtEditor.Services
{
    public sealed class DialogService : IDialogService
    {
        public PaletteDialogResult ShowPaletteDialog(PaletteMode mode, PaletteEntry? defaultPaletteEntry = null)
        {
            var vm = new PaletteDialogViewModel(mode, defaultPaletteEntry);
            var view = new PaletteDialogView(vm);

            var result = view.ShowDialog();

            return new PaletteDialogResult
            {
                Confirmed = result == true,
                Draft = vm.Result
            };
        }

        public bool ShowCommonDialog(CommonDialogViewModel viewModel)
        {
            var view = new CommonDialogView(viewModel);
            var result = view.ShowDialog();

            return result ?? false;
        }

        public bool ShowCommonDialog(string title, string message, bool visibleCancel)
        {
            var vm = new CommonDialogViewModel(title, message, visibleCancel);
            var view = new CommonDialogView(vm);

            var result = view.ShowDialog();

            return result ?? false;
        }

        public HasChangesDialogResult ShowHasChangesDialog()
        {
            var vm = new HasChangesDialogViewModel();
            var view = new HasChangesDialogView(vm);

            var result = view.ShowDialog();

            return vm.Result;
        }
    }
}
