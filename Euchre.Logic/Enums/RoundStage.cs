namespace Euchre.Logic.Enums;

/// <summary>
/// Checkpoint flags – they survive a shutdown and tell us where to resume.  All are persisted together 
/// with the rest of the game data.
/// </summary>
public enum RoundStage
{
    None,               // No round started yet (e.g., fresh game)
    ResetRoundDone,     // ResetRound finished, deck shuffled, hands cleared
    CardsDealt,         // DealCards finished, kitty set
    TrumpChosen,        // PlayersChoseTrump finished (someone called trump)
    TricksPlayed,       // All tricks for the round have been played
    Scored,             // ScoreRound finished
    DealerAdvanced      // AdvanceDealer finished
}
