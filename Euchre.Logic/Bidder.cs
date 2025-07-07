using Euchre.Logic.Interfaces;

namespace Euchre.Logic;

/// <summary>
/// Contains logic for bidding in a game of Euchre.
/// </summary>
public class Bidder : IBidder
{
    public Bidder(List<Card> playerHand)
    {
        _playerHand = playerHand ?? 
            throw new ArgumentNullException(nameof(playerHand), "Player's hand cannot be null.");
    }

    private readonly List<Card> _playerHand;

    // TODO: Add more complex logic to determine if the player should order up the kitty or not.
    //       For example, add logic to consider the strength of the kitty card.
    // TODO: Move the logic for determining if the player should order up the kitty to a separate method.
    //       Set it up to determine if any suit can be called as trump based on the player's hand.
    public bool DetermineWhetherToOrderUp(Card kitty, bool isDealer)
    {
        var trumpCards = _playerHand.Count(c => c.EffectiveSuit(kitty.Suit) == kitty.Suit);
        var bowers = _playerHand.Count(c => c.IsBower(kitty.Suit));
        var aces = _playerHand.Count(c => c.Rank == Rank.Ace);

        return trumpCards >= 3 || (trumpCards >= 2 && bowers >= 1) || (isDealer && trumpCards >= 2);
    }

    public Suit? DetermineTrump(Card kitty)
    {
        var suitCounts = new Dictionary<Suit, int>();

        // Can't call kitty suit in round 2.

        foreach (Suit suit in Enum.GetValues<Suit>().Where(s => s != kitty.Suit))
        {
            suitCounts[suit] = _playerHand.Count(c => c.EffectiveSuit(suit) == suit);
        }

        var bestSuit = suitCounts.OrderByDescending(kvp => kvp.Value).First();
        return bestSuit.Value >= 3 ? bestSuit.Key : null;
    }

    public void DiscardForKitty(Card kitty, Suit trump)
    {
        // Discard lowest non-trump card.

        var nonTrumps = _playerHand.Where(c => c.EffectiveSuit(trump) != trump && !c.IsBower(trump)).ToList();
        if (nonTrumps.Count > 0)
        {
            var discard = nonTrumps.OrderBy(c => (int)c.Rank).First();
            _playerHand.Remove(discard);
        }
        else
        {
            // If all trumps, discard lowest.

            var discard = _playerHand.OrderBy(c => c.GetTrickValue(trump, trump)).First();
            _playerHand.Remove(discard);
        }

        _playerHand.Add(kitty);
    }
}
