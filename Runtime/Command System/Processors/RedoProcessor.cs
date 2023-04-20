using Hybel.ExtensionMethods;
using Hybel.Generics;
using System;

namespace Hybel.Commands
{
    /// <summary>
    /// Processor that can handle the redoing of commands.
    /// <para>Should always be used in tandem with an <see cref="IUndoProcessor{TCommand}"/>.</para>
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be redone with this processor.</typeparam>
    public class RedoProcessor<TCommand> : IRedoProcessor<TCommand>
        where TCommand : IUndoableCommand
    {
        protected LimitedStack<TCommand> _undoneCommands;

        public RedoProcessor(int maxUndoSteps = 128) =>
            _undoneCommands = new LimitedStack<TCommand>(maxUndoSteps, true, OverrideMode.OldestEntry);

        public virtual TCommand[] UndoneCommands => _undoneCommands.ToArray();

        public int PossibleRedos => _undoneCommands.Count;

        public virtual void RecordRedo(TCommand command) => _undoneCommands.Push(command);

        public virtual TCommand Redo() => Redo(command => command.Execute());

        public virtual TCommand Redo(Action<TCommand> redoAction)
        {
            if (_undoneCommands.IsEmpty())
                return default;

            TCommand command = _undoneCommands.Pop();
            redoAction(command);
            return command;
        }

        public virtual void Clear() => _undoneCommands.Clear();
    }
}
