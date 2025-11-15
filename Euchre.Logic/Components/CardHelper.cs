

namespace Euchre.Logic.Components;

/// <summary>
/// Provides helper methods for cards and their suits.
/// </summary>
internal class CardHelper
{
    /// <summary>
    /// Returns the suit of the left bower given the trump suit.
    /// </summary>
    /// <param name="trump">The trump suit.</param>
    /// <returns>The suit for the specified trump's left bower.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public static Suit GetLeftBowerSuit(Suit trump)
    {
        return trump switch
        {
            Suit.Hearts => Suit.Diamonds,
            Suit.Diamonds => Suit.Hearts,
            Suit.Clubs => Suit.Spades,
            Suit.Spades => Suit.Clubs,
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Returns a hand containing the highest possible cards for the specified trump suit.
    /// </summary>
    /// <param name="trump">The trump suit.</param>
    /// <returns>A List of Card objects containing the 5 highest trump cards.</returns>
    public static List<Card> CreateHighestTrumpHand(Suit trump)
    {
        return
        [
            new(trump, Rank.Jack),
            new(GetLeftBowerSuit(trump), Rank.Jack),
            new(trump, Rank.Ace),
            new(trump, Rank.King),
            new(trump, Rank.Queen),
        ];
    }

    public static bool TrumpCardsAreSequential(List<Card> cards, Suit trump)
    {
        bool areSequential = false;

        switch (cards.First())
        {
            case Card c when c.IsRightBower(trump):
                areSequential = cards[1].IsLeftBower(trump);
                if (cards.Count > 2)
                {
                    if (cards[2].Rank == Rank.Ace)
                    {
                        if (cards.Count > 3)
                        { 
                            areSequential = CheckTrumpFromAce(cards[2..]);
                        }
                    }
                    else
                    {
                        areSequential = false;
                    }
                }
                break;

            case Card c when c.IsLeftBower(trump):
                areSequential = cards[1].Rank == Rank.Ace;
                if (areSequential)
                {
                    areSequential = CheckTrumpFromAce(cards[1..]);
                }
                break;

            case Card c when c.Rank == Rank.Ace:
                areSequential = CheckTrumpFromAce(cards);
                break;

            default:
                areSequential = false;
                break;
        }

        return areSequential;
    }

    private static bool CheckTrumpFromAce(List<Card> cards)
    {
        int previousRankValue = (int)Rank.Ace;

        for (int idx = 1; idx < cards.Count; idx++)
        {
            int currentRankValue = (int)cards[idx].Rank;
            if (currentRankValue != previousRankValue - 1)
            {
                return false;
            }
            previousRankValue = currentRankValue;
        }

        return true;
    }

    public static bool AllCardsAreHigherThanTrumpCard(IEnumerable<Card> playedTrumpCards, Card highestTrumpInHand)
    {
        bool anyLowerTrumpFound = false;

        if (!highestTrumpInHand.IsRightBower(highestTrumpInHand.Suit))
        {
            var highTrumpCards = CreateHighestTrumpHand(highestTrumpInHand.Suit);
            if (highTrumpCards.Contains(highestTrumpInHand))
            {
                int indexOfHighestInHand = highTrumpCards.IndexOf(highestTrumpInHand);
                var remainingHighTrumpCards = highTrumpCards[..(indexOfHighestInHand + 1)];
                foreach (var playedCard in playedTrumpCards)
                {
                    if (!remainingHighTrumpCards.Contains(playedCard))
                    {
                        anyLowerTrumpFound = true;
                        break;
                    }
                }
            }
        }

        return anyLowerTrumpFound;
    }
}