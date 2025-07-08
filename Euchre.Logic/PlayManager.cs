using Euchre.Logic.Interfaces;

namespace Euchre.Logic;

public class PlayManager : IPlayManager
{
    public PlayManager(List<Card> playerHand)
    {
        _playerHand = playerHand ?? throw new ArgumentNullException(nameof(playerHand));
    }

    private readonly List<Card> _playerHand;


    /// <summary>
    /// Determines the valid cards that can be played based on the lead suit and trump suit.
    /// </summary>
    /// <remarks>This method enforces the rule that players must follow the lead suit if they have cards of
    /// that suit. If no cards match the lead suit, the player may play any card from their hand.</remarks>
    /// <param name="leadSuit">The suit of the card that was led in the current trick.</param>
    /// <param name="trump">The trump suit for the game, which may affect card behaviour.</param>
    /// <returns>A list of valid cards from the player's hand. If the hand contains cards matching the lead 
    /// suit, those cards are returned. Otherwise, all cards in the hand are considered valid.</returns>
    public List<Card> GetValidCards(Suit leadSuit, Suit trump)
    {
        if (_playerHand.Count == 0) return [];

        var leadCards = _playerHand.Where(c => c.EffectiveSuit(trump) == leadSuit).ToList();
        return leadCards.Count > 0 ? leadCards : [.. _playerHand];
    }

    public Card DetermineCardToPlay(Trick trick, Suit trump, Suit? leadSuit)
    {
        var validCards = GetValidCards(leadSuit ?? trump, trump);
        if (validCards.Count == 0) return _playerHand[0];

        if (trick.Cards.Count == 0) // Leading
        {
            // Lead with highest trump if available, otherwise highest card
            var trumps = validCards.Where(c => c.EffectiveSuit(trump) == trump).ToList();
            if (trumps.Count > 0)
                return trumps.OrderByDescending(c => c.GetTrickValue(trump, trump)).First();

            return validCards.OrderByDescending(c => (int)c.Rank).First();
        }
        else // Following
        {
            var currentWinner = trick.GetCurrentLeadingCard();
            var winningValue = currentWinner.GetTrickValue(trump, leadSuit.Value);

            // Try to win the trick
            var canWin = validCards.Where(c => c.GetTrickValue(trump, leadSuit.Value) > winningValue).ToList();
            if (canWin.Count > 0)
                return canWin.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();

            // Can't win, play lowest card
            return validCards.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();
        }
    }
}
