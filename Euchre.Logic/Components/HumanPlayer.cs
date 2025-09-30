using Euchre.Logic.EventArgs;
using Euchre.Logic.Exceptions;

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
    public HumanPlayer(string name, int teamIndex, int avatarNumber) : 
        base(name, teamIndex, true, avatarNumber) 
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
    /// Prompts the user on whether to order up the given card during the bidding phase.
    /// </summary>
    /// <param name="kitty">The card being considered for ordering up.</param>
    /// <param name="isDealer">A boolean value indicating whether the current player is the dealer.</param>
    /// <returns><see langword="true"/> if the player decides to order up the card; otherwise, 
    /// <see langword="false"/>.</returns>
    public override bool OrderUp(Card kitty, bool isDealer)
    {
        PromptToOrderUpEventArgs args = new(kitty);
        PromptToOrderUp?.Invoke(this, args);

        _goAlone = args.GoAlone;

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
    public override Suit? CallTrump(Card kitty)
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
}
