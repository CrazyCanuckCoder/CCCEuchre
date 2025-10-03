using Euchre.Logic.Components;

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

    public static bool IsTrump(this Card card, Suit trumpSuit)
    {
        return card.EffectiveSuit(trumpSuit) == trumpSuit;
    }
}
