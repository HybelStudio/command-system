namespace Hybel.Commands
{
    public interface ISequentialCommandProcessor<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Collection of currently queued <see cref="TCommand"/>s.
        /// </summary>
        TCommand[] ExecuteCommandQueue { get; }

        /// <summary>
        /// Is the processor currently executing?
        /// </summary>
        bool IsExecuting { get; }

        /// <summary>
        /// Execute all currently queued <see cref="TCommand"/>s.
        /// </summary>
        void ExecuteAllCommands();

        /// <summary>
        /// Queue a <see cref="TCommand"/> to be executed in the future.
        /// </summary>
        void QueueExecute(TCommand command);
    }
}