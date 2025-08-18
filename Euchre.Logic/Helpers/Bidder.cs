using Euchre.Logic.Components;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Helpers;

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

    public bool DetermineWhetherToOrderUp(Card kitty, bool isDealer, out bool goAlone)
    {
        var copyOfHand = new List<Card>(_playerHand);

        // If the player is the dealer, they can consider the kitty card as part of their hand.

        if (isDealer)
        {
            copyOfHand.Add(kitty);
        }

        return ShouldCallSuit(copyOfHand, kitty.Suit, out goAlone);
    }

    public Suit? DetermineTrump(Card kitty, out bool goAlone)
    {
        var suitCounts = new Dictionary<Suit, int>();

        // Determine how many cards for each suit. Can't call kitty suit in round 2.

        foreach (Suit suit in Enum.GetValues<Suit>().Where(s => s != kitty.Suit))
        {
            suitCounts[suit] = CardFinder.CountCardsOfSuit(_playerHand, suit);
        }

        // Determine the order of suits based on the number of cards.

        var suitsByNumber = suitCounts.OrderByDescending(kvp => kvp.Value);

        // Determine if we should call any suit based on the player's hand.

        foreach (var suit in suitsByNumber)
        {
            if (ShouldCallSuit(_playerHand, suit.Key, out goAlone))
            {
                return suit.Key;
            }
        }

        goAlone = false;

        return null;
    }

    public void DiscardForKitty(Card kitty, Suit trump)
    {
        // Discard lowest non-trump card. If all trumps, discard lowest.

        var discard = (CardFinder.HasOffSuit(_playerHand, trump)
            ? CardFinder.GetLowestOffSuitCard(_playerHand, trump)
            : CardFinder.GetLowestTrumpCard(_playerHand, trump)) 
              ?? throw new InvalidGameConditionException("No valid card to discard for the kitty.");

        _playerHand.Remove(discard);
        _playerHand.Add(kitty);
    }

    private bool ShouldCallSuit(List<Card> cards, Suit suit, out bool goAlone)
    {
        bool callSuit = false;

        var numTrumpCards = CardFinder.CountCardsOfSuit(cards, suit);
        var numBowers = CardFinder.CountBowers(cards, suit);
        var aces = CardFinder.CountCardsOfRank(cards, Rank.Ace);
        var otherTrump = CardFinder.GetNonBowers(cards, suit);

        // If we have two bowers and another trump, we can order up.

        if (numBowers == 2 && otherTrump.Count >= 1)
        {
            callSuit = true;
        }
        else if (numBowers == 1 && otherTrump.Count >= 2)
        {
            // If we have one bower and at least two trump cards, we can order up.

            callSuit = true;
        }
        else if (aces >= 2 && numTrumpCards >= 2)
        {
            // If we have two aces and at least 2 trump cards, we can order up.

            callSuit = true;
        }
        else if (aces >= 3 && numTrumpCards >= 1)
        {
            // If we have three aces and at least trump card, we can order up.

            callSuit = true;
        }

        goAlone = callSuit && CanGoAlone(suit);

        return callSuit;
    }

    private bool CanGoAlone(Suit trump)
    {
        // Check if the player has a strong hand to go alone.

        var numBowers = CardFinder.CountBowers(_playerHand, trump);
        var numTrumpCards = CardFinder.CountCardsOfSuit(_playerHand, trump);
        var numAces = CardFinder.CountCardsOfRank(_playerHand, Rank.Ace);

        return (numBowers >= 1 && numTrumpCards >= 4) 
            || (numBowers == 2 && numTrumpCards > 2 && numAces >= 1);
    }
}
