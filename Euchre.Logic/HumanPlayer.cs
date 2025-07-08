using Euchre.Logic.EventArgs;
using Euchre.Logic.Exceptions;

namespace Euchre.Logic;

public class HumanPlayer : Player
{
    public HumanPlayer(string name) : base(name, true) 
    { 
    }

    /// <summary>
    /// Fired when to determine if the user wants to order up the kitty card.
    /// </summary>
    public event EventHandler<PromptToOrderUpEventArgs> PromptToOrderUp;

    /// <summary>
    /// Fired to determine if the user wants to call trump.
    /// </summary>
    public event EventHandler<PromptForTrumpSuitEventArgs> PromptForTrumpSuit;

    /// <summary>
    /// Fired to prompt the user to discard a card from their hand in exchange for the kitty card.
    /// </summary>
    public event EventHandler<PromptForDiscardEventArgs> PromptForDiscard;

    /// <summary>
    /// Fired to prompt the user to select a card to play in the current trick.
    /// </summary>
    public event EventHandler<PromptForCardToPlayEventArgs> PromptForCardToPlay;

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

        return args.OrderedUp;
    }

    /// <summary>
    /// Prompts the user to discard a card from their hand in exchange for the kitty card.
    /// </summary>
    /// <param name="kitty">The card facing up on top of the kitty.</param>
    /// <param name="trump">The suit designated as trump.</param>
    /// <exception cref="MissingCardException"></exception>
    /// <exception cref="CardWasNotSelectedException"></exception>
    public override void DiscardForKitty(Card kitty, Suit trump)
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
