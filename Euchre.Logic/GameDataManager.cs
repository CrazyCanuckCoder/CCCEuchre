using Euchre.Logic.Interfaces;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

/// <summary>
/// Manages the game data, including player information, deck, scores, and current hand tricks.
/// </summary>
[Serializable]
public class GameDataManager
{
    public GameDataManager()
    {
        Deck = new Deck();
        TeamScores = new int[NUMBER_OF_TEAMS];
        CurrentRoundTricks = [];
    }

    /// <summary>
    /// The name of the file where game data is saved and loaded from.
    /// </summary>
    private const string FILE_NAME = "GameData.json";

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
    /// Saves the current game data to a file.
    /// </summary>
    public void SaveGameData()
    {
        DataPersistence.SaveToFile(this, FILE_NAME);
    }

    /// <summary>
    /// Loads the game data from a file.
    /// </summary>
    /// <returns>An instance of this class with the loaded data.</returns>
    public static GameDataManager LoadGameData()
    {
        return DataPersistence.LoadFromFile<GameDataManager>(FILE_NAME);
    }
}
