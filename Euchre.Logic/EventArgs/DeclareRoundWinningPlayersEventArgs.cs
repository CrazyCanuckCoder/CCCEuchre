namespace Euchre.Logic.EventArgs;

public class DeclareRoundWinningPlayersEventArgs : System.EventArgs
{
    /// <summary>
    /// Sets the property for this class.
    /// </summary>
    /// <param name="winningPlayers">The list of winning players or player of the current round.</param>
    public DeclareRoundWinningPlayersEventArgs(IEnumerable<string> winningPlayers)
    {
        WinningPlayers = winningPlayers.ToList();
    }

    /// <summary>
    /// The list of winning players or player of the current round.
    /// </summary>
    public List<string> WinningPlayers { get; private set; }
}