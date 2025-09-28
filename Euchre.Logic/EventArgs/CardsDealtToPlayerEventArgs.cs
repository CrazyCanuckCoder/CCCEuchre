using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class CardsDealtToPlayerEventArgs : System.EventArgs
{
    /// <summary>
    /// Constructor to set the player.
    /// </summary>
    /// <param name="player">The player that was just dealt some cards.</param>
    /// <param name="numberOfCardsDealt">The number of cards dealt to the player.</param>
    public CardsDealtToPlayerEventArgs(IPlayer player, int numberOfCardsDealt)
    {
        Player = player;
        NumberOfCardsDealt = numberOfCardsDealt;
    }

    /// <summary>
    /// Gets the player that was just dealt some cards.
    /// </summary>
    public IPlayer Player { get; }

    /// <summary>
    /// Gets the number of cards that were dealt to the player.
    /// </summary>
    public int NumberOfCardsDealt { get; }
}