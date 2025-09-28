using Euchre.Logic.Helpers;

namespace Euchre.Logic.EventArgs;

public class GameOverEventArgs
{
    public GameOverEventArgs(GameStateManager gameInfo)
    {
        GameInfo = gameInfo;
    }

    /// <summary>
    /// Contains the final state of the game when it ends.
    /// </summary>
    public GameStateManager GameInfo { get; set; }
}