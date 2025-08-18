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

    /// <summary>
    /// The list of cards in the player's hand.
    /// </summary>
    private readonly List<Card> _playerHand;

    /// <summary>
    /// Determines whether the player should order up the given card as the trump suit.
    /// </summary>
    /// <param name="kitty">The card in the kitty being considered for trump.</param>
    /// <param name="isDealer">A value indicating whether the player is the dealer.</param>
    /// <param name="goAlone">Indicates whether the player should go alone when ordering up the card.</param>
    /// <returns><see langword="true"/> if the player decides to order up the card as the trump suit.</returns>
    public bool DetermineWhetherToOrderUp(Card kitty, bool isDealer, out bool goAlone)
    {
        var copyOfHand = new List<Card>(_playerHand);

        // If the player is the dealer, consider the kitty card as part of their hand.

        if (isDealer)
        {
            copyOfHand.Add(kitty);
        }

        return ShouldCallSuit(copyOfHand, kitty.Suit, out goAlone);
    }

    /// <summary>
    /// When no one orders up the kitty, determines if the trump suit can be called based on the player's
    /// hand.
    /// </summary>
    /// <param name="kitty">The card that was the kitty to ensure that suit cannot be called.</param>
    /// <param name="goAlone">Indicates whether the player should go alone when calling trump.</param>
    /// <returns>The suit to be used for trump, or null to indicate the player wants to pass.</returns>
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

    /// <summary>
    /// Determines which card in the player's hand will be replaced by the kitty's upturned card.
    /// </summary>
    /// <param name="kitty">The card from the kitty to add to the user's hand.</param>
    /// <exception cref="InvalidGameConditionException"></exception>
    public void DiscardForKitty(Card kitty)
    {
        // Discard lowest non-trump card. If all trumps, discard lowest.

        var discard = (CardFinder.HasOffSuit(_playerHand, kitty.Suit)
            ? CardFinder.GetLowestOffSuitCard(_playerHand, kitty.Suit)
            : CardFinder.GetLowestTrumpCard(_playerHand, kitty.Suit)) 
              ?? throw new InvalidGameConditionException("No valid card to discard for the kitty.");

        _playerHand.Remove(discard);
        _playerHand.Add(kitty);
    }

    /// <summary>
    /// Determines whether the player should call a specific suit as trump based on their hand.
    /// </summary>
    /// <param name="cards">The player's hand.</param>
    /// <param name="suit">The suit to consider for trump.</param>
    /// <param name="goAlone">Indicates whether the player should go alone when calling trump.</param>
    /// <returns>True to indicate the user should call the specified suit as trump.</returns>
    private bool ShouldCallSuit(List<Card> cards, Suit suit, out bool goAlone)
    {
        bool callSuit = false;

        var numTrumpCards = CardFinder.CountCardsOfSuit(cards, suit);
        var numBowers = CardFinder.CountBowers(cards, suit);
        var numAces = CardFinder.CountCardsOfRank(cards, Rank.Ace);
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
        else if (numAces >= 2 && numTrumpCards >= 2)
        {
            // If we have two aces and at least 2 trump cards, we can order up.

            callSuit = true;
        }
        else if (numAces >= 3 && numTrumpCards >= 1)
        {
            // If we have three aces and at least trump card, we can order up.

            callSuit = true;
        }

        goAlone = callSuit && CanGoAlone(suit);

        return callSuit;
    }

    /// <summary>
    /// Determines if the player has a strong enough hand to go alone with the given trump suit.
    /// </summary>
    /// <param name="trump">The suit considered trump.</param>
    /// <returns>True to indicate the user should go alone in the specified trump suit.</returns>
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
