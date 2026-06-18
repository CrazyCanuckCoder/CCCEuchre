using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class GetPlayersCardsEventArgs : System.EventArgs
{
    /// <summary>
    /// The list of cards for player 1.
    /// </summary>
    public List<ICard> Player1Cards { get; set; }

    /// <summary>
    /// The list of cards for player 2.
    /// </summary>
    public List<ICard> Player2Cards { get; set; }

    /// <summary>
    /// The list of cards for player 3.
    /// </summary>
    public List<ICard> Player3Cards { get; set; }

    /// <summary>
    /// The list of cards for player 4.
    /// </summary>
    public List<ICard> Player4Cards { get; set; }

    /// <summary>
    /// The card to be used as the kitty.
    /// </summary>
    public Card KittyCard { get; set; }

    /// <summary>
    /// The list of cards not dealt to the players or set as the kitty card.
    /// </summary>
    public List<Card> RemainingCards { get; set; }
}