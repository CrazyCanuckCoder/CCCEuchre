using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class CardPlayedByPlayerEventArgs : System.EventArgs
{
    /// <summary>
    /// A construct to set the properties of this class.
    /// </summary>
    /// <param name="player">The player that played the card.</param>
    /// <param name="cardPlayed">The card that the player played.</param>
    /// <param name="currentTrick">The current trick being played.</param>
    public CardPlayedByPlayerEventArgs(IPlayer player, ICard cardPlayed, Trick currentTrick)
    {
        Player = player;
        CardPlayed = cardPlayed;
        CurrentTrick = currentTrick;
    }

    /// <summary>
    /// The player that played the card.
    /// </summary>
    public IPlayer Player { get; private set; }

    /// <summary>
    /// The card that the player played.
    /// </summary>
    public ICard CardPlayed { get; private set; }

    /// <summary>
    /// The current trick being played.
    /// </summary>
    public Trick CurrentTrick { get; private set; }
}