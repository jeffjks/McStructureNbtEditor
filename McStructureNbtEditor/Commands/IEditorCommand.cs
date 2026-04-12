using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public interface IEditorCommand
    {
        string Description { get; }
        bool Execute(EditorSession session);
        void Undo(EditorSession session);
        ReloadScope ChangeType { get; }
    }
}
