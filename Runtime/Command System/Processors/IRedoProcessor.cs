using System;

namespace Hybel.Commands
{
    /// <summary>
    /// Processor that can handle the redoing of commands.
    /// <para>Should always be used in tandem with <see cref="IUndoProcessor{TCommand}"/>.</para>
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be redone with this processor.</typeparam>
    public interface IRedoProcessor<TCommand> where TCommand : IUndoableCommand
    {
        /// <summary>
        /// Collection of all currently undone <see cref="TCommand"/>s.
        /// </summary>
        TCommand[] UndoneCommands { get; }

        /// <summary>
        /// Amount of possible redos.
        /// </summary>
        int PossibleRedos { get; }

        /// <summary>
        /// Redo the last undone <see cref="TCommand"/>.
        /// </summary>
        /// <returns>The redone <see cref="TCommand"/>.</returns>
        TCommand Redo();

        /// <summary>
        /// Redo the last undone <see cref="TCommand"/>.
        /// </summary>
        /// <param name="redoAction">Custom redo behaviour.</param>
        /// <returns>The redone <see cref="TCommand"/>.</returns>
        TCommand Redo(Action<TCommand> redoAction);

        /// <summary>
        /// Record an undone <see cref="TCommand"/> to be redone in the future.
        /// </summary>
        void RecordRedo(TCommand command);

        /// <summary>
        /// Clear the entire redo Stack.
        /// </summary>
        void Clear();
    }
}