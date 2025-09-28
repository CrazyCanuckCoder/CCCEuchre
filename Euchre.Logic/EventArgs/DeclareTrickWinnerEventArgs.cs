using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class DeclareTrickWinnerEventArgs : System.EventArgs
{
    /// <summary>
    /// A constructor to set this class' property.
    /// </summary>
    /// <param name="trickWinner">The player that won the most recent trick.</param>
    public DeclareTrickWinnerEventArgs(IPlayer trickWinner)
    {
        TrickWinningPlayer = trickWinner;
    }

    /// <summary>
    /// The player that won the most recent trick.
    /// </summary>
    public IPlayer TrickWinningPlayer { get; private set; }
}