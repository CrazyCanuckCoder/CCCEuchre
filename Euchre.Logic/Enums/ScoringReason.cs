namespace Euchre.Logic.Enums;

/// <summary>
/// Describes how the points awarded for winning a round were obtained.
/// </summary>
public enum ScoringReason
{
    None,
    WonHand,
    GotAllTricks,
    Euchred,
    GotAllTricksAlone,
}
