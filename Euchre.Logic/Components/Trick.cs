using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Components;

public class Trick
{
    public Trick(Suit trump)
    {
        Cards = [];
        Trump = trump;
    }

    /// <summary>
    /// The cards in the trick, indexed by the player who played them.
    /// </summary>
    public Dictionary<IPlayer, Card> Cards { get; }

    /// <summary>
    /// The suit of the first card played in the trick, which determines the lead suit.
    /// </summary>
    public Suit LeadSuit { get; private set; }

    /// <summary>
    /// The trump suit for the trick, which affects the value of cards played.
    /// </summary>
    public Suit Trump { get; }

    /// <summary>
    /// True to indicate the trick is complete, meaning all players have played a card.
    /// </summary>
    public bool IsComplete => Cards.Count == MAX_NUMBER_OF_TRICK_CARDS;

    /// <summary>
    /// Adds a card played by a player to the trick. If this is the first card played, it sets the lead suit.
    /// </summary>
    /// <param name="player">The player who played the card.</param>
    /// <param name="card">The card to add to the trick.</param>
    public void AddCard(IPlayer player, Card card)
    {
        if (Cards.Count == 0)
        {
            LeadSuit = card.EffectiveSuit(Trump);
        }
        Cards.Add(player, card);
    }

    /// <summary>
    /// Gets the player who won the trick.
    /// </summary>
    /// <returns>The IPlayer representing the player who won the trick.</returns>
    /// <exception cref="TrickIncompleteException"></exception>
    public IPlayer GetWinner()
    {
        if (!IsComplete) throw new TrickIncompleteException();

        return GetHighestCardInTrick().Key;
    }

    /// <summary>
    /// Gets the card that is currently leading the trick, which is the highest card played so far.
    /// </summary>
    /// <returns>The Card that is leading the trick.</returns>
    /// <exception cref="EmptyTrickException"></exception>
    public Card GetCurrentLeadingCard()
    {
        if (Cards.Count == 0) throw new EmptyTrickException();

        return GetHighestCardInTrick().Value;
    }

    /// <summary>
    /// Gets the player and card that has the highest value in the trick.
    /// </summary>
    /// <returns>A KeyValuePair containing the highest card and the player that played it.</returns>
    public KeyValuePair<IPlayer, Card> GetHighestCardInTrick()
    {
        return Cards.OrderByDescending(c => c.Value.GetTrickValue(Trump, LeadSuit))
                    .First();
    }
}
