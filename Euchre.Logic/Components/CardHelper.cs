using Euchre.Logic.Helpers;

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
            Suit.Hearts   => Suit.Diamonds,
            Suit.Diamonds => Suit.Hearts,
            Suit.Clubs    => Suit.Spades,
            Suit.Spades   => Suit.Clubs,
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

    /// <summary>
    /// Determines if the provided list of trump cards are sequential in rank.
    /// </summary>
    /// <param name="cards">A list of trump cards.</param>
    /// <param name="trump">The trump suit.</param>
    /// <returns>True when the cards are in sequential order.</returns>
    public static bool TrumpCardsAreSequential(List<Card> cards, Suit trump)
    {
        bool areSequential = false;

        switch (cards.First())
        {
            case Card c when c.IsRightBower(trump):
                areSequential = cards[1].IsLeftBower(trump);
                if (areSequential && cards.Count > 2)
                {
                    if (cards[2].Rank == Rank.Ace)
                    {
                        if (cards.Count > 3)
                        { 
                            areSequential = CheckTrumpSequenceFromAce(cards[2..]);
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
                    areSequential = CheckTrumpSequenceFromAce(cards[1..]);
                }
                break;

            case Card c when c.Rank == Rank.Ace:
                areSequential = CheckTrumpSequenceFromAce(cards);
                break;

            default:
                areSequential = false;
                break;
        }

        return areSequential;
    }

    /// <summary>
    /// Determines if the cards played are all higher than the highest trump card in hand.
    /// </summary>
    /// <param name="playedTrumpCards">A list of played cards.</param>
    /// <param name="highestTrumpInHand">The highest trump card in the player's hand.</param>
    /// <returns>True to indicate the cards are higher than the specified player's card.</returns>
    public static bool AllCardsAreHigherThanTrumpCard(IEnumerable<Card> playedTrumpCards, 
        Card highestTrumpInHand)
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

    public static bool NoMoreTrumpRemaining(List<Trick> currentTricks, Suit trump)
    {
        var lastTrickWithTrumpLead = (  from trick in currentTricks.Reverse<Trick>()
                                       where trick.LeadSuit == trump
                                      select trick).FirstOrDefault();
        return lastTrickWithTrumpLead != null
            && CardFinder.CountTrump(lastTrickWithTrumpLead.Cards.Values.ToList(), trump) == 1;
    }

    /// <summary>
    /// Returns true if all cards in the list are either trump cards or aces.
    /// </summary>
    /// <param name="cards">The cards in the player's hand.</param>
    /// <param name="trump">The trump suit.</param>
    /// <returns>True when the hand contains only trump and aces.</returns>
    public static bool PlayerHasTrumpAndAces(List<Card> cards, Suit trump)
    {
        int numTrump = cards.Count(c => c.EffectiveSuit(trump) == trump);
        int numAces = cards.Count(c => c.Rank == Rank.Ace);

        return (numTrump + numAces) == cards.Count;
    }

    /// <summary>
    /// Determines if the provided list of cards form a sequence starting from Ace downwards.
    /// </summary>
    /// <param name="cards">The list of cards to check.</param>
    /// <returns>True if the cards are sequential in rank.</returns>
    private static bool CheckTrumpSequenceFromAce(List<Card> cards)
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

    /// <summary>
    /// Determines whether two lists of cards contain the same elements, regardless of order.
    /// </summary>
    /// <param name="firstList">The first list of cards to compare. Cannot be null.</param>
    /// <param name="secondList">The second list of cards to compare. Cannot be null.</param>
    /// <returns>True if both lists contain the same cards and have the same number of elements.</returns>
    public static bool CardListsAreEqual(List<Card> firstList, List<Card> secondList)
    {
        bool areEqual = firstList.Count == secondList.Count;

        if (areEqual)
        {
            foreach (var card in firstList)
            {
                if (!secondList.Contains(card))
                {
                    areEqual = false;
                    break;
                }
            }
        }

        return areEqual;
    }
}