using Euchre.Logic.Components;

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

    public static bool HasOffSuit(List<Card> cards, Suit trump)
    {
        return cards.Any(c => c.EffectiveSuit(trump) != trump);
    }

    public static Card GetHighestOffSuitCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) != trump)
                .OrderByDescending(c => (int)c.Rank)
                .FirstOrDefault() ?? cards[0];
    }

    public static Card GetHighestTrumpCard(List<Card> cards, Suit trump)
    {
        return cards.OrderByDescending(c => c.GetTrickValue(trump, trump)).First();
    }

    public static Card GetLowestTrumpCard(List<Card> cards, Suit trump)
    {
        return cards.OrderByDescending(c => c.GetTrickValue(trump, trump)).Last();
    }

    public static Card GetHighestCardOfSuit(List<Card> cards, Suit? suit)
    {
        return cards
                .Where(c => c.Suit == suit)
                .OrderByDescending(c => (int)c.Rank)
                .FirstOrDefault() ?? cards[0];
    }

    public static Card GetLowestOffSuitCard(List<Card> cards, Suit trump)
    {
        return cards
                .Where(c => c.EffectiveSuit(trump) != trump)
                .OrderByDescending(c => (int)c.Rank)
                .LastOrDefault() ?? cards[0];
    }
}
