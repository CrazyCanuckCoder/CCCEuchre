using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class GameOverEventArgs
{
    public GameOverEventArgs(IGameStateManager gameInfo)
    {
        GameInfo = gameInfo;
    }

    /// <summary>
    /// Contains the final state of the game when it ends.
    /// </summary>
    public IGameStateManager GameInfo { get; set; }
}