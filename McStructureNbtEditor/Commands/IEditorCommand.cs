using McStructureNbtEditor.Services;

namespace McStructureNbtEditor.Commands
{
    public interface IEditorCommand
    {
        string Description { get; }
        bool CanExecute(EditorSession session);
        void Execute(EditorSession session);
        void Undo(EditorSession session);
    }
}
