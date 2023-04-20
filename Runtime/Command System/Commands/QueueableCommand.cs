using System;

namespace Hybel.Commands
{
    /// <summary>
    /// A decorated <see cref="Command"/> that can be queued up for bulk execution.
    /// <para>They can still be executed normally without being queued up.</para>
    /// </summary>
    public abstract class QueueableCommand : Command, IQueueableCommand<QueueableCommand>
    {
        public abstract event Action<QueueableCommand> OnCommandCompleted;

        /// <param name="sender">The <see cref="object"/> which is sending this <see cref="ICommand"/>.</param>
        public QueueableCommand(object sender) : base(sender) { }
    }
}