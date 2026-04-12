using McStructureNbtEditor.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace McStructureNbtEditor.Commands
{
    public class CompositeCommand : IEditorCommand
    {
        private readonly List<IEditorCommand> _commands = new();

        public string Description { get; }

        public CompositeCommand(string description)
        {
            Description = description;
        }

        public void Add(IEditorCommand command)
        {
            _commands.Add(command);
        }

        public bool Execute(EditorSession session)
        {
            if (session.CurrentStructure == null)
                return false;

            var executed = new List<IEditorCommand>();

            foreach (var command in _commands)
            {
                if (!command.Execute(session))
                {
                    for (int i = executed.Count - 1; i >= 0; i--)
                        executed[i].Undo(session);

                    return false;
                }

                executed.Add(command);
            }

            return true;
        }

        public void Undo(EditorSession session)
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
                _commands[i].Undo(session);
        }
    }
}
