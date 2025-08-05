using Euchre.Logic.Interfaces;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

/// <summary>
/// Manages the logic for playing cards by an automated player.
/// </summary>
public class PlayManager : IPlayManager
{
    public PlayManager(List<Card> playerHand, GameDataManager dataManager, IPlayer player)
    {
        _playerHand = playerHand ?? throw new ArgumentNullException(nameof(playerHand));
        _dataManager = dataManager ?? throw new ArgumentNullException(nameof(dataManager));
        _player = player;
    }

    /// <summary>
    /// The player's hand, which contains the cards they can play.
    /// </summary>
    private readonly List<Card> _playerHand;

    /// <summary>
    /// The game data manager that provides access to the game's state.
    /// </summary>
    private readonly GameDataManager _dataManager;

    /// <summary>
    /// A reference to the player for this play manager.
    /// </summary>
    private readonly IPlayer _player;

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
        // Get which cards can be played based on the lead suit and trump suit.  Return the first card if no
        //   valid cards are found.

        var validCards = GetValidCards(leadSuit ?? trump, trump);
        if (validCards.Count == 0)
        {
            return _playerHand[0];
        }
        else
        {
            return trick.Cards.Count == 0
                ? DetermineWhichCardWhenLeading(trump, validCards)
                : DetermineWhichCardWhenFollowing(trick, trump, leadSuit, validCards);
        }
    }

    // Information needed to determine which card to play when leading or following in a trick.
    // All tricks played so far.
    // Which players are partners.
    // Which team called trump.
    // Which player is the caller.
    // If the player is the caller, are they going alone?

    private Card DetermineWhichCardWhenLeading(Suit trump, List<Card> validCards)
    {
        Card cardToLead;

        bool isTrumpCaller = _player == _dataManager.TrumpCaller;

        // See if trump has been led in the current round.

        bool hasTrumpBeenLed = _dataManager.CurrentRoundTricks
                                      .Any(t => t.Cards.First().Card.EffectiveSuit(trump) == trump);

        // If the player is the caller, they can lead with the highest trump card.

        if (isTrumpCaller)
        {
            if (!hasTrumpBeenLed)
            {
                var trumps = validCards.Where(c => c.EffectiveSuit(trump) == trump).ToList();

                // If no trumps are available, lead with the highest card of any suit.

                cardToLead = trumps.Count > 0 ?
                    trumps.OrderByDescending(c => c.GetTrickValue(trump, trump)).First() :
                    validCards.OrderByDescending(c => (int)c.Rank).First();
            }
            else
            {
                // If the player is the caller, lead the highest card of any suit that is not trump.

                cardToLead = GetHighestOffsuitCard(trump, validCards);
            }
        }
        else
        {
            bool isPartnerOfCaller = GetPlayerTeam(_player) == GetPlayerTeam(_dataManager.TrumpCaller);
            if (isPartnerOfCaller)
            {
                // If the player is the partner of the caller, if the caller has not led with a trump card,
                //  and the player has the left or right bower, lead it.

                bool partnerHasLedWithTrump = HasPlayerLedTrump(_dataManager.CurrentRoundTricks, 
                                                                _dataManager.TrumpCaller);
                bool hasBower = validCards.Any(c => c.IsBower(trump));
                if (!partnerHasLedWithTrump)
                {
                    if ( hasBower)
                    {
                        cardToLead = validCards.Where(c => c.IsBower(trump)).First();
                    }
                    else
                    {
                        cardToLead = GetHighestOffsuitCard(trump, validCards);
                    }
                }
                else
                {
                    // If the player is the partner of the caller, if the caller has led with a trump card,
                    //  lead the  highest card of any suit that is not trump.

                    cardToLead = GetHighestOffsuitCard(trump, validCards);
                }
            }
            else
            {
                // If the player is not a member of the calling team, lead the highest card of any suit that
                //  is not trump.

                cardToLead = GetHighestOffsuitCard(trump, validCards);
            }
        }

        return cardToLead;
    }

    private static Card GetHighestOffsuitCard(Suit trump, List<Card> validCards)
    {
        return validCards
            .Where(c => c.EffectiveSuit(trump) != trump)
            .OrderByDescending(c => (int)c.Rank)
            .FirstOrDefault() ?? validCards[0];
    }

    /// <summary>
    /// Determines if the specified player has led the trump suit in the list of tricks.
    /// </summary>
    /// <param name="trick">The list of tricks to check for a trump lead.</param>
    /// <param name="player">The player that has lead the trump suit.</param>
    /// <returns>True if the player led the trump suit; otherwise, false.</returns>
    public static bool HasPlayerLedTrump(List<Trick> tricks, IPlayer player)
    {
        foreach (var trick in tricks)
        {
            // The first card in the trick is the lead card, and its player is the leader.
            
            var (leader, leadCard) = trick.Cards[0];

            // Check if the specified player is the leader and if the lead card's effective suit is trump.
            
            if (leader == player && leadCard.EffectiveSuit(trick.Trump) == trick.Trump)
            {
                return true;
            }
        }

        return false;
    }

    private int GetPlayerTeam(IPlayer player)
    {
        int index = Array.IndexOf(_dataManager.Players ?? [], player);
        return index % NUMBER_OF_PLAYERS; // Players 0,2 are team 0; Players 1,3 are team 1
    }


    private static Card DetermineWhichCardWhenFollowing(Trick trick, Suit trump, Suit? leadSuit, List<Card> validCards)
    {
        Card cardToPlay;

        #region Original Code
        
        var currentWinner = trick.GetCurrentLeadingCard();
        var winningValue = currentWinner.GetTrickValue(trump, leadSuit.Value);

        // Try to win the trick
        var canWin = validCards.Where(c => c.GetTrickValue(trump, leadSuit.Value) > winningValue).ToList();
        if (canWin.Count > 0)
            cardToPlay = canWin.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();

        // Can't win, play lowest card
        cardToPlay = validCards.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();

        #endregion Original Code

        return cardToPlay;
    }
}
