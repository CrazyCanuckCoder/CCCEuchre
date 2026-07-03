using Euchre.Logic.Components;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Helpers;

/// <summary>
/// Contains logic for bidding in a game of Euchre.
/// </summary>
public class Bidder : IBidder
{
    public Bidder(IPlayer player)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player), "Player cannot be null.");
    }

    /// <summary>
    /// A reference to the player in order to access the cards in their hand.
    /// </summary>
    private readonly IPlayer _player;

    /// <summary>
    /// Determines whether the player should order up the given card as the trump suit.
    /// </summary>
    /// <param name="kitty">The card in the kitty being considered for trump.</param>
    /// <param name="isDealer">A value indicating whether the player is the dealer.</param>
    /// <param name="goAlone">Indicates whether the player should go alone when ordering up the card.</param>
    /// <returns><see langword="true"/> if the player decides to order up the card as the trump suit.</returns>
    public bool DetermineWhetherToOrderUp(Card kitty, bool isDealer, out bool goAlone)
    {
        var copyOfHand = new List<Card>(_player.Hand);

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
    public Suit? DetermineTrump(Card kitty, bool isDealer, out bool goAlone)
    {
        var suitCounts = new Dictionary<Suit, int>();

        // Determine how many cards for each suit. Can't call kitty suit in round 2.

        foreach (Suit suit in Enum.GetValues<Suit>().Where(s => s != kitty.Suit))
        {
            suitCounts[suit] = CardFinder.CountCardsOfSuitExceptForTheJack(_player.Hand, suit) +
                CardFinder.CountBowers(_player.Hand, suit);
        }

        // Determine the order of suits based on the number of cards.

        var suitsByNumber = suitCounts.OrderByDescending(kvp => kvp.Value);

        // Determine if we should call any suit based on the player's hand.

        foreach (var suit in suitsByNumber)
        {
            if (ShouldCallSuit(_player.Hand, suit.Key, out goAlone))
            {
                return suit.Key;
            }
        }

        goAlone = false;

        // Check for the stick the dealer condition and return the strongest suit as trump.
        //  If not, return null to indicate the user will pass.

        return GameSettingsManager.Instance.StickTheDealer && isDealer ? suitsByNumber.First().Key : null;
    }

    /// <summary>
    /// Determines which card in the player's hand will be replaced by the kitty's upturned card.
    /// </summary>
    /// <param name="kitty">The card from the kitty to add to the user's hand.</param>
    /// <exception cref="InvalidGameConditionException"></exception>
    public void DiscardForKitty(Card kitty)
    {
        // Discard lowest non-trump card. If all trumps, discard lowest.

        var discard = (CardFinder.HasOffSuit(_player.Hand, kitty.Suit)
            ? CardFinder.GetLowestOffSuitCard(_player.Hand, kitty.Suit)
            : CardFinder.GetLowestTrumpCard(_player.Hand, kitty.Suit)) 
              ?? throw new InvalidGameConditionException("No valid card to discard for the kitty.");

        _player.Hand.Remove(discard);
        _player.Hand.Add(kitty);
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
        var callSuit = false;

        var numBowers = CardFinder.CountBowers(cards, suit);
        var numAces = CardFinder.CountNonTrumpCardsOfRank(cards, Rank.Ace, suit);
        var otherTrump = CardFinder.GetNonBowerTrump(cards, suit);

        switch (numBowers)
        {
            // If the player has two bowers and another trump, can call the suit.

            case 2 when otherTrump.Count >= 1:
                callSuit = true;
                break;

            // If the player has one bower and at least two trump cards, can call the suit.

            case 1 when otherTrump.Count >= 2:
                callSuit = true;
                break;

            default:
                if (numAces >= 2 && otherTrump.Count >= 2)
                {
                    // If the player has two aces and at least 2 trump cards, can call the suit.

                    callSuit = true;
                }
                else if (numAces >= 3 && otherTrump.Count >= 1)
                {
                    // If the player has three aces and at least trump card, can call the suit.

                    callSuit = true;
                }
                break;
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

        var numBowers = CardFinder.CountBowers(_player.Hand, trump);
        var numTrumpCards = CardFinder.GetNonBowerTrump(_player.Hand, trump).Count;
        var numAces = CardFinder.CountCardsOfRank(_player.Hand, Rank.Ace);

        return (numBowers + numTrumpCards >= 4) 
            || (numBowers >= 1 && numTrumpCards >= 3) 
            || (numBowers == 2 && numTrumpCards >= 1 && numAces >= 1);
    }
}
