using Euchre.Logic.EventArgs;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Components;

/// <summary>
/// Represents a human-controlled player in the game, allowing user interaction for decision-making during 
/// game play.
/// </summary>
/// <remarks>The <see cref="HumanPlayer"/> class provides mechanisms for user interaction by raising events 
/// that prompt the user to make decisions, such as ordering up a card, calling trump, discarding a card, or 
/// selecting a card to play. These events must be handled by the consuming application to provide the 
/// necessary user interface for input.</remarks>
public class HumanPlayer : Player
{
    public HumanPlayer(string name, int teamIndex, int playerIndex, int avatarNumber) :
        base(name, teamIndex, playerIndex, true, avatarNumber)
    {
    }

    /// <summary>
    /// True to indicate the individual is going alone; otherwise, false.
    /// </summary>
    private bool _goAlone;

    /// <summary>
    /// Gets a value indicating whether the individual is going alone.
    /// </summary>
    public override bool IsGoingAlone
    {
        get => _goAlone;
        set => _goAlone = value;
    }

    /// <summary>
    /// Fired when to determine if the user wants to order up the kitty card.
    /// </summary>
    public event EventHandler<PromptToOrderUpEventArgs>? PromptToOrderUp;

    /// <summary>
    /// Fired to determine if the user wants to call trump.
    /// </summary>
    public event EventHandler<PromptForTrumpSuitEventArgs>? PromptForTrumpSuit;

    /// <summary>
    /// Fired to prompt the user to discard a card from their hand in exchange for the kitty card.
    /// </summary>
    public event EventHandler<PromptForDiscardEventArgs>? PromptForDiscard;

    /// <summary>
    /// Fired to prompt the user to select a card to play in the current trick.
    /// </summary>
    public event EventHandler<PromptForCardToPlayEventArgs>? PromptForCardToPlay;

    /// <summary>
    /// Fired when the player's hand has been updated.  Currently called after the user discards a card when
    /// adding the kitty card.
    /// </summary>
    public event EventHandler<System.EventArgs>? UserHandUpdated;

    /// <summary>
    /// Fired to prompt the user to decide whether to invoke the No Ace, No Face, No Trump rule.
    /// </summary>
    public event EventHandler<PromptForNoAceNoFaceNoTrumpRuleEventArgs>? PromptForNoAceNoFaceNoTrumpRule;

    /// <summary>
    /// Fired to prompt the user to select 3 cards from their hand to place under the kitty in exchange for 
    /// the 3 cards in the kitty when going under.
    /// </summary>
    public event EventHandler<PromptForGoUnderCardsEventArgs>? PromptForGoUnderCards;

    /// <summary>
    /// Prompts the user on whether to order up the given card during the bidding phase.
    /// </summary>
    /// <param name="kitty">The card being considered for ordering up.</param>
    /// <param name="isDealer">A boolean value indicating whether the current player is the dealer.</param>
    /// <param name="goUnder">A boolean value indicating whether the player chooses to go under.</param>
    /// <returns><see langword="true"/> if the player decides to order up the card; otherwise, 
    /// <see langword="false"/>.</returns>
    public override bool OrderUp(Card kitty, bool isDealer, out bool goUnder)
    {
        PromptToOrderUpEventArgs args = new(kitty);
        PromptToOrderUp?.Invoke(this, args);

        _goAlone = args.GoAlone;
        goUnder = args.GoUnder;

        return args.OrderedUp;
    }

    /// <summary>
    /// Prompts the user to discard a card from their hand in exchange for the kitty card.
    /// </summary>
    /// <param name="kitty">The card facing up on top of the kitty.</param>
    /// <exception cref="MissingCardException"></exception>
    /// <exception cref="CardWasNotSelectedException"></exception>
    public override void DiscardForKitty(Card kitty)
    {
        PromptForDiscardEventArgs args = new(kitty);
        PromptForDiscard?.Invoke(this, args);
        if (args.DiscardedCard != null)
        {
            if (!Hand.Contains(args.DiscardedCard))
            {
                throw new MissingCardException(args.DiscardedCard);
            }
            Hand.Remove(args.DiscardedCard);
            Hand.Add(kitty);
            SortPlayerCards(kitty.Suit);
            UserHandUpdated?.Invoke(this, new System.EventArgs());
        }
        else
        {
            throw new CardWasNotSelectedException();
        }
    }

