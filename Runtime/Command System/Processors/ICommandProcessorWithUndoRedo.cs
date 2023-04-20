namespace Hybel.Commands
{
    /// <summary>
    /// Processor that also handles undoing and redoing of commands.
    /// <para>By formal terminology this is an invoker.</para>
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be executed with this processor.</typeparam>
    public interface ICommandProcessorWithUndoRedo<TCommand> : ICommandProcessorWithUndo<TCommand> where TCommand : IUndoableCommand
    {
        /// <summary>
        /// Collection of all undone <see cref="TCommand"/>s that can be redone.
        /// </summary>
        TCommand[] UndoneCommands { get; }

        /// <summary>
        /// Redo the last undone <see cref="TCommand"/>.
        /// </summary>
        /// <returns>The redone <see cref="TCommand"/>.</returns>
        TCommand Redo();
    }
}