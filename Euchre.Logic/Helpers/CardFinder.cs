using CrazyCanuckCoder.Library.Common;
using Euchre.Logic.Components;
using System.Windows.Controls.Primitives;

namespace Euchre.Logic.Helpers;

/// <summary>
/// Finds a card or cards in a collection based on various criteria.
/// </summary>
internal static class CardFinder
{
    /// <summary>
    /// Determines whether the specified collection of cards contains at least one card of the given suit.
    /// </summary>
    /// <param name="cards">The collection of cards to search. Cannot be <see langword="null"/>.</param>
    /// <param name="suit">The suit to check for in the collection.</param>
    /// <returns>True if the collection contains at least one card of the specified suit; otherwise, false.</returns>
    public static bool HasACardOfSuit(List<Card> cards, Suit suit, Suit trump)
    {
        return cards.Any(c => c.EffectiveSuit(trump) == suit);
    }

    /// <summary>
    /// Determines whether the specified collection of cards contains at least one card of the trump suit.
    /// </summary>
    /// <param name="cards">The collection of cards to evaluate. Cannot be null.</param>
    /// <param name="trump">The suit to check for as the trump suit.</param>
    /// <returns>True if the collection contains at least one card with the specified trump suit; otherwise,
    /// false.</returns>
    public static bool HasTrump(List<Card> cards, Suit trump)
    {
        return cards.Any(c => c.EffectiveSuit(trump) == trump);
    }

    /// <summary>
    /// Determines whether the specified collection of cards contains a bower.
    /// </summary>
    /// <param name="cards">The collection of cards to search.</param>
    /// <param name="trump">The trump suit used to identify the bower.</param>
    /// <returns>True if the collection contains a bower for the specified trump suit; otherwise, false.</returns>
    public static bool HasBower(List<Card> cards, Suit trump)
    {
        return cards.Any(c => c.IsBower(trump));
    }

    /// <summary>
    /// Determines whether the specified collection of cards contains at least one card with a suit that is
    /// not the trump suit.
    /// </summary>
    /// <param name="cards">The collection of cards to evaluate.</param>
    /// <param name="trump">The trump suit used to determine the off suit cards.</param>
    /// <returns>True if the collection contains at least one card with a suit that is not the trump suit; 
    /// otherwise, false.</returns>
    public static bool HasOffSuit(List<Card> cards, Suit trump)
    {
        return cards.Any(c => c.EffectiveSuit(trump) != trump);
    }

    /// <summary>
    /// Retrieves the highest ranked card from the specified list that is not of the trump suit.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="trump">The trump suit to exclude from consideration.</param>
    /// <returns>The highest ranked card that is not of the trump suit, or null if no such card exists.</returns>
    public static Card? GetHighestOffSuitCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) != trump)
                .OrderByDescending(c => (int)c.Rank)
                .FirstOrNull();
    }

    /// <summary>
    /// Retrieves the lowest ranked card from the specified list that is not of the trump suit.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="trump">The trump suit to exclude from consideration.</param>
    /// <returns>The lowest ranked card that is not of the trump suit, or null if no such card exists.</returns>
    public static Card? GetLowestOffSuitCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) != trump)
                .OrderByDescending(c => (int)c.Rank)
                .LastOrNull();
    }

    /// <summary>
    /// Retrieves the highest trump card from the specified list of cards.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="trump">The suit considered trump.</param>
    /// <returns>The highest ranked card of the trump suit, or null if no such card exists.</returns>
    public static Card? GetHighestTrumpCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) == trump)
                .OrderBy(c => c.GetTrickValue(trump, trump))
                .FirstOrNull();
    }

    /// <summary>
    /// Retrieves the lowest trump card from the specified list of cards.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="trump">The suit considered trump.</param>
    /// <returns>The lowest ranked card of the trump suit, or null if no such card exists.</returns>
    public static Card? GetLowestTrumpCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) == trump)
                .OrderBy(c => c.GetTrickValue(trump, trump))
                .FirstOrNull();
    }

    /// <summary>
    /// Retrieves the highest ranked card of the specified suit from the given list of cards.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="suit">The suit of the card to look for.</param>
    /// <returns>The highest ranked card of the specified suit, or null if no such card exists.</returns>
    public static Card? GetHighestCardOfSuit(List<Card> cards, Suit? suit, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) == suit)
                .OrderByDescending(c => (int)c.Rank)
                .FirstOrNull();
    }

    /// <summary>
    /// Retrieves the highest bower card from the specified list of cards.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="trump">The suit considered trump.</param>
    /// <returns>The highest bower card or null if one is not found.</returns>
    public static Card? GetHighestBowerCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.IsBower(trump))
                .OrderByDescending(c => c.GetTrickValue(trump, trump))
                .FirstOrNull();
    }

    /// <summary>
    /// Counts the number of cards in the specified list that match the given suit except for the Jacks.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="suit">The suit of the cards to count.  Does not work for trump.</param>
    /// <returns>The number of cards found for the specified suit.</returns>
    public static int CountCardsOfSuitLessJacks(List<Card> cards, Suit suit)
    {
        return cards.Count(c => c.Suit == suit && c.Rank != Rank.Jack);
    }

    /// <summary>
    /// Counts the number of cards in the specified list that match the given rank.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="rank">The rank of cards to count.</param>
    /// <returns>The number of cards found for the specified rank.</returns>
    public static int CountCardsOfRank(List<Card> cards, Rank rank)
    {
        return cards.Count(c => c.Rank == rank);
    }

    /// <summary>
    /// Counts the number of cards in the specified list that match the given rank and are not trump.
    /// </summary>
    /// <param name="cards">The list of cards to evaluate. Cannot be null.</param>
    /// <param name="rank">The rank of cards to count.</param>
    /// <param name="trump">The suit that is trump for the current round.</param>
    /// <returns>The number of cards found for the specified rank.</returns>
    public static int CountNonTrumpCardsOfRank(List<Card> cards, Rank rank, Suit trump)
    {
        return cards.Count(c => c.Rank == rank && c.Suit != trump);
    }

    /// <summary>
    /// Returns the number of bowers in a list of cards.
    /// </summary>
    /// <param name="cards">The list of cards containing bowers.</param>
    /// <param name="trump">The suit that is considered trump.</param>
    /// <returns>The number of bowers found.</returns>
    public static int CountBowers(List<Card> cards, Suit trump)
    {
        return cards.Count(c => c.IsBower(trump));
    }

    /// <summary>
    /// Retrieves the bowers from a list of cards.
    /// </summary>
    /// <param name="cards">The list of cards containing bowers.</param>
    /// <param name="trump">The suit that is considered trump.</param>
    /// <returns>A List containing the found bowers.</returns>
    public static List<Card> GetBowers(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.IsBower(trump))
                .OrderByDescending(c => c.GetTrickValue(trump, trump))
                .ToList();
    }

    /// <summary>
    /// Returns the trump cards from a list of cards that are not bowers.
    /// </summary>
    /// <param name="cards">The list of cards containing bowers.</param>
    /// <param name="trump">The suit that is considered trump.</param>
    /// <returns>A List containing non bower trump cards.</returns>
    public static List<Card> GetNonBowerTrump(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => !c.IsBower(trump) && c.EffectiveSuit(trump) == trump)
                .OrderByDescending(c => c.GetTrickValue(trump, trump))
                .ToList();
    }
}
