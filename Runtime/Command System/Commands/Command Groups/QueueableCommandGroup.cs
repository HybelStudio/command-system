using System;
using System.Collections.Generic;
using System.Linq;

namespace Hybel.Commands
{
    /// <summary>
    /// Functions as a normal <see cref="QueueableCommand"/>, but contains other commands within it.
    /// </summary>
    /// <typeparam name="TCommand">Base type for command contained in the group.</typeparam>
    public class QueueableCommandGroup<TCommand> : QueueableCommand, IQueueableCommandGroup<TCommand>
        where TCommand : ICommand, IQueueableCommand<QueueableCommand>
    {
        public event Action OnAllCommandsCompleted;
        public override event Action<QueueableCommand> OnCommandCompleted;

        private readonly List<TCommand> _commands;
        private int _completedCommands;
        private bool _allCompleted;

        public QueueableCommandGroup(IEnumerable<TCommand> commands, object sender) : base(sender)
        {
            _commands = commands.ToList();
            _commands.ForEach(command => command.OnCommandCompleted += OnCommandCompletedAction);

            _completedCommands = 0;
            _allCompleted = false;
        }

        ~QueueableCommandGroup() => _commands.ForEach(command => command.OnCommandCompleted -= OnCommandCompletedAction);

        public IReadOnlyCollection<TCommand> Commands => _commands.AsReadOnly();

        public override void Execute() => _commands.ForEach(command => command.Execute());

        private void OnCommandCompletedAction(QueueableCommand command)
        {
            OnCommandCompleted?.Invoke(command);
            _completedCommands++;

            if (_completedCommands < _commands.Count || _allCompleted)
                return;

            OnAllCommandsCompleted?.Invoke();
            _allCompleted = true;
        }
    }

    /// <summary>
    /// Functions as a normal <see cref="QueueableUndoableCommand"/>, but contains other commands within it.
    /// </summary>
    /// <typeparam name="TCommand">Base type for command contained in the group.</typeparam>
    public class QueueableUndoableCommandGroup<TCommand> : QueueableUndoableCommand, IQueueableCommandGroup<TCommand>
        where TCommand : IUndoableCommand, IQueueableCommand<QueueableUndoableCommand>
    {
        public event Action OnAllCommandsCompleted;
        public override event Action<QueueableUndoableCommand> OnCommandCompleted;

        private readonly List<TCommand> _commands;
        private int _completedCommands;
        private bool _allCompleted;

        public QueueableUndoableCommandGroup(IEnumerable<TCommand> commands, object sender) : base(sender)
        {
            _commands = commands.ToList();
            _commands.ForEach(command => command.OnCommandCompleted += OnCommandCompletedAction);

            _completedCommands = 0;
            _allCompleted = false;
        }

        ~QueueableUndoableCommandGroup() => _commands.ForEach(command => command.OnCommandCompleted -= OnCommandCompletedAction);

        public IReadOnlyCollection<TCommand> Commands => _commands.AsReadOnly();

        public override void Execute() => _commands.ForEach(command => command.Execute());

        public override void Undo() => _commands.ForEach(command => command.Undo());

        private void OnCommandCompletedAction(QueueableUndoableCommand command)
        {
            OnCommandCompleted?.Invoke(command);
            _completedCommands++;

            if (_completedCommands < _commands.Count || _allCompleted)
                return;

            OnAllCommandsCompleted?.Invoke();
            _allCompleted = true;
        }
    }
}