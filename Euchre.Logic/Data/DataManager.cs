using Euchre.Logic.Helpers;
using System.IO;

namespace Euchre.Logic.Data;

internal class DataManager
{

    /// <summary>
    /// The name of the file where game data is saved and loaded from.
    /// </summary>
    internal const string FILE_NAME = "GameData.xml";

    /// <summary>
    /// Saves the information in the provided game state manager to an XML file.
    /// </summary>
    /// <param name="gameStateManager">The object containing the game's state.</param>
    /// <exception cref="ArgumentException"></exception>
    public static void SaveGameState(GameStateManager gameStateManager)
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
                player.IsGoingAlone).PlayerID;
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
                stateDS.Player.Where(p => p.Name == gameStateManager.Dealer.Name).First().PlayerID,
            gameStateManager.TrumpCaller is null ? 0 :
                stateDS.Player.Where(p => p.Name == gameStateManager.TrumpCaller.Name).First().PlayerID,
            gameStateManager.AlonePlayer is null ? 0 :
                stateDS.Player.Where(p => p.Name == gameStateManager.AlonePlayer.Name).First().PlayerID,
            gameStateManager.NextTrickPlayer is null ? 0 :
                stateDS.Player.Where(p => p.Name == gameStateManager.NextTrickPlayer.Name).First().PlayerID,
            (int)gameStateManager.LastCompletedStage,
            gameStateManager.CurrentTrickNumber);

        // Save the dataset to the file.

        if (File.Exists(FILE_NAME))
        {
            File.Delete(FILE_NAME);
        }
        stateDS.WriteXml(FILE_NAME);
    }

    public static GameStateManager LoadGameState()
    {
        if (!File.Exists(FILE_NAME))
        {
            throw new FileNotFoundException("The game data file was not found.", FILE_NAME);
        }

        GameStateManager gameStateManager = new();
        GameStateDS stateDS = new();
        stateDS.ReadXml(FILE_NAME);


        return gameStateManager;
    }
}
