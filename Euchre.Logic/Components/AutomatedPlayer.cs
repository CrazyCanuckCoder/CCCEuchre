using Euchre.Logic.Data;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Components;

/// <summary>
/// Represents the non human players in this game.
/// </summary>
public class AutomatedPlayer : Player
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutomatedPlayer"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the automated player. This value cannot be null or empty.</param>
    /// <param name="dataManager">The <see cref="GameStateManager"/> instance that provides access to the 
    /// game's state.</param>
    public AutomatedPlayer(string name, int teamIndex, int playerIndex, GameStateManager dataManager, int avatarNumber) : 
        base(name, teamIndex, playerIndex, false, avatarNumber) 
    {
        _bidder = new Bidder(this);
        _dataManager = dataManager;
        _playManager = new PlayManager(dataManager, this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AutomatedPlayer"/> class with the specified name, 
    /// bidder, and play manager.
    /// </summary>
    /// <param name="name">The name of the automated player. Cannot be null or empty.</param>
    /// <param name="bidder">The <see cref="IBidder"/> implementation used by the player to make bids. If 
    /// null, a default bidder is created using the player's hand.</param>
    /// <param name="playManager">The <see cref="IPlayManager"/> implementation used by the player to manage
    /// plays. If null, a default play manager is created using the player's hand.</param>
    /// <param name="dataManager">The <see cref="GameStateManager"/> instance that provides access to the 
    /// game's state.</param>
    public AutomatedPlayer(string name, int teamIndex, int playerIndex, IBidder bidder, IPlayManager playManager,
        GameStateManager dataManager, int avatarNumber) : base(name, teamIndex, playerIndex, false, avatarNumber)
    {
        _bidder = bidder ?? new Bidder(this);
        _dataManager = dataManager;
        _playManager = playManager ?? new PlayManager(dataManager, this);
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
    /// Represents the state manager used for handling the state of the game.
    /// </summary>
    private readonly GameStateManager _dataManager;

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
    /// Determines whether to order up the given card during the bidding phase.
    /// </summary>
    /// <param name="kitty">The card being considered for ordering up.</param>
    /// <param name="isDealer">A boolean value indicating whether the current player is the dealer.</param>
    /// <returns><see langword="true"/> if the player decides to order up the card; otherwise, 
    /// <see langword="false"/>.</returns>
    public override bool OrderUp(Card kitty, bool isDealer)
    {
        return _bidder.DetermineWhetherToOrderUp(kitty, isDealer, out _goAlone);
    }

    /// <summary>
    /// Determines the trump suit for the game.
    /// </summary>
    /// <param name="kitty">The card from the kitty indicating which suit cannot be called trump.</param>
    /// <returns>The trump suit selected for the game, or <see langword="null"/> if no trump suit is 
    /// determined which indicates the automated player will pass.</returns>
    public override Suit? CallTrump(Card kitty)
    {
        return _bidder.DetermineTrump(kitty, out _goAlone);
    }

    /// <summary>
    /// Determines which card in the player's hand will be replaced by the kitty's upturned card and replaces
    /// it in the player's hand.
    /// </summary>
    /// <param name="kitty">The upturned card from the kitty.</param>
    public override void DiscardForKitty(Card kitty)
    {
        _bidder.DiscardForKitty(kitty);
    }

    /// <summary>
    /// Selects the card to play for the current trick based on the provided game state.
    /// </summary>
    /// <param name="trick">The current trick containing the cards played so far.</param>
    /// <param name="trump">The trump suit for the game.</param>
    /// <param name="leadSuit">The suit that was led for the trick, or <see langword="null"/> if no suit has 
    /// been led yet.</param>
    /// <returns>The card selected to play in the current trick.</returns>
    public override Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit)
    {
        return _playManager.DetermineCardToPlay(trick, trump, leadSuit);
    }

    /// <summary>
    /// Copies the values of this object.
    /// </summary>
    /// <returns>An IPlayer containing the same properties as this object.</returns>
    public override IPlayer Clone()
    {
        return new AutomatedPlayer(Name, TeamIndex, PlayerIndex, _dataManager, AvatarNumber)
        {
            Hand = [.. Hand],
            IsGoingAlone = IsGoingAlone,
        };
    }
}
