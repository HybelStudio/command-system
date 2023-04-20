using System;

namespace Hybel.Commands
{
    /// <summary>
    /// Processor that can handle the undoing of commands.
    /// <para>Often used in tandem with <see cref="IRedoProcessor{TCommand}"/>.</para>
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be undone with this processor.</typeparam>
    public interface IUndoProcessor<TCommand> where TCommand : IUndoableCommand
    {
        /// <summary>
        /// Collection of all currently executed <see cref="TCommand"/>s.
        /// </summary>
        TCommand[] ExecutedCommands { get; }

        /// <summary>
        /// Amount of possible undos.
        /// </summary>
        int PossibleUndos { get; }

        /// <summary>
        /// Undo the last executed <see cref="TCommand"/>.
        /// </summary>
        /// <returns>The undone <see cref="TCommand"/>.</returns>
        TCommand Undo();

        /// <summary>
        /// Undo the last executed <see cref="TCommand"/>.
        /// </summary>
        /// <param name="redoAction">Custom undo behaviour.</param>
        /// <returns>The undone <see cref="TCommand"/>.</returns>
        TCommand Undo(Action<TCommand> undoAction);

        /// <summary>
        /// Record an executed <see cref="TCommand"/> to be undone in the future.
        /// </summary>
        void RecordUndo(TCommand command);

        /// <summary>
        /// Clear the entire undo Stack.
        /// </summary>
        void Clear();
    }
}