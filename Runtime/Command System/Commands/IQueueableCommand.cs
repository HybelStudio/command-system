using System;

namespace Hybel.Commands
{
    /// <summary>
    /// A decorator for <see cref="Command"/>s that could be queued up for bulk execution.
    /// <para>They can still be executed normally without being queued up.</para>
    /// <para>By formal terminology this is not a Command, but should always be implemented on a <see cref="Command"/>.</para>
    /// </summary>
    /// <typeparam name="TCommand">Base type for what <see cref="ICommand"/> can be queued.</typeparam>
    public interface IQueueableCommand<TCommand>
        where TCommand : IQueueableCommand<TCommand>
    {
        /// <summary>
        /// Fired when the <see cref="TCommand"/> has completed.
        /// </summary>
        event Action<TCommand> OnCommandCompleted;
    }
}