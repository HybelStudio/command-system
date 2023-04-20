using System.Collections.Generic;
using System.Linq;

namespace Hybel.Commands
{
    /// <summary>
    /// Functions as a normal <see cref="ICommand"/>, but contains other commands within it.
    /// </summary>
    /// <typeparam name="TCommand">Base type for command contained in the group.</typeparam>
    public class CommandGroup<TCommand> : ICommand, ICommandGroup<TCommand>
        where TCommand : ICommand
    {
        private readonly List<TCommand> _commands;

        /// <param name="commands">Collection of <see cref="TCommand"/>s to inject into the group.</param>
        /// <param name="sender">The object that sent the <see cref="CommandGroup{TCommand}"/>.</param>
        public CommandGroup(IEnumerable<TCommand> commands, object sender)
        {
            Sender = sender;
            _commands = commands.ToList();
        }

        public object Sender { get; }

        public IReadOnlyCollection<TCommand> Commands => _commands.AsReadOnly();

        public void Execute() => _commands.ForEach(command => command.Execute());
    }

    /// <summary>
    /// Functions as a normal <see cref="IUndoableCommand"/>, but contains other commands within it.
    /// </summary>
    /// <typeparam name="TCommand">Base type for command contained in the group.</typeparam>
    public class UndoableCommandGroup<TCommand> : IUndoableCommand, ICommandGroup<TCommand>
        where TCommand : IUndoableCommand
    {
        private readonly List<TCommand> _undoableCommands;

        /// <param name="undoableCommands">Collection of <see cref="TCommand"/>s to inject into the group.</param>
        /// <param name="sender">The object that sent the <see cref="UndoableCommandGroup{TCommand}"/>.</param>
        public UndoableCommandGroup(IEnumerable<TCommand> undoableCommands, object sender)
        {
            Sender = sender;
            _undoableCommands = undoableCommands.ToList();
        }

        public object Sender { get; }

        public IReadOnlyCollection<TCommand> Commands => _undoableCommands.AsReadOnly();

        public void Execute() => _undoableCommands.ForEach(command => command.Execute());

        public void Undo() => _undoableCommands.ForEach(command => command.Undo());
    }
}