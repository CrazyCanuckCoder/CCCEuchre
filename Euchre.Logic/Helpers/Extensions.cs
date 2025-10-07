using Euchre.Logic.Components;
using System.Collections.Generic;

namespace Euchre.Logic.Helpers;

public static class Extensions
{
    /// <summary>
    /// Determines if the list of cards has any cards of a particular suit that haven't been played. 
    /// </summary>
    /// <remarks>Does not look at trump cards.  Use the HasAnyTrump() method to determine if the cards have
    /// any trump cards.
    /// </remarks>
    /// <param name="cards">The list of cards to check.</param>
    /// <param name="suitToFind">The suit of the cards to find.</param>
    /// <returns>True indicating the list has at least one card in the specified suit.</returns>
    public static bool HasAnyOfSuit(this List<Card> cards, Suit suitToFind)
    {
        return CardFinder.HasACardOfSuit(cards, suitToFind);
    }

    /// <summary>
    /// Determines if the list of cards has any trump cards regardless of whether or not they have been 
    /// played.
    /// </summary>
    /// <param name="cards">The list of cards to check.</param>
    /// <returns>True indicating the list has at least one trump card.</returns>
    public static bool HasAnyTrump(this List<Card> cards, Suit trumpSuit)
    {
        return CardFinder.HasTrump(cards, trumpSuit);
    }

    /// <summary>
    /// Returns true to indicate a specified card is trump.
    /// </summary>
    /// <param name="card">The card to test if it is a trump card.</param>
    /// <param name="trumpSuit">The suit considered trump.</param>
    /// <returns>True if the card is a trump card and false if it is not.</returns>
    public static bool IsTrump(this Card card, Suit trumpSuit)
    {
        return card.EffectiveSuit(trumpSuit) == trumpSuit;
    }

    public static List<Card> Sort(this List<Card> cards, Suit? trump)
    {
        List<Card> sortedCards = [];

        var suitOrder = GetComplementarySuits(trump);

        foreach (Suit suit in suitOrder)
        {
            if (trump.HasValue && suit == trump)
            {
                sortedCards.AddRange(    from card in cards
                                        where card.Suit == suit
                                      orderby card.GetTrickValue(trump.Value, suit) descending
                                       select card);
            }
            else
            {
                sortedCards.AddRange(   from card in cards
                                       where card.Suit == suit
                                     orderby card.Rank descending
                                      select card);
            }
        }

        return sortedCards;
    }

    /// <summary>
    /// Based on a trump suit, determines the sorting order of the rest of the suits.
    /// </summary>
    /// <param name="trumpSuit">The current trump suit.</param>
    /// <returns>A list of SuitTypes in an order that alternates the suit colour compared to the trump suit.</returns>
    private static List<Suit> GetComplementarySuits(Suit? trumpSuit)
    {
        List<Suit> suitTypes = [];

        if (trumpSuit == null)
        {
            suitTypes.Add(Suit.Clubs);
            suitTypes.Add(Suit.Hearts);
            suitTypes.Add(Suit.Spades);
            suitTypes.Add(Suit.Diamonds);
        }
        else
        {
            // Start with the trump suit then add the rest of the suits.

            suitTypes.Add(trumpSuit.Value);

            switch (trumpSuit)
            {
                case Suit.Clubs:
                    suitTypes.Add(Suit.Hearts);
                    suitTypes.Add(Suit.Spades);
                    suitTypes.Add(Suit.Diamonds);
                    break;

                case Suit.Hearts:
                    suitTypes.Add(Suit.Spades);
                    suitTypes.Add(Suit.Diamonds);
                    suitTypes.Add(Suit.Clubs);
                    break;

                case Suit.Spades:
                    suitTypes.Add(Suit.Hearts);
                    suitTypes.Add(Suit.Clubs);
                    suitTypes.Add(Suit.Diamonds);
                    break;

                case Suit.Diamonds:
                    suitTypes.Add(Suit.Clubs);
                    suitTypes.Add(Suit.Hearts);
                    suitTypes.Add(Suit.Spades);
                    break;
            }
        }

        return suitTypes;
    }
}