    /// <summary>
    /// Prompts the user to call trump for the game.
    /// </summary>
    /// <param name="kitty">The card from the kitty indicating which suit cannot be called trump.</param>
    /// <returns>The trump suit selected by the user, or <see langword="null"/> if no trump suit is 
    /// selected which indicates the automated player will pass.</returns>
    public override Suit? CallTrump(Card kitty, bool isDealer)
    {
        PromptForTrumpSuitEventArgs args = new(kitty);
        PromptForTrumpSuit?.Invoke(this, args);

        _goAlone = args.GoAlone;

        return args.TrumpSuit;
    }

    /// <summary>
    /// Prompts the user to select a card to play in the current trick.
    /// </summary>
    /// <param name="trick">The current trick.</param>
    /// <param name="trump">The trump suit for the current round.</param>
    /// <param name="leadSuit">The suit lead for the trick.</param>
    /// <returns>The card selected by the user.</returns>
    /// <exception cref="CardWasNotSelectedException"></exception>
    /// <exception cref="MissingCardException"></exception>
    public override Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit)
    {
        PromptForCardToPlayEventArgs args = new(trump, leadSuit);
        PromptForCardToPlay?.Invoke(this, args);

        if (args.PlayedCard == null)
        {
            throw new CardWasNotSelectedException();
        }

        if (!Hand.Contains(args.PlayedCard))
        {
            throw new MissingCardException(args.PlayedCard);
        }

        return args.PlayedCard;
    }

    /// <summary>
    /// Copies the values of this object.
    /// </summary>
    /// <returns>An IPlayer containing the same properties as this object.</returns>
    public override IPlayer Clone()
    {
        return new HumanPlayer(Name, TeamIndex, PlayerIndex, AvatarNumber)
        {
            Hand = [.. Hand],
            IsGoingAlone = IsGoingAlone,
        };
    }

    /// <summary>
    /// Checks the player's hand for a lack of Aces, cards above 10, and trump.
    /// </summary>
    /// <param name="trump">The current trump suit.</param>
    /// <returns>Returns true to indicate there are no Aces, cards above 10, or trump in the player's hand.</returns>
    public override bool HasNoAceNoFaceNoTrump(Suit trump)
    {
        var hasNoAceNoFaceNoTrump = base.HasNoAceNoFaceNoTrump(trump);

        if (hasNoAceNoFaceNoTrump)
        {
            // Prompt the user to see if they want to invoke the rule.

            var eventArgs = new PromptForNoAceNoFaceNoTrumpRuleEventArgs();
            PromptForNoAceNoFaceNoTrumpRule?.Invoke(this, eventArgs);
            hasNoAceNoFaceNoTrump = eventArgs.InvokeRule;
        }

        return hasNoAceNoFaceNoTrump;
    }

    /// <summary>
    /// Retrieves a list of cards from the player's hand that are eligible to be placed under the kitty when
    /// going under.
    /// </summary>
    /// <param name="kittyCards">The kitty cards that will be added to the player's hand.</param>
    /// <returns>A list of cards from the player's hand to be placed into the kitty.</returns>
    public override List<Card> GetGoUnderCards(List<Card> kittyCards)
    {
        if (kittyCards.Count != 3)
        {
            throw new InvalidOperationException("Expected exactly 3 cards from the kitty.");
        }

        // Check if there are only 3 cards to discard, and if so, return those cards without prompting the
        // user.

        var discardCards = (  from card in Hand
                             where card.Rank == Rank.Ten || card.Rank == Rank.Nine
                            select card)
                           .ToList();
        if (discardCards.Count > 3)
        {
            // Get which cards to discard from the user.

            var args = new PromptForGoUnderCardsEventArgs();
            PromptForGoUnderCards?.Invoke(this, args);
            discardCards = args.DiscardCards;
        }

        if (discardCards.Count != 3)
        {
            throw new InvalidOperationException("Expected exactly 3 cards to be discarded.");
        }

        // Remove the found cards from the player's hand.

        Hand.RemoveAll(c => discardCards.Contains(c));

        // Add the kitty cards to the player's hand.

        Hand.AddRange(kittyCards);

        return discardCards;
    }
}
