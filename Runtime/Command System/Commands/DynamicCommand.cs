using System;
#if UNITY_EDITOR
using UnityEngine;
#endif

namespace Hybel.Commands
{
    public partial class SequentialCommandProcessorWithUndoRedo<TCommand>
    {
        /// <summary>
        /// Use this command if you you don't know what command to execute until runtime.
        /// <para>Used when queueing Undos and Redos since its hard to know what command is meant to be undone or redone before they are executed in the first place.</para>
        /// </summary>
        //// NOTE: This class feels like an over-engineered solution to a simple problem that might be solved with just a bit of caching of commands when queueing them.
        protected class DynamicCommand<TQueueableCommand> : QueueableUndoableCommand where TQueueableCommand : QueueableUndoableCommand
        {
            public override event Action<QueueableUndoableCommand> OnCommandCompleted;

            private object _sender;
            private TQueueableCommand _command;
            private bool _commandIsSet = false;

            public TQueueableCommand Command
            {
                get => _command;
                private set
                {
                    _command = value;
#if CLOGGER
                    this.Log("Commands", $"Dynamic Command: {(_command == null ? "null" : _command.ToString())}");
#elif UNITY_EDITOR
                    Debug.Log($"Dynamic Command: {(_command == null ? "null" : _command)}");
#endif
                }
            }

            public override object Sender => _sender;

            /// <param name="sender">This is temporary. Make sure you call SetCommand on the DynamicCommand before using it.</param>
            public DynamicCommand(object sender = null) : base(sender) { }

            public void SetCommand(TQueueableCommand command)
            {
                if (_commandIsSet)
                    return;

                _commandIsSet = true;
                Command = command;
                _sender = command.Sender;

                Command.OnCommandCompleted += previuosCommand =>
                OnCommandCompleted?.Invoke(previuosCommand);
            }

            public override void Execute() => Command.Execute();

            public override void Undo() => Command.Undo();
        }
    }
}