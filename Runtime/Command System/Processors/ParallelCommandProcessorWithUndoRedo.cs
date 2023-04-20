using Hybel.Generics;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Hybel.Commands
{
    /// <summary>
    /// A processor to queue execute, undo and redo commands in parallell.
    /// <para>All commands passed will be processed in order of passing, including Undo and Redo.</para>
    /// <para>You can still execute single commands directly using this processor.</para>
    /// <para>This type of <see cref="ICommandProcessorWithUndo{TCommand}"/> may return an <see cref="ICommandGroup{TCommand}"/> when using <see cref="Undo"/> or <see cref="Redo"/>.</para>
    /// </summary>
    public class ParallelCommandProcessorWithUndoRedo<TCommand> : CommandProcessorWithUndoRedo<TCommand>, IParallelCommandProcessor<TCommand>
        where TCommand : QueueableUndoableCommand
    {
        protected LimitedList<TCommand> _commandsList;                      // Commands that can be executed.
        protected LimitedStack<LimitedList<TCommand>> _executedQueues;     // Queues of commands that can be undone.
        protected LimitedStack<LimitedList<TCommand>> _undoneQueues;       // Queues of commands that can be redone.
        protected LimitedStack<ExecutionType> _undoTypeStack;                      // Stack for which undo-stack to pick from when undoing a command.
        protected LimitedStack<ExecutionType> _redoTypeStack;                      // Stack for which redo-stack to pick from when redoing a command.

        /// <param name="maxQueuedCommands">Maximum number of commands that can be queued up.</param>
        /// <param name="maxUndoSteps">How many undo action are saved.</param>
        public ParallelCommandProcessorWithUndoRedo(int maxQueuedCommands, int maxUndoSteps = 32) :
            base(maxUndoSteps)
        {
            _commandsList = new LimitedList<TCommand>(maxQueuedCommands, false);
            _executedQueues = new LimitedStack<LimitedList<TCommand>>(maxUndoSteps, true, OverrideMode.OldestEntry);
            _undoneQueues = new LimitedStack<LimitedList<TCommand>>(maxUndoSteps, true, OverrideMode.OldestEntry);
        }

        public virtual void QueueCommand(TCommand command) =>
            _commandsList.Add(command);

        public override void Execute(TCommand command)
        {
            _undoTypeStack.Push(ExecutionType.Single);
            base.Execute(command);
        }

        public virtual void ExecuteQueuedCommands()
        {
            if (_commandsList.Count <= 0)
                return;

            _undoTypeStack.Push(ExecutionType.Multiple);

            foreach (TCommand command in _commandsList)
            {
                _commandsList.Remove(command);
                command.Execute();
                _redoProcessor.Clear();
                _undoneQueues.Clear();
            }
        }

        public override TCommand Undo()
        {
            if (_undoTypeStack.Count <= 0)
                return null;

            ExecutionType doType = _undoTypeStack.Pop();
            switch (doType)
            {
                case ExecutionType.Single:
                    return _undoProcessor.Undo(command =>
                    {
                        command.Undo();
                        _redoTypeStack.Push(ExecutionType.Single);
                        _redoProcessor.RecordRedo(command);
                    });

                case ExecutionType.Multiple:
                    {
                        LimitedList<TCommand> commands = _executedQueues.Pop();

                        commands.ForEach(c => c.Undo());
                        commands.Clear();

                        _redoTypeStack.Push(ExecutionType.Multiple);
                        _undoneQueues.Push(commands);

                        var commandGroup = new QueueableUndoableCommandGroup<TCommand>(commands, this);
                        return (TCommand)(QueueableUndoableCommand)commandGroup;
                    }

                default:
#if CLOGGER
                    global::Clogger.LogError<ParallelCommandProcessorWithUndoRedo<TCommand>>("Command System", $"Invalid DoType: {doType}.");
#elif UNITY_EDITOR
                    Debug.LogError($"Invalid DoType: {doType}.");
#endif
                    return null;
            }
        }

        public override TCommand Redo()
        {
            if (_redoTypeStack.Count <= 0)
                return null;

            ExecutionType doType = _redoTypeStack.Pop();
            switch (doType)
            {
                case ExecutionType.Single:
                    return _redoProcessor.Redo(command =>
                    {
                        command.Execute();
                        _undoTypeStack.Push(ExecutionType.Single);
                        _undoProcessor.RecordUndo(command);
                    });

                case ExecutionType.Multiple:
                    {
                        LimitedList<TCommand> commands = _undoneQueues.Pop();

                        commands.ForEach(c => c.Execute());
                        commands.Clear();

                        _undoTypeStack.Push(ExecutionType.Multiple);
                        _executedQueues.Push(commands);

                        var commandGroup = new QueueableUndoableCommandGroup<TCommand>(commands, this);
                        return (TCommand)(QueueableUndoableCommand)commandGroup;
                    }

                default:
#if CLOGGER
                    global::Clogger.LogError<ParallelCommandProcessorWithUndoRedo<TCommand>>("Command System", $"Invalid DoType: {doType}.");
#elif UNITY_EDITOR
                    Debug.LogError($"Invalid DoType: {doType}.");
#endif
                    return null;
            }
        }

        protected enum ExecutionType
        {
            Single,
            Multiple
        }
    }
}
