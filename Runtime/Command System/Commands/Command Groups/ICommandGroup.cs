using System;
using System.Collections.Generic;

namespace Hybel.Commands
{
    /// <summary>
    /// Functions as a normal <see cref="ICommand"/>, but contains other commands within it.
    /// </summary>
    /// <typeparam name="TCommand">Base type for command contained in the group.</typeparam>
    public interface ICommandGroup<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Collection of all contained <see cref="TCommand"/>s.
        /// </summary>
        public IReadOnlyCollection<TCommand> Commands { get; }
    }

    /// <summary>
    /// Functions as a normal <see cref="IUndoableCommand"/>, but contains other commands within it.
    /// </summary>
    /// <typeparam name="TCommand">Base type for command contained in the group.</typeparam>
    public interface IQueueableCommandGroup<TCommand> : ICommandGroup<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Fired when all contained <see cref="TCommand"/>s are completed.
        /// </summary>
        public event Action OnAllCommandsCompleted;
    }
}