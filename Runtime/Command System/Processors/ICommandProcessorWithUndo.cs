namespace Hybel.Commands
{
    /// <summary>
    /// Processor that also handles undoing of commands, but not redoing.
    /// <para>By formal terminology this is an invoker.</para>
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be executed with this processor.</typeparam>
    public interface ICommandProcessorWithUndo<TCommand> : ICommandProcessor<TCommand> where TCommand : IUndoableCommand
    {
        /// <summary>
        /// Collection of all executed <see cref="TCommand"/>s that can be undone.
        /// </summary>
        TCommand[] ExecutedCommands { get; }

        /// <summary>
        /// Undo the last executed <see cref="TCommand"/>.
        /// </summary>
        /// <returns>The undone <see cref="TCommand"/>.</returns>
        TCommand Undo();
    }
}