using Euchre.Logic.Components;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Helpers;

/// <summary>
/// Manages the logic for playing cards by an automated player.
/// </summary>
public class PlayManager : IPlayManager
{
    public PlayManager(GameStateManager gameStateManager, IPlayer player)
    {
        _gameStateManager = gameStateManager ?? throw new ArgumentNullException(nameof(gameStateManager));
        _player = player;
    }

    /// <summary>
    /// The game data manager that provides access to the game's state.
    /// </summary>
    private readonly GameStateManager _gameStateManager;

    /// <summary>
    /// A reference to the player for this play manager.
    /// </summary>
    private readonly IPlayer _player;

    /// <summary>
    /// A reference to the trick being played.
    /// </summary>
    private Trick _currentTrick;

    /// <summary>
    /// Determines the card to play for the current trick based on the lead suit, trump suit, and the cards
    /// in the player's hand.
    /// </summary>
    /// <param name="trick">The current trick, containing the cards already played in this round.</param>
    /// <param name="trump">The trump suit for the game, which may influence card selection.</param>
    /// <param name="leadSuit">The suit that was led in the current trick, or <see langword="null"/> if no
    /// suit has been led yet.</param>
    /// <returns>The card to play for the current trick. If no valid cards can be played, the first card in
    /// the player's hand is returned.</returns>
    public Card DetermineCardToPlay(Trick trick, Suit trump, Suit? leadSuit)
    {
        if (_player.Hand.Count == 0)
        {
            throw new InvalidGameConditionException("Cannot play a card when the player's hand is empty.");
        }

        // If there is only one card left in the player's hand, choose it.

        if (_player.Hand.Count == 1) 
        {
            return _player.Hand[0];
        }
        else
        {
            _currentTrick = trick;

            // Which card to play depends on whether the player is leading the trick or following suit.

            return trick.Cards.Count == 0
                ? DetermineWhichCardWhenLeading(trump)
                : DetermineWhichCardWhenFollowingSuit(trick, trump, leadSuit);
        }
    }

    /// <summary>
    /// Determines the card to lead at the start of a trick based on the player's role, the trump suit, and 
    /// the valid cards available to play.
    /// </summary>
    /// <param name="trump">The trump suit for the current round.</param>
    /// <returns>The card that the player should lead with.</returns>
    /// <exception cref="InvalidGameConditionException"></exception>
    private Card DetermineWhichCardWhenLeading(Suit trump)
    {
        Card? cardToLead;

        if (_player == _gameStateManager.TrumpCaller)
        {
            cardToLead = DetermineCardToLeadWhenTrumpCaller(trump);
        }
        else
        {
            if (_player.TeamIndex == _gameStateManager.TrumpCaller?.TeamIndex)
            {
                cardToLead = DetermineCardToLeadWhenPartnerOfTrumpCaller(trump);
            }
            else
            {
                // If the player is not a member of the calling team, lead the highest card of any suit that
                //  is not trump.

                cardToLead = CardFinder.GetHighestOffSuitCard(_player.Hand, trump)
                          ?? CardFinder.GetLowestTrumpCard(_player.Hand, trump);
            }
        }

        return cardToLead ?? 
            throw new InvalidGameConditionException("No card was found for the player to lead!");
    }

    /// <summary>
    /// Chooses the card to lead when the player is the trump caller.
    /// </summary>
    /// <param name="trump">The trump suit for the current trick.</param>
    /// <returns>A card to lead or null to indicate one could not be chosen.</returns>
    private Card? DetermineCardToLeadWhenTrumpCaller(Suit trump)
    {
        Card? cardToLead;

        if (_gameStateManager.GoingAlone)
        {
            // If the player is going alone, lead the highest trump card until trump is exhausted.
            //   When no trumps are available, lead with the highest card of any suit.

            cardToLead = CardFinder.HasTrump(_player.Hand, trump)
                ? CardFinder.GetHighestTrumpCard(_player.Hand, trump)
                : CardFinder.GetHighestOffSuitCard(_player.Hand, trump);
        }
        else
        {
            // See if trump has been led in the current round.

            bool hasTrumpBeenLed = _gameStateManager.CurrentRoundTricks
                                          .Any(t => t.Cards.First().Value.EffectiveSuit(trump) == trump);
            if (!hasTrumpBeenLed)
            {
                // Lead the highest trump. If no trumps are available, lead with the highest card of any suit.

                cardToLead = CardFinder.HasTrump(_player.Hand, trump)
                    ? CardFinder.GetHighestTrumpCard(_player.Hand, trump)
                    : CardFinder.GetHighestOffSuitCard(_player.Hand, trump);
            }
            else
            {
                // Lead the highest card of any suit that is not trump.

                cardToLead = CardFinder.HasOffSuit(_player.Hand, trump)
                    ? CardFinder.GetHighestOffSuitCard(_player.Hand, trump)
                    : CardFinder.GetHighestTrumpCard(_player.Hand, trump);
            }
        }

        return cardToLead;
    }

