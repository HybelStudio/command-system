namespace Hybel.Commands
{
    /// <summary>
    /// Command with Undo/Redo fonctionality.
    /// <para>By formal terminology this is a Command.</para>
    /// </summary>
    public abstract class UndoableCommand : Command, IUndoableCommand
    {
        /// <param name="sender">The <see cref="object"/> that is sending this <see cref="ICommand"/>.</param>
        protected UndoableCommand(object sender) : base(sender) { }

        public abstract void Undo();
    }
}