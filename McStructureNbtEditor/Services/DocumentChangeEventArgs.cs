using System;
using System.Collections.Generic;
using System.Text;

namespace McStructureNbtEditor.Services
{
    public enum ChangeType
    {
        FullReload,
        EditorCommand
    }

    public class DocumentChangedEventArgs : EventArgs
    {
        public ChangeType ChangeType { get; set; }
    }
}
