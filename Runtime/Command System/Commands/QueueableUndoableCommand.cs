using System;

namespace Hybel.Commands
{
    /// <summary>
    /// A decorator for <see cref="UndoableCommand"/>s that could be queued up for bulk execution.
    /// <para>They can still be executed normally without being queued up.</para>
    /// <para>By formal terminology this is not a Command, but should always be implemented on a <see cref="UndoableCommand"/>.</para>
    /// </summary>
    public abstract class QueueableUndoableCommand : UndoableCommand, IQueueableCommand<QueueableUndoableCommand>
    {
        public abstract event Action<QueueableUndoableCommand> OnCommandCompleted;

        /// <param name="sender">The <see cref="object"/> which is sending this <see cref="ICommand"/>.</param>
        public QueueableUndoableCommand(object sender) : base(sender) { }
    }
}