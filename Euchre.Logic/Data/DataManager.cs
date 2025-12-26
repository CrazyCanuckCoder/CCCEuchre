using Euchre.Logic.Components;
using Euchre.Logic.Enums;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using System.IO;

namespace Euchre.Logic.Data;

internal class DataManager : IDataManager
{
    /// <summary>
    /// The name of the file where game data is saved and loaded from.
    /// </summary>
    internal const string FILE_NAME = "GameData.xml";
    private static readonly object _lockObject = new();

    /// <summary>
    /// Saves the information in the provided game state manager to an XML file.
    /// </summary>
    /// <param name="gameStateManager">The object containing the game's state.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void SaveGameState(GameStateManager gameStateManager)
    {
        lock (_lockObject)
        {
            // Validate the input parameter.

            ArgumentNullException.ThrowIfNull(gameStateManager);

            if (gameStateManager.Players is null || gameStateManager.Players.Length == 0)
            {
                throw new ArgumentException("No players exist in the game state manager.");
            }

            GameStateDS stateDS = new();

            // Add the players and their hands to the dataset.

            foreach (var player in gameStateManager.Players)
            {
                int playerID = stateDS.Player.AddPlayerRow(player.Name, player.IsHuman, player.TeamIndex,
                    player.PlayerIndex, player.IsGoingAlone, player.AvatarNumber).PlayerID;
                foreach (var card in player.Hand)
                {
                    stateDS.Hand.AddHandRow(playerID, (int)card.Suit, (int)card.Rank);
                }
            }

            // Save the trick information to the dataset.

            foreach (var trick in gameStateManager.CurrentRoundTricks)
            {
                int trickID = stateDS.Trick.AddTrickRow((int)trick.LeadSuit, (int)trick.Trump).TrickID;
                foreach (var playerCard in trick.Cards)
                {
                    int playerID = stateDS.Player.Where(p => p.Name == playerCard.Key.Name).First().PlayerID;
                    stateDS.TrickCards.AddTrickCardsRow(trickID, playerID, (int)playerCard.Value.Suit,
                        (int)playerCard.Value.Rank);
                }
            }

            // Add the team scores to the dataset.

            for (int idx = 0; idx < gameStateManager.TeamScores.Length; idx++)
            {
                stateDS.TeamScores.AddTeamScoresRow(idx, gameStateManager.TeamScores[idx]);
            }

            // Add the remaining game state information to the dataset.

            stateDS.GameData.AddGameDataRow(
                gameStateManager.Kitty is null ? 0 : (int)gameStateManager.Kitty.Suit,
                gameStateManager.Kitty is null ? 0 : (int)gameStateManager.Kitty.Rank,
                gameStateManager.Trump is null ? 0 : (int)gameStateManager.Trump,
                gameStateManager.Dealer is null ? 0 :
                    stateDS.Player.First(p => p.Name == gameStateManager.Dealer.Name).PlayerID,
                gameStateManager.TrumpCaller is null ? 0 :
                    stateDS.Player.First(p => p.Name == gameStateManager.TrumpCaller.Name).PlayerID,
                gameStateManager.AlonePlayer is null ? 0 :
                    stateDS.Player.First(p => p.Name == gameStateManager.AlonePlayer.Name).PlayerID,
                gameStateManager.NextTrickPlayer is null ? 0 :
                    stateDS.Player.First(p => p.Name == gameStateManager.NextTrickPlayer.Name).PlayerID,
                (int)gameStateManager.LastCompletedStage,
                gameStateManager.CurrentTrickNumber);

            // Save the dataset to the file.

            if (File.Exists(FILE_NAME))
            {
                File.Delete(FILE_NAME);
            }
            stateDS.WriteXml(FILE_NAME);
        }
    }


    /// <summary>
    /// Retrieves the game information from an existing game save file and creates an instance of the 
    /// GameStateManager class.
    /// </summary>
    /// <returns>An instance of the GameStateManager class based on the game save file.</returns>
    /// <exception cref="FileNotFoundException" />
    public static GameStateManager LoadGameState()
    {
        if (!File.Exists(FILE_NAME))
        {
            throw new FileNotFoundException("The game data file was not found.", FILE_NAME);
        }

        GameStateManager gameStateManager = new();
        GameStateDS stateDS = new();
        stateDS.ReadXml(FILE_NAME);
        var gameDataRow = stateDS.GameData.First();

        AddPlayers(gameStateManager, stateDS);
        AddPlayerBasedProperties(gameStateManager, stateDS, gameDataRow);
        AddRemainingProperties(gameStateManager, gameDataRow);
        AddPlayedTricks(gameStateManager, stateDS);
        AddTeamScores(gameStateManager, stateDS);

        return gameStateManager;
    }

    /// <summary>
    /// Updates the team scores in the game state manager with the scores from the dataset.
    /// </summary>
    /// <param name="gameStateManager">The reference to the game state manager.</param>
    /// <param name="stateDS">The reference to the dataset containing the information to load.</param>
    private static void AddTeamScores(GameStateManager gameStateManager, GameStateDS stateDS)
    {
        gameStateManager.TeamScores = new int[Constants.NUMBER_OF_TEAMS];
        foreach (var scoreRow in stateDS.TeamScores)
        {
            gameStateManager.TeamScores[scoreRow.TeamIndex] = scoreRow.TeamScore;
        }
    }

    /// <summary>
    /// Adds the properties that are not associated to a specific section of the game state manager.
    /// </summary>
    /// <param name="gameStateManager">The reference to the game state manager.</param>
    /// <param name="gameDataRow">A reference to the single row in the GameData table.</param>
    private static void AddRemainingProperties(GameStateManager gameStateManager,
        GameStateDS.GameDataRow gameDataRow)
    {
        gameStateManager.Kitty = new Card((Suit)gameDataRow.KittySuit, (Rank)gameDataRow.KittyRank);
        gameStateManager.Trump = (Suit?)gameDataRow.TrumpSuit;
        gameStateManager.LastCompletedStage = (RoundStage)gameDataRow.LastCompletedStage;
        gameStateManager.CurrentTrickNumber = gameDataRow.CurrentTrickNumber;
    }

    /// <summary>
    /// Loads the tricks from the dataset to the game state manager.
    /// </summary>
    /// <param name="gameStateManager">The reference to the game state manager.</param>
    /// <param name="stateDS">The reference to the dataset containing the information to load.</param>
    private static void AddPlayedTricks(GameStateManager gameStateManager, GameStateDS stateDS)
    {
        gameStateManager.ResetTricksWonByPlayers();
        gameStateManager.CurrentRoundTricks = [];
        foreach (var trickRow in stateDS.Trick)
        {
            Trick newTrick = new((Suit)trickRow.TrumpSuit);
            var foundCards = stateDS.TrickCards.Where(tc => tc.TrickID == trickRow.TrickID);
            foreach (var cardRow in foundCards)
            {
                var player = gameStateManager.Players!
                    .First(p => p.Name == stateDS.Player
                        .First(r => r.PlayerID == cardRow.PlayerID).Name);
                newTrick.AddCard(player, new Card((Suit)cardRow.Suit, (Rank)cardRow.Rank));
            }
            gameStateManager.CurrentRoundTricks.Add(newTrick);
            gameStateManager.TricksWonByPlayers[newTrick.GetWinner().PlayerIndex]++;
        }
    }

    /// <summary>
    /// Adds the data that is player related to the game state manager.
    /// </summary>
    /// <param name="gameStateManager">The reference to the game state manager.</param>
    /// <param name="stateDS">The reference to the dataset containing the information to load.</param>
    /// <param name="gameDataRow">A reference to the single row in the GameData table.</param>
    private static void AddPlayerBasedProperties(GameStateManager gameStateManager, GameStateDS stateDS,
        GameStateDS.GameDataRow gameDataRow)
    {
        if (gameDataRow.DealerID > 0)
        {
            gameStateManager.Dealer = GetPlayerFromPlayerID(gameStateManager, stateDS,
                gameDataRow.DealerID);
        }
        if (gameDataRow.TrumpCallerID > 0)
        {
            gameStateManager.TrumpCaller = GetPlayerFromPlayerID(gameStateManager, stateDS,
                gameDataRow.TrumpCallerID);
        }
        if (gameDataRow.AlonePlayerID > 0)
        {
            gameStateManager.AlonePlayer = GetPlayerFromPlayerID(gameStateManager, stateDS,
                gameDataRow.AlonePlayerID);
        }
        if (gameDataRow.NextTrickPlayerID > 0)
        {
            gameStateManager.NextTrickPlayer = GetPlayerFromPlayerID(gameStateManager, stateDS,
                gameDataRow.NextTrickPlayerID);
        }
        gameStateManager.GoingAlone = gameStateManager.TrumpCaller != null
            && gameStateManager.TrumpCaller.IsGoingAlone;
    }

    /// <summary>
    /// Adds the players information to the game state manager.
    /// </summary>
    /// <param name="gameStateManager">The reference to the game state manager.</param>
    /// <param name="stateDS">The reference to the dataset containing the information to load.</param>
    private static void AddPlayers(GameStateManager gameStateManager, GameStateDS stateDS)
    {
        gameStateManager.Players = new IPlayer[Constants.NUMBER_OF_PLAYERS];
        int playerIdx = 0;
        foreach (var playerRow in stateDS.Player)
        {
            gameStateManager.Players[playerIdx] = CreatePlayerFromDataRow(playerRow, gameStateManager);
            var foundCards = stateDS.Hand.Where(h => h.PlayerID == playerRow.PlayerID);
            foreach (var cardRow in foundCards)
            {
                gameStateManager.Players[playerIdx].AddCard(new Card((Suit)cardRow.Suit, (Rank)cardRow.Rank));
            }
            playerIdx++;
        }
    }

    /// <summary>
    /// Retrieves the player that matches a specified player ID.
    /// </summary>
    /// <param name="gameStateManager">The class containing the list of players.</param>
    /// <param name="stateDS">The database with the player ID.</param>
    /// <param name="playerID">The ID of the player to find.</param>
    /// <returns>A player matching the specified player ID.</returns>
    private static IPlayer GetPlayerFromPlayerID(GameStateManager gameStateManager, GameStateDS stateDS,
        int playerID)
    {
        return gameStateManager.Players!
            .First(p => p.Name == stateDS.Player.First(r => r.PlayerID == playerID).Name);
    }

    /// <summary>
    /// Creates an instance of either the HumanPlayer or AutomatedPlayer class from the information in a row 
    /// from the Players table of the dataset.
    /// </summary>
    /// <param name="playerRow">The data row containing the information about the player.</param>
    /// <param name="gameStateManager">The instance of the game state manager for use by the AutomatedPlayer.
    /// </param>
    /// <returns>An instance of either the HumanPlayer or AutomatedPlayer class depending on information from 
    /// the data row.</returns>
    private static IPlayer CreatePlayerFromDataRow(GameStateDS.PlayerRow playerRow,
        GameStateManager gameStateManager)
    {
        IPlayer newPlayer;

        if (playerRow.IsHuman)
        {
            newPlayer = new HumanPlayer(playerRow.Name, playerRow.TeamIndex, playerRow.PlayerIndex,
                playerRow.AvatarNumber);
        }
        else
        {
            newPlayer = new AutomatedPlayer(playerRow.Name, playerRow.TeamIndex, playerRow.PlayerIndex,
                gameStateManager, playerRow.AvatarNumber);
        }

        newPlayer.IsGoingAlone = playerRow.IsGoingAlone;

        return newPlayer;
    }
}
