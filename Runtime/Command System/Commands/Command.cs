using System.Collections.Generic;

namespace Hybel.Commands
{
    /// <summary>
    /// <para>By formal terminology this is a Command.</para>
    /// </summary>
    public abstract class Command : ICommand
    {
        public virtual object Sender { get; }

        /// <param name="sender">The <see cref="object"/> that is sending this <see cref="ICommand"/>.</param>
        protected Command(object sender) => Sender = sender;

        public abstract void Execute();
    }
}