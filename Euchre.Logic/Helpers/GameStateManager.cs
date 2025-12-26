using Euchre.Logic.Components;
using Euchre.Logic.Data;
using Euchre.Logic.Enums;
using Euchre.Logic.Interfaces;
using System.IO;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Helpers;

/// <summary>
/// Manages the game state, including player information, deck, scores, and current hand tricks.
/// </summary>
public class GameStateManager : IGameStateManager
{
    public GameStateManager()
    {
        Deck = new Deck();
        TeamScores = new int[NUMBER_OF_TEAMS];
        CurrentRoundTricks = [];
    }

    /// <summary>
    /// The list of players in the game. The first two players are on one team, and the last two players are 
    /// on the other team.
    /// </summary>
    public IPlayer[]? Players { get; set; }

    /// <summary>
    /// The deck of cards used in the game, which is shuffled and dealt to players at the start of each round.
    /// </summary>
    public Deck Deck { get; set; }

    /// <summary>
    /// The kitty card, which is the upturned card from the kitty that players can order up or exchange if
    /// they are the dealer.
    /// </summary>
    public Card? Kitty { get; set; }

    /// <summary>
    /// The trump suit for the current round. This is determined during the bidding phase when a player calls
    /// trump or orders up the kitty card.
    /// </summary>
    public Suit? Trump { get; set; }

    /// <summary>
    /// The dealer for the current round.
    /// </summary>
    public IPlayer? Dealer { get; set; }

    /// <summary>
    /// The player who called trump for the current round.
    /// </summary>
    public IPlayer? TrumpCaller { get; set; }

    /// <summary>
    /// Stores the scores for each team in the game. Team 0 (Players 0,2), Team 1 (Players 1,3).
    /// </summary>
    public int[] TeamScores { get; set; }

    /// <summary>
    /// The list of tricks played in the current round. Each trick contains the cards played by each player.
    /// </summary>
    public List<Trick> CurrentRoundTricks { get; set; }

    /// <summary>
    /// True if the game is currently in a hand where one player has declared they are going alone, and false
    /// if otherwise.
    /// </summary>
    public bool GoingAlone { get; set; }

    /// <summary>
    /// The player who is going alone in the current hand, if applicable.
    /// </summary>
    public IPlayer? AlonePlayer { get; set; }

    /// <summary>
    /// Indicates the player who is to play the next card in the current trick.
    /// </summary>
    public IPlayer? NextTrickPlayer { get; set; }

    /// <summary>
    /// True to indicate the game should be restarted, which can be used to reset the game state.
    /// </summary>
    public bool RestartGame { get; set; }

    /// <summary>
    /// The stage we were in when the app last saved.
    /// </summary>
    public RoundStage LastCompletedStage { get; set; } = RoundStage.None;

    /// <summary>
    /// If we stopped mid‑trick, remember which trick number we were on.
    /// </summary>
    public int CurrentTrickNumber { get; set; } = 0;

    /// <summary>
    /// Tracks the number of tricks won by each player for the current round.
    /// </summary>
    public int[] TricksWonByPlayers { get; set; } = new int[NUMBER_OF_PLAYERS];

    /// <summary>
    /// Helper to reset checkpoint info when a brand‑new round begins.
    /// </summary>
    public void ResetRoundCheckpoint()
    {
        LastCompletedStage = RoundStage.ResetRoundDone;
        CurrentTrickNumber = 0;
        RestartGame = false;
    }

    /// <summary>
    /// Resets the tricks won by players property for the next round.
    /// </summary>
    public void ResetTricksWonByPlayers()
    {
        for (int index = 0; index < NUMBER_OF_PLAYERS; index++)
        {
            TricksWonByPlayers[index] = 0;
        }
    }

    /// <summary>
    /// Saves the current game data to a file.
    /// </summary>
    public void SaveGameData()
    {
        GameStateManager gsManagerClone = Clone();
        DataManager.SaveGameState(gsManagerClone);
    }

    /// <summary>
    /// Returns true if saved game data exists; otherwise, false.
    /// </summary>
    /// <returns>A boolean indicating whether saved game data exists.</returns>
    public static bool DataExists()
    {
        return File.Exists(DataManager.FILE_NAME);
    }

    /// <summary>
    /// Loads the game data from a file.
    /// </summary>
    /// <returns>An instance of this class with the loaded data.</returns>
    public static GameStateManager LoadGameData()
    {
        return DataManager.LoadGameState();
    }

    /// <summary>
    /// Deletes any saved game data file.
    /// </summary>
    public static void ClearSavedGameData()
    {
        if (DataExists())
        {
            File.Delete(DataManager.FILE_NAME);
        }
    }

    /// <summary>
    /// Clones the values of this class and creates a new instance.
    /// </summary>
    /// <returns>A new instance of this class with the same values.</returns>
    private GameStateManager Clone()
    {
        return new GameStateManager()
        {
            Players = (IPlayer[])Players!.Clone(),
            Deck = Deck.Clone(),
            Kitty = (Card?)Kitty?.Clone(),
            Trump = Trump,
            Dealer = Dealer?.Clone(),
            TrumpCaller = TrumpCaller?.Clone(),
            TeamScores = (int[])TeamScores.Clone(),
            CurrentRoundTricks = [.. CurrentRoundTricks],
            GoingAlone = GoingAlone,
            AlonePlayer = AlonePlayer?.Clone(),
            NextTrickPlayer = NextTrickPlayer?.Clone(),
            RestartGame = RestartGame,
            LastCompletedStage = LastCompletedStage,
            CurrentTrickNumber = CurrentTrickNumber,
            TricksWonByPlayers = (int[])TricksWonByPlayers.Clone(),
        };
    }
}
