namespace Hybel.Commands
{
    /// <summary>
    /// How to process the next command.
    /// <para>
    /// Mainly used in systems that needs to keep track of <see cref="QueueableCommand"/>s.
    /// This way the system knows when to execute undo or redo as its next action.
    /// </para>
    /// </summary>
    public enum CommandProcessingMode
    {
        Execute = 0,
        Undo = 1,
        Redo = 2
    }
}