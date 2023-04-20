namespace Hybel.Commands
{
    /// <summary>
    /// <para>By formal terminology this is an invoker.</para>
    /// </summary>
    /// <typeparam name="TCommand">The base type for commands that can be executed with this processor.</typeparam>
    public interface ICommandProcessor<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Execute a <paramref name="command"/>.
        /// </summary>
        void Execute(TCommand command);
    }
}