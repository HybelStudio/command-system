namespace Hybel.Commands
{

    /// <summary>
    /// Processor that handles the execution of commands in parallel (bulk) where undoing will all commands that was executed in parallel all at once.
    /// <para>By formal terminology this is an invoker.</para>
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be executed with this processor.</typeparam>
    public interface IParallelCommandProcessor<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Execute all currently queued <typeparamref name="TCommand"/>s.
        /// </summary>
        void ExecuteQueuedCommands();

        /// <summary>
        /// Queue a <paramref name="command"/>.
        /// </summary>
        void QueueCommand(TCommand command);
    }
}