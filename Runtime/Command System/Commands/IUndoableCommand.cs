namespace Hybel.Commands
{
    /// <summary>
    /// Command with Undo/Redo fonctionality.
    /// <para>By formal terminology this is a Command.</para>
    /// </summary>
    public interface IUndoableCommand : ICommand
    {
        /// <summary>
        /// Undo the <see cref="IUndoableCommand"/>.
        /// </summary>
        void Undo();
    }
}