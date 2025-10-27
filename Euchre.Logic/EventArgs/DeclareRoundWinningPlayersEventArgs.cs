using Euchre.Logic.Enums;

namespace Euchre.Logic.EventArgs;

public class DeclareRoundWinningPlayersEventArgs : System.EventArgs
{
    /// <summary>
    /// Sets the property for this class.
    /// </summary>
    /// <param name="winningPlayers">The list of winning players or player of the current round.</param>
    /// <param name="points">The number of points won.</param>
    /// <param name="scoringReason">The explanation of how the points were won.</param>
    public DeclareRoundWinningPlayersEventArgs(IEnumerable<string> winningPlayers, int points, 
        ScoringReason scoringReason)
    {
        WinningPlayers = winningPlayers.ToList();
        Points = points;
        ReasonForPoints = scoringReason;
    }

    /// <summary>
    /// The list of winning players or player of the current round.
    /// </summary>
    public List<string> WinningPlayers { get; private set; }

    /// <summary>
    /// The number of points won.
    /// </summary>
    public int Points { get; private set; }

    /// <summary>
    /// The explanation of how the points were won.
    /// </summary>
    public ScoringReason ReasonForPoints { get; private set; }
}