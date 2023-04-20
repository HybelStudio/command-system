using System.Collections.Generic;
using Hybel.Generics;
using System.Linq;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Hybel.Commands
{
    /// <summary>
    /// A structure to queue execute, undo and redo commands to be executed sequentially
    /// <para>All commands passed will be processed in order of passing, including Undo and Redo.</para>
    /// <para>You can still execute single commands directly using this structure.</para>
    /// </summary>
    public partial class SequentialCommandProcessorWithUndoRedo<TCommand> : CommandProcessorWithUndoRedo<TCommand>, ISequentialCommandProcessor<TCommand>
        where TCommand : QueueableUndoableCommand
    {
        protected LimitedQueue<TCommand> _executeCommandQueue;                  // Commands that can be executed.
        protected LimitedQueue<DynamicCommand<TCommand>> _undoCommandQueue;     // Commands that can be undone.
        protected LimitedQueue<DynamicCommand<TCommand>> _redoCommandQueue;     // Commands that can be redone.
        protected Queue<CommandProcessingMode> _actionTypeQueue;                // Queue for which command-queue to pick from when executing an action.

        public TCommand[] ExecuteCommandQueue => _executeCommandQueue.ToArray();

        public bool IsExecuting { get; private set; } = false;

        protected int possibleUndos =>
            _undoProcessor.PossibleUndos +
            _executeCommandQueue.Count +
            _redoCommandQueue.Count -
            _undoCommandQueue.Count;

        protected int possibleRedos =>
            _redoProcessor.PossibleRedos +
            _undoCommandQueue.Count -
            _redoCommandQueue.Count;

        /// <param name="maxQueuedCommands">Maximum number of commands that can be queued up.</param>
        /// <param name="maxUndoSteps">How many undo action are saved.</param>
        public SequentialCommandProcessorWithUndoRedo(int maxQueuedCommands, int maxUndoSteps = 128) :
            base(maxUndoSteps)
        {
            _executeCommandQueue = new LimitedQueue<TCommand>(maxQueuedCommands, false);
            _undoCommandQueue = new LimitedQueue<DynamicCommand<TCommand>>(maxUndoSteps, true);
            _redoCommandQueue = new LimitedQueue<DynamicCommand<TCommand>>(maxUndoSteps, true);
            _actionTypeQueue = new Queue<CommandProcessingMode>();
        }

        public virtual void ExecuteAllCommands()
        {
            if (IsExecuting)
                return;

            IsExecuting = true;
            ExecuteNextCommand(null);
        }

        protected virtual void ExecuteNextCommand(QueueableUndoableCommand previousCommand)
        {
            if (previousCommand != null)
                previousCommand.OnCommandCompleted -= ExecuteNextCommand;

            if (_actionTypeQueue.Count <= 0)
            {
                IsExecuting = false;
                return;
            }

            CommandProcessingMode processingMode = _actionTypeQueue.Dequeue();

            if ((int)processingMode >= 3)
            {
#if CLOGGER
                this.LogError("Commands", $"Invalid ActionType: {commandActionType}");
#elif UNITY_EDITOR
                Debug.LogError($"Invalid CommandProcessingMode: {processingMode}");
#endif
                return;
            }

            switch (processingMode)
            {
                case CommandProcessingMode.Execute:
                    HandleExecute(); // HandleExecute recursively calls this method again.
                    return;

                case CommandProcessingMode.Undo:
                    HandleUndo(); // HandleUndo recursively calls this method again.
                    return;

                case CommandProcessingMode.Redo:
                    HandleRedo(); // HandleRedo recursively calls this method again.
                    return;

                default:
                    return;
            }
        }

        public virtual void QueueExecute(TCommand command)
        {
            _executeCommandQueue.Enqueue(command);
            _actionTypeQueue.Enqueue(CommandProcessingMode.Execute);
        }

        /// <summary>
        /// Queues a command for undoing.
        /// </summary>
        public virtual void QueueUndo()
        {
            if (possibleUndos <= 0)
                return;

            var command = new DynamicCommand<TCommand>();

            // Possible other solution
            //var command = _executeCommandQueue.Peek();

            _undoCommandQueue.Enqueue(command);
            _actionTypeQueue.Enqueue(CommandProcessingMode.Undo);
        }

        /// <summary>
        /// Queues a command for redoing.
        /// </summary>
        public virtual void QueueRedo()
        {
            if (possibleRedos <= 0)
                return;

            var command = new DynamicCommand<TCommand>();

            // Possible other solution
            //var command = _undoCommandQueue.Peek();

            _redoCommandQueue.Enqueue(command);
            _actionTypeQueue.Enqueue(CommandProcessingMode.Redo);
        }

        protected virtual void HandleExecute()
        {
            TCommand command = _executeCommandQueue.Dequeue();

            if (_actionTypeQueue.Count > 0)
                command.OnCommandCompleted += ExecuteNextCommand;

            _undoProcessor.RecordUndo(command);
            _redoProcessor.Clear();
            command.Execute();
        }

        protected virtual void HandleUndo()
        {
            DynamicCommand<TCommand> command = _undoCommandQueue.Dequeue();

            if (_undoProcessor.PossibleUndos <= 0)
                return;

            _undoProcessor.Undo(nextCommand =>
            {
                command.SetCommand(nextCommand);

                if (_actionTypeQueue.Count > 0)
                    command.OnCommandCompleted += ExecuteNextCommand;

                _redoProcessor.RecordRedo(command as TCommand);
                command.Undo();
            });
        }

        protected virtual void HandleRedo()
        {
            DynamicCommand<TCommand> command = _redoCommandQueue.Dequeue();

            if (_redoProcessor.PossibleRedos <= 0)
                return;

            _redoProcessor.Redo(nextCommand =>
            {
                command.SetCommand(nextCommand);

                if (_actionTypeQueue.Count > 0)
                    command.OnCommandCompleted += ExecuteNextCommand;

                command.Execute();
                _undoProcessor.RecordUndo(command as TCommand);
            });
        }
    }
}