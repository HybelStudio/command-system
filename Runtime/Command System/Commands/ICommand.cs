namespace Hybel.Commands
{
    /// <summary>
    /// <para>By formal terminology this is a Command.</para>
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// The <see cref="object"/> which sent the <see cref="ICommand"/>.
        /// </summary>
        object Sender { get; }

        /// <summary>
        /// Execute the <see cref="ICommand"/>.
        /// </summary>
        void Execute();
    }
}