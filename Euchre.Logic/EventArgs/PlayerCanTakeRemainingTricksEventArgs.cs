using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class PlayerCanTakeRemainingTricksEventArgs : System.EventArgs
{
    public PlayerCanTakeRemainingTricksEventArgs(IPlayer playerToTakeTricks)
    {
        PlayerToTakeTricks = playerToTakeTricks;
    }

    /// <summary>
    /// A reference to the player who can take the remaining tricks.
    /// </summary>
    public IPlayer PlayerToTakeTricks { get; set; }
}
