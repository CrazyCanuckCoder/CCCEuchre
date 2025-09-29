using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class CardPlayedByPlayerEventArgs : System.EventArgs
{
    /// <summary>
    /// A construct to set the properties of this class.
    /// </summary>
    /// <param name="player">The player that played the card.</param>
    /// <param name="cardPlayed">The card that the player played.</param>
    public CardPlayedByPlayerEventArgs(IPlayer player, ICard cardPlayed)
    {
        Player = player;
        CardPlayed = cardPlayed;
    }

    /// <summary>
    /// The player that played the card.
    /// </summary>
    public IPlayer Player { get; private set; }

    /// <summary>
    /// The card that the player played.
    /// </summary>
    public ICard CardPlayed { get; private set; }
}