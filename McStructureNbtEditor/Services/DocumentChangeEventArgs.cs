using System;
using System.Collections.Generic;
using System.Text;

namespace McStructureNbtEditor.Services
{
    [Flags]
    public enum ReloadScope
    {
        ReloadFile,
        ReloadAll,
        ReloadBlock
    }

    public class DocumentChangedEventArgs : EventArgs
    {
        public ReloadScope ChangeType { get; set; }
    }
}
