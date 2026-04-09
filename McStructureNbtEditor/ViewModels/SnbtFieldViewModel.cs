using McStructureNbtEditor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace McStructureNbtEditor.ViewModels
{
    public class SnbtFieldViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private string _selectedSnbt = string.Empty;
        public string SelectedSnbt
        {
            get => _selectedSnbt;
            set { _selectedSnbt = value; OnPropertyChanged(); }
        }

        public SnbtFieldViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.OnSnbtUpdated += UpdateSnbtText;
        }

        public void UpdateSnbtText(NbtTreeNode? node)
        {
            if (node == null)
            {
                SelectedSnbt = string.Empty;
                return;
            }

            SelectedSnbt = node.ToSnbtString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
