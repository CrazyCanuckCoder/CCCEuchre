using CrazyCanuckCoder.Library.Common;
using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Helpers;

public static class CardFinder
{

    public static bool HasACardOfSuit(List<Card> cards, Suit suit)
    {
        return cards.Any(c => c.Suit == suit);
    }

    public static bool HasTrump(List<Card> cards, Suit trump)
    {
        return cards.Any(c => c.EffectiveSuit(trump) == trump);
    }

    public static bool HasBower(List<Card> cards, Suit trump)
    {
        return cards.Any(c => c.IsBower(trump));
    }

    public static bool HasOffSuit(List<Card> cards, Suit trump)
    {
        return cards.Any(c => c.EffectiveSuit(trump) != trump);
    }

    public static Card? GetHighestOffSuitCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) != trump)
                .OrderByDescending(c => (int)c.Rank)
                .FirstOrNull();
    }

    public static Card? GetLowestOffSuitCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) != trump)
                .OrderByDescending(c => (int)c.Rank)
                .LastOrNull();
    }

    public static Card? GetHighestTrumpCard(List<Card> cards, Suit trump)
    {
        return cards.OrderByDescending(c => c.GetTrickValue(trump, trump)).FirstOrNull();
    }

    public static Card? GetLowestTrumpCard(List<Card> cards, Suit trump)
    {
        return cards.OrderByDescending(c => c.GetTrickValue(trump, trump)).LastOrNull();
    }

    public static Card? GetHighestCardOfSuit(List<Card> cards, Suit? suit)
    {
        return cards
                .Where(c => c.Suit == suit)
                .OrderByDescending(c => (int)c.Rank)
                .FirstOrNull();
    }

    public static Card? GetHighestBowerCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.IsBower(trump))
                .OrderByDescending(c => c.GetTrickValue(trump, trump))
                .FirstOrNull();
    }
}
