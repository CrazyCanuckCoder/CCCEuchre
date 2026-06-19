using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class PlayerBidEventArgs
{
    /// <summary>
    /// Sets the values for this event args.
    /// </summary>
    /// <param name="player">The player who is making a bid or passing.</param>
    /// <param name="madeTrump">True to indicate the player made a bid and false for the player to pass.</param>
    /// <param name="trump">The suit that will be trump if the player made a bid.</param>
    /// <param name="isGoingAlone">True if the player is going alone when they made a bid.</param>
    /// <param name="isKittyRound">True if the players are bidding on the suit displayed by the card on top 
    /// of the kitty pile.</param>
    /// <param name="wentUnder">True to indicate the player went under and exchanged 3 low cards for the 3 
    /// cards in the kitty pile.</param>
    public PlayerBidEventArgs(IPlayer player, bool madeTrump, Suit? trump, bool isGoingAlone, 
        bool isKittyRound, bool wentUnder)
    {
        Player = player;
        MadeTrump = madeTrump;
        Trump = trump;
        IsGoingAlone = isGoingAlone;
        IsKittyRound = isKittyRound;
        WentUnder = wentUnder;
    }

    /// <summary>
    /// The player who is making a bid or passing.
    /// </summary>
    public IPlayer Player { get; set; }

    /// <summary>
    /// True to indicate the player made a bid and false for the player to pass.
    /// </summary>
    public bool MadeTrump { get; set; }

    /// <summary>
    /// The suit that will be trump if the player made a bid.
    /// </summary>
    public Suit? Trump { get; set; }

    /// <summary>
    /// True if the player is going alone when they made a bid.
    /// </summary>
    public bool IsGoingAlone { get; set; }

    /// <summary>
    /// True to indicate the players are bidding on the suit displayed by the card on top of the kitty pile.
    /// </summary>
    public bool IsKittyRound { get; set; }

    /// <summary>
    /// True to indicate the player went under and exchanged 3 low cards for the 3 cards in the kitty pile.
    /// </summary>
    public bool WentUnder { get; set; }

}