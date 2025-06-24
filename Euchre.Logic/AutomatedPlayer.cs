namespace Euchre.Logic;

public class AutomatedPlayer : Player
{
    public AutomatedPlayer(string name) : base(name, false) 
    { 
    }

    public override List<Card> GetValidCards(Suit leadSuit, Suit trump)
    {
        if (Hand.Count == 0) return [];

        var leadCards = Hand.Where(c => c.EffectiveSuit(trump) == leadSuit).ToList();
        return leadCards.Count > 0 ? leadCards : [.. Hand];
    }

    // TODO: Add more complex logic to determine if the player should order up the kitty or not.
    //       For example, add logic to consider the strength of the kitty card.
    // TODO: Move the logic for determining if the player should order up the kitty to a separate method.
    //       Set it up to determine if any suit can be called as trump based on the player's hand.
    public override bool OrderUp(Card kitty, bool isDealer)
    {
        var trumpCards = Hand.Count(c => c.EffectiveSuit(kitty.Suit) == kitty.Suit);
        var bowers = Hand.Count(c => c.IsBower(kitty.Suit));
        var aces = Hand.Count(c => c.Rank == Rank.Ace);

        return trumpCards >= 3 || (trumpCards >= 2 && bowers >= 1) || (isDealer && trumpCards >= 2);
    }

    // TODO: Implement a stronger strategy for AI to call trump based on strength of each suit.
    public override Suit? CallTrump(Card kitty)
    {
        var suitCounts = new Dictionary<Suit, int>();

        // Can't call kitty suit in round 2.

        foreach (Suit suit in Enum.GetValues<Suit>().Where(s => s != kitty.Suit))
        {
            suitCounts[suit] = Hand.Count(c => c.EffectiveSuit(suit) == suit);
        }

        var bestSuit = suitCounts.OrderByDescending(kvp => kvp.Value).First();
        return bestSuit.Value >= 3 ? bestSuit.Key : null;
    }

    public override Card SelectCardToPlay(List<Card> trick, Suit trump, Suit? leadSuit)
    {
        var validCards = GetValidCards(leadSuit ?? trump, trump);
        if (validCards.Count == 0) return Hand[0];

        if (trick.Count == 0) // Leading
        {
            // Lead with highest trump if available, otherwise highest card
            var trumps = validCards.Where(c => c.EffectiveSuit(trump) == trump).ToList();
            if (trumps.Count > 0)
                return trumps.OrderByDescending(c => c.GetTrickValue(trump, trump)).First();

            return validCards.OrderByDescending(c => (int)c.Rank).First();
        }
        else // Following
        {
            var currentWinner = GetTrickWinner(trick, trump, leadSuit.Value);
            var winningValue = currentWinner.GetTrickValue(trump, leadSuit.Value);

            // Try to win the trick
            var canWin = validCards.Where(c => c.GetTrickValue(trump, leadSuit.Value) > winningValue).ToList();
            if (canWin.Count > 0)
                return canWin.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();

            // Can't win, play lowest card
            return validCards.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();
        }
    }

    public override void DiscardForKitty(Card kitty, Suit trump)
    {
        // Discard lowest non-trump card
        var nonTrumps = Hand.Where(c => c.EffectiveSuit(trump) != trump && !c.IsBower(trump)).ToList();
        if (nonTrumps.Count > 0)
        {
            var discard = nonTrumps.OrderBy(c => (int)c.Rank).First();
            Hand.Remove(discard);
        }
        else
        {
            // If all trumps, discard lowest
            var discard = Hand.OrderBy(c => c.GetTrickValue(trump, trump)).First();
            Hand.Remove(discard);
        }

        Hand.Add(kitty);
    }

    private Card GetTrickWinner(List<Card> trick, Suit trump, Suit leadSuit)
    {
        return trick.OrderByDescending(c => c.GetTrickValue(trump, leadSuit)).First();
    }
}
