using Euchre.Logic.Components;
using Euchre.Logic.Enums;

namespace Euchre.Logic.Interfaces;

public interface IGameStateManager
{
    IPlayer? AlonePlayer { get; set; }
    List<Trick> CurrentRoundTricks { get; set; }
    int CurrentTrickNumber { get; set; }
    IPlayer? Dealer { get; set; }
    Deck Deck { get; set; }
    bool GoingAlone { get; set; }
    Card? Kitty { get; set; }
    RoundStage LastCompletedStage { get; set; }
    IPlayer? NextTrickPlayer { get; set; }
    IPlayer[]? Players { get; set; }
    bool RestartGame { get; set; }
    int[] TeamScores { get; set; }
    int[] TricksWonByPlayers { get; set; }
    Suit? Trump { get; set; }
    IPlayer? TrumpCaller { get; set; }

    void ResetRoundCheckpoint();
    void ResetTricksWonByPlayers();
    void SaveGameData();
}