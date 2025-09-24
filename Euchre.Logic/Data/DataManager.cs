using Euchre.Logic.Components;
using Euchre.Logic.Enums;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
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

        // Add the players first.

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

        // Now that we have the players, add the properties based on the players.

        if (stateDS.GameData.First().DealerID != 0)
        {
            gameStateManager.Dealer = gameStateManager.Players
                .Where(p => p.Name == stateDS.Player
                    .Where(r => r.PlayerID == stateDS.GameData.First().DealerID)
                    .First().Name)
                .First();
        }
        if (stateDS.GameData.First().TrumpCallerID != 0)
        {
            gameStateManager.TrumpCaller = gameStateManager.Players
                .Where(p => p.Name == stateDS.Player
                    .Where(r => r.PlayerID == stateDS.GameData.First().TrumpCallerID)
                    .First().Name)
                .First();
        }
        if (stateDS.GameData.First().AlonePlayerID != 0)
        {
            gameStateManager.AlonePlayer = gameStateManager.Players
                .Where(p => p.Name == stateDS.Player
                    .Where(r => r.PlayerID == stateDS.GameData.First().AlonePlayerID)
                    .First().Name)
                .First();
        }
        if (stateDS.GameData.First().NextTrickPlayerID != 0)
        {
            gameStateManager.NextTrickPlayer = gameStateManager.Players
                .Where(p => p.Name == stateDS.Player
                    .Where(r => r.PlayerID == stateDS.GameData.First().NextTrickPlayerID)
                    .First().Name)
                .First();
        }

        // Setup the tricks for the current round.

        gameStateManager.CurrentRoundTricks = [];
        foreach (var trickRow in stateDS.Trick)
        {
            Trick newTrick = new((Suit)trickRow.TrumpSuit);
            var foundCards = stateDS.TrickCards.Where(tc => tc.TrickID == trickRow.TrickID);
            foreach (var cardRow in foundCards)
            {
                var player = gameStateManager.Players
                    .Where(p => p.Name == stateDS.Player
                        .Where(r => r.PlayerID == cardRow.PlayerID)
                        .First().Name)
                    .First();
                newTrick.AddCard(player, new Card((Suit)cardRow.Suit, (Rank)cardRow.Rank));
            }
            gameStateManager.CurrentRoundTricks.Add(newTrick);
        }

        // Setup the team scores.

        gameStateManager.TeamScores = new int[Constants.NUMBER_OF_TEAMS];
        foreach (var scoreRow in stateDS.TeamScores)
        {
            gameStateManager.TeamScores[scoreRow.TeamIndex] = scoreRow.TeamScore;
        }

        // Setup the remaining properties.

        gameStateManager.Deck = new Deck();
        gameStateManager.Kitty = stateDS.GameData.First().KittySuit == 0 ? null :
            new Card((Suit)stateDS.GameData.First().KittySuit, (Rank)stateDS.GameData.First().KittyRank);
        gameStateManager.Trump = stateDS.GameData.First().TrumpSuit == 0 ? null :
            (Suit?)stateDS.GameData.First().TrumpSuit;
        gameStateManager.LastCompletedStage = (RoundStage)stateDS.GameData.First().LastCompletedStage;
        gameStateManager.CurrentTrickNumber = stateDS.GameData.First().CurrentTrickNumber;

        return gameStateManager;
    }

    private static IPlayer CreatePlayerFromDataRow(GameStateDS.PlayerRow playerRow, 
        GameStateManager gameStateManager)
    {
        IPlayer newPlayer;

        if (playerRow.IsHuman)
        {
            newPlayer = new HumanPlayer(playerRow.Name, playerRow.TeamIndex);
        }
        else
        {
            newPlayer = new AutomatedPlayer(playerRow.Name, playerRow.TeamIndex, gameStateManager);
        }

        newPlayer.IsGoingAlone = playerRow.IsGoingAlone;

        return newPlayer;
    }
}
