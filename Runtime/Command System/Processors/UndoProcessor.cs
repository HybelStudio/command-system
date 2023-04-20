using Hybel.ExtensionMethods;
using Hybel.Generics;
using System;

namespace Hybel.Commands
{
    public class UndoProcessor<TCommand> : IUndoProcessor<TCommand>
        where TCommand : IUndoableCommand
    {
        protected LimitedStack<TCommand> _executedCommands;

        public UndoProcessor(int maxUndoSteps = 128) =>
            _executedCommands = new LimitedStack<TCommand>(maxUndoSteps, true, OverrideMode.OldestEntry);

        public virtual TCommand[] ExecutedCommands => _executedCommands.ToArray();

        public int PossibleUndos => _executedCommands.Count;

        public virtual void RecordUndo(TCommand command) => _executedCommands.Push(command);

        public virtual TCommand Undo() => Undo(command => command.Undo());

        public virtual TCommand Undo(Action<TCommand> action)
        {
            if (_executedCommands.IsEmpty())
                return default;

            TCommand command = _executedCommands.Pop();
            action(command);
            return command;
        }

        public virtual void Clear() => _executedCommands.Clear();
    }
}
