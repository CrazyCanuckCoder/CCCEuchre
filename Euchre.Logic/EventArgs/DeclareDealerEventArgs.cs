using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class DeclareDealerEventArgs : System.EventArgs
{
    /// <summary>
    /// Constructor to add the dealer.
    /// </summary>
    /// <param name="dealer"></param>
    public DeclareDealerEventArgs(IPlayer dealer)
    {
        Dealer = dealer;
    }

    /// <summary>
    /// Gets the player that is the dealer of the current round.
    /// </summary>
    public IPlayer Dealer { get; }
}
