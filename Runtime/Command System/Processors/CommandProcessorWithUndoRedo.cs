namespace Hybel.Commands
{
    /// <summary>
    /// A processor which can execute, undo and redo <see cref="IUndoableCommand"/>s one at a time.
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be undone with this processor.</typeparam>
    public class CommandProcessorWithUndoRedo<TCommand> : ICommandProcessorWithUndoRedo<TCommand>
        where TCommand : IUndoableCommand
    {
        protected IUndoProcessor<TCommand> _undoProcessor;
        protected IRedoProcessor<TCommand> _redoProcessor;

        public readonly int MaxUndoSteps;

        public TCommand[] ExecutedCommands => _undoProcessor.ExecutedCommands;
        public TCommand[] UndoneCommands => _redoProcessor.UndoneCommands;

        /// <param name="maxUndoSteps">How many undo actions are saved.</param>
        public CommandProcessorWithUndoRedo(int maxUndoSteps = 128)
        {
            _undoProcessor = new UndoProcessor<TCommand>(maxUndoSteps);
            _redoProcessor = new RedoProcessor<TCommand>(maxUndoSteps);
            MaxUndoSteps = maxUndoSteps;
        }

        /// <summary>
        /// Executes the <paramref name="command"/>.
        /// <para>Clears all previously undone commands so you can't redo after executing a new command.</para>
        /// </summary>
        public virtual void Execute(TCommand command)
        {
            command.Execute();
            _undoProcessor.RecordUndo(command);
            _redoProcessor.Clear();
        }

        /// <summary>
        /// Undoes the last executed command.
        /// </summary>
        public virtual TCommand Undo()
        {
            return _undoProcessor.Undo(command =>
            {
                command.Undo();
                _redoProcessor.RecordRedo(command);
            });
        }

        /// <summary>
        /// Redoes the last undone command.
        /// </summary>
        public virtual TCommand Redo()
        {
            return _redoProcessor.Redo(command =>
            {
                command.Execute();
                _undoProcessor.RecordUndo(command);
            });
        }
    }
}