    /// <summary>
    /// Chooses the card to lead when the player is the partner of the trump caller.
    /// </summary>
    /// <param name="trump">The trump suit for the current trick.</param>
    /// <returns>A card to lead or null to indicate one could not be chosen.</returns>
    private Card? DetermineCardToLeadWhenPartnerOfTrumpCaller(Suit trump)
    {
        Card? cardToLead;

        // If the player is the partner of the caller, if the caller has not led with a trump card,
        //  and the player has the left or right bower, lead it.

        if (!HasPlayerLedTrump(_gameStateManager.CurrentRoundTricks, _gameStateManager.TrumpCaller!))
        {
            cardToLead = CardFinder.HasBower(_player.Hand, trump)
                ? CardFinder.GetHighestBowerCard(_player.Hand, trump)
                : CardFinder.GetHighestOffSuitCard(_player.Hand, trump)
                  ?? CardFinder.GetLowestTrumpCard(_player.Hand, trump);
        }
        else
        {
            // If the player is the partner of the caller, if the caller has led with a trump card,
            //  lead the highest card of any suit that is not trump.

            cardToLead = CardFinder.GetHighestOffSuitCard(_player.Hand, trump)
                      ?? CardFinder.GetLowestTrumpCard(_player.Hand, trump);
        }

        return cardToLead;
    }

    /// <summary>
    /// Chooses the card to play when following suit in the current trick.
    /// </summary>
    /// <param name="trick">The details about the current trick.</param>
    /// <param name="trump">The trump suit for the current trick.</param>
    /// <param name="leadSuit">The suit lead for the current trick.</param>
    /// <returns>A card to play or null to indicate one could not be chosen.</returns>
    /// <exception cref="InvalidGameConditionException"></exception>
    private Card DetermineWhichCardWhenFollowingSuit(Trick trick, Suit trump, Suit? leadSuit)
    {
        Card? cardToPlay;

        // If the lead suit is null, it means the player is following a trick that has not been led yet.
        //   Throw an exception if this is the case.

        if (leadSuit == null)
        {
            throw new InvalidGameConditionException("Lead suit cannot be null when following a trick.");
        }

        // Determine who is winning the current trick, whether the player has trump, and whether the player
        //   has off suit.

        (IPlayer winningPlayer, ICard highestTrickCard) = trick.GetHighestCardInTrick();
        bool hasTrump = CardFinder.HasTrump(_player.Hand, trump);
        bool hasOffSuit = CardFinder.HasOffSuit(_player.Hand, trump);

        // If the lead suit is trump, determine which trump card to play.

        cardToPlay = leadSuit == trump
            ? DetermineCardToPlayWhenTrumpLead(trump, winningPlayer, hasTrump, highestTrickCard)
            : DetermineCardToPlayWhenOffSuitLead(trump, leadSuit.Value, winningPlayer, hasTrump, hasOffSuit,
                highestTrickCard);

        return cardToPlay ??
            throw new InvalidGameConditionException("No card was found for the player to play!");
    }

    /// <summary>
    /// Chooses the card to play when the lead suit is trump.
    /// </summary>
    /// <param name="trump">The suit that was declared trump for the current trick.</param>
    /// <param name="winningPlayer">Which player is currently winning the trick.</param>
    /// <param name="hasTrump">True to indicate the player has trump in their hand.</param>
    /// <param name="highestTrickCard">The highest card played in the current trick.</param>
    /// <returns>A card to play or null to indicate one could not be chosen.</returns>
    private Card? DetermineCardToPlayWhenTrumpLead(Suit trump, IPlayer winningPlayer, bool hasTrump, 
        ICard highestTrickCard)
    {
        Card? cardToPlay;

        if (hasTrump)
        {
            // If the player's partner is winning the current trick, play the lowest trump card.
            //   Else, if the player has a trump card, play the highest trump card in their hand.

            cardToPlay = winningPlayer.TeamIndex == _player.TeamIndex
                ? CardFinder.GetLowestTrumpCard(_player.Hand, trump)
                : GetTrumpCardToBeatOpponent(trump, highestTrickCard);
        }
        else
        {
            // If the player does not have a trump card, play the lowest off suit card.

            cardToPlay = CardFinder.GetLowestOffSuitCard(_player.Hand, trump);
        }

        return cardToPlay;
    }

