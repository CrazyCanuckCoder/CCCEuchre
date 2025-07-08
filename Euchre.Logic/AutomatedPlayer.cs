using Euchre.Logic.Interfaces;

namespace Euchre.Logic;

/// <summary>
/// Represents the non human players in this game.
/// </summary>
public class AutomatedPlayer : Player
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomatedPlayer"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the automated player. This value cannot be null or empty.</param>
    public AutomatedPlayer(string name) : base(name, false) 
    {
        _bidder = new Bidder(Hand);
        _playManager = new PlayManager(Hand);
    }

    /// <summary>
    /// Represents the logic for bidding.
    /// </summary>
    private readonly IBidder _bidder;

    /// <summary>
    /// Represents the play manager used to handle play-related operations.
    /// </summary>
    private readonly IPlayManager _playManager;

    /// <summary>
    /// Determines whether to order up the given card during the bidding phase.
    /// </summary>
    /// <param name="kitty">The card being considered for ordering up.</param>
    /// <param name="isDealer">A boolean value indicating whether the current player is the dealer.</param>
    /// <returns><see langword="true"/> if the player decides to order up the card; otherwise, 
    /// <see langword="false"/>.</returns>
    public override bool OrderUp(Card kitty, bool isDealer)
    {
        return _bidder.DetermineWhetherToOrderUp(kitty, isDealer);
    }

    public override Suit? CallTrump(Card kitty)
    {
        return _bidder.DetermineTrump(kitty);
    }

    public override void DiscardForKitty(Card kitty, Suit trump)
    {
        _bidder.DiscardForKitty(kitty, trump);
    }

    public override Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit)
    {
        return _playManager.DetermineCardToPlay(trick, trump, leadSuit);
    }
}