    /// <summary>
    /// Chooses the card to play when the lead suit is not trump.
    /// </summary>
    /// <param name="trump">The suit that was declared trump for the current trick.</param>
    /// <param name="leadSuit">The suit that was lead for the current trick.</param>
    /// <param name="winningPlayer">Which player is currently winning the trick.</param>
    /// <param name="hasTrump">True to indicate the player has trump in their hand.</param>
    /// <param name="hasOffSuit">True to indicate the player has non trump cards in their hand.</param>
    /// <returns>A card to play or null to indicate one could not be chosen.</returns>
    private Card? DetermineCardToPlayWhenOffSuitLead(Suit trump, Suit leadSuit, IPlayer winningPlayer, 
        bool hasTrump, bool hasOffSuit, ICard highestTrickCard)
    {
        Card? cardToPlay;

        if (CardFinder.HasACardOfSuit(_player.Hand, leadSuit, trump))
        {
            // If the player has a card of the lead suit, check if they have a card that can win the trick.
            //   If so, play the highest card of the lead suit. If not, play the lowest card of the lead suit.

            Card playersHighestCard = CardFinder.GetHighestCardOfSuit(_player.Hand, leadSuit, trump)!;
            bool hasHigherLeadSuitCard = playersHighestCard.GetTrickValue(trump, leadSuit) > 
                highestTrickCard.GetTrickValue(trump, leadSuit);
            cardToPlay = hasHigherLeadSuitCard
                ? playersHighestCard
                : CardFinder.GetLowestCardOfSuit(_player.Hand, leadSuit, trump);
        }
        else
        {
            // If the player does not have a card of the lead suit, determine if the player's partner has the
            //   lead of the current trick. If they do, play the lowest off-suit card.  If not, play the
            //   lowest trump card in the player's hand.

            if (winningPlayer.TeamIndex == _player.TeamIndex)
            {
                cardToPlay = hasOffSuit
                    ? CardFinder.GetLowestOffSuitCard(_player.Hand, trump)
                    : CardFinder.GetLowestTrumpCard(_player.Hand, trump);
            }
            else
            {
                // If the player's partner is not winning the current trick, see if the trick can be trumped.
                //   If not, play the lowest off suit card.

                cardToPlay = hasTrump
                    ? DetermineCardToTrumpOpponentsTrick(trump)
                    : CardFinder.GetLowestOffSuitCard(_player.Hand, trump);
            }
        }

        return cardToPlay;
    }

    /// <summary>
    /// Determines a card to play when the player's opponents are winning the trick.
    /// </summary>
    /// <remarks>This method assumes it is being called when an opponent is winning the trick.</remarks>
    /// <param name="trump">The suit that was declared trump for the current trick.</param>
    /// <returns>A Card that was determined to play for the current trick.</returns>
    private Card? DetermineCardToTrumpOpponentsTrick(Suit trump)
    {
        Card? cardToPlay = null;

        // Has the trick been trumped already?

        var trickCards = (  from card in _currentTrick.Cards.Values
                          select card)
                         .ToList();
        if (CardFinder.HasTrump(trickCards, trump))
        {
            // Since it is the opponent's card, does the player have a higher trump?

            var winningCard = _currentTrick.GetCurrentLeadingCard();
            var playersHighestTrump = CardFinder.GetHighestTrumpCard(_player.Hand, trump);

            if (playersHighestTrump!.GetTrickValue(trump, _currentTrick.LeadSuit) >
                winningCard.GetTrickValue(trump, _currentTrick.LeadSuit))
            {
                // If yes, play the next highest trump.

                cardToPlay = CardFinder.GetNextHighestTrump(_player.Hand, trump, winningCard);
            }
            else
            {
                // If no, play off suit.

                cardToPlay = CardFinder.GetLowestOffSuitCard(_player.Hand, trump)
                    ?? CardFinder.GetLowestTrumpCard(_player.Hand, trump);
            }
        }
        else
        {
            // If not, return the lowest trump card.

            cardToPlay = CardFinder.GetLowestTrumpCard(_player.Hand, trump);
        }

        return cardToPlay;
    }

    /// <summary>
    /// Determines if the specified player has led the trump suit in the list of tricks.
    /// </summary>
    /// <param name="trick">The list of tricks to check for a trump lead.</param>
    /// <param name="player">The player that has lead the trump suit.</param>
    /// <returns>True if the player led the trump suit; otherwise, false.</returns>
    private static bool HasPlayerLedTrump(List<Trick> tricks, IPlayer player)
    {
        foreach (var trick in tricks)
        {
            // The first card in the trick is the lead card, and its player is the leader.

            KeyValuePair<IPlayer, Card> leadCard = trick.Cards.First();

            // Check if the specified player is the leader and if the lead card's effective suit is trump.

            if (leadCard.Key == player && leadCard.Value.EffectiveSuit(trick.Trump) == trick.Trump)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds a trump card in the player hand higher than the specified opponent's card.  If there is no 
    /// higher trump card in the player's hand, the lowest trump card is returned.
    /// </summary>
    /// <param name="trump">The suit that was declared trump for the current trick.</param>
    /// <param name="opponentsCard">The card the player is trying to beat.</param>
    /// <returns>A card greater than the opponent card, if possible, or the lowest trump card when not 
    /// possible.</returns>
    /// <exception cref="InvalidGameConditionException"></exception>
    private Card GetTrumpCardToBeatOpponent(Suit trump, ICard opponentsCard)
    {
        Card? cardToPlay;

        bool canBeatOpponentCard =
            CardFinder.GetHighestTrumpCard(_player.Hand, trump)!.GetTrickValue(trump, trump) >
            opponentsCard.GetTrickValue(trump, trump);

        cardToPlay = canBeatOpponentCard
            ? CardFinder.GetHighestTrumpCard(_player.Hand, trump)
            : CardFinder.GetLowestTrumpCard(_player.Hand, trump);

        return cardToPlay ??
            throw new InvalidGameConditionException("No card was found for the player to play!");
    }
}
