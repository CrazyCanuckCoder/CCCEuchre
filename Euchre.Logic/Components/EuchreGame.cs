using Euchre.Logic.Exceptions;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using System;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Components;

public class EuchreGame
{
    public EuchreGame(string[] playerNames)
    {
        if (playerNames.Length != NUMBER_OF_PLAYERS)
        {
            throw new InvalidNumberOfPlayersException();
        }

        GameInfo = new GameDataManager();
        var players = new IPlayer[NUMBER_OF_PLAYERS];

        // Add the human player as the first player.

        players[0] = new HumanPlayer(playerNames[0], 0);

        // Add automated players.

        for (int i = 1; i < NUMBER_OF_PLAYERS; i++)
        {
            players[i] = new AutomatedPlayer(playerNames[i], i % NUMBER_OF_PLAYERS, GameInfo);
        }

        GameInfo.Players = players;
        GameInfo.Dealer = players[0];
        GameInfo.SaveGameData();
    }

    public GameDataManager GameInfo { get; private set; }

    public async Task PlayGameAsync()
    {
        // The main game loop is synchronous, but this method is now asynchronous for future UI or I/O
        //   integration.
        
        while (GameInfo.TeamScores[0] < WINNING_SCORE && GameInfo.TeamScores[1] < WINNING_SCORE)
        {
            await Task.Run(PlayRound);
        }
    }

    public async Task RestartGameAsync()
    {
        GameInfo = GameDataManager.LoadGameData();
        GameInfo.RestartGame = true;
        await PlayGameAsync();
    }

    private void PlayRound()
    {
        // TODO: Add logic to check if the game is restarted and determine where to resume from saved state.

        ResetRound();
        DealCards();

        if (PlayersChoseTrump())
        {
            PlayTricksForRound();
            ScoreRound();
        }

        AdvanceDealer();
    }

    private void ResetRound()
    {
        // Reset for new round.

        GameInfo.CurrentRoundTricks.Clear();
        GameInfo.Trump = null;
        GameInfo.TrumpCaller = null;
        GameInfo.GoingAlone = false;
        GameInfo.AlonePlayer = null;
        GameInfo.Kitty = null;
        GameInfo.SaveGameData();
    }

    private void DealCards()
    {
        if (GameInfo.Players == null)
        {
            throw new InvalidGameConditionException("Players must be initialized before dealing cards.");
        }

        GameInfo.Deck.Shuffle();
        
        // Clear hands.

        foreach (var player in GameInfo.Players)
        {
            player.ClearHand();
        }

        // Deal 5 cards to each player.

        for (int round = 0; round < CARDS_PER_PLAYER; round++)
        {
            for (int playerCount = 0; playerCount < NUMBER_OF_PLAYERS; playerCount++)
            {
                var playerIndex = (Array.IndexOf(GameInfo.Players, GameInfo.Dealer) + 1 + playerCount)
                    % NUMBER_OF_PLAYERS;
                GameInfo.Players[playerIndex].AddCard(GameInfo.Deck.Deal());
            }
        }

        // Set turned up card.

        GameInfo.Kitty = GameInfo.Deck.Deal();
        GameInfo.SaveGameData();
    }

    private bool PlayersChoseTrump()
    {
        if (GameInfo.Kitty == null)
        {
            throw new InvalidGameConditionException("The Kitty card must be selected before bidding round.");
        }

        // Round 1 - Bid for Kitty suit.

        if (BiddingRound(1, GameInfo.Kitty.Suit))
            return true;
        
        // Round 2 - Bid the other suits.

        return BiddingRound(2, null);
    }

    private bool BiddingRound(int round, Suit? forcedSuit)
    {
        if (GameInfo.Players == null)
        {
            throw new InvalidGameConditionException("Players must be initialized before dealing cards.");
        }
        if (GameInfo.Kitty == null)
        {
            throw new InvalidGameConditionException("The Kitty card must be selected before bidding round.");
        }
        if (GameInfo.Dealer == null)
        {
            throw new InvalidGameConditionException("The dealer must be set before bidding round.");
        }

        int startPlayerIndex = (Array.IndexOf(GameInfo.Players, GameInfo.Dealer) + 1) % NUMBER_OF_PLAYERS;
        
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            var playerIndex = (startPlayerIndex + i) % NUMBER_OF_PLAYERS;
            var player = GameInfo.Players[playerIndex];
            bool isDealer = player == GameInfo.Dealer;
            
            if (round == 1 && forcedSuit.HasValue)
            {
                if (player.OrderUp(GameInfo.Kitty, isDealer))
                {
                    GameInfo.Trump = forcedSuit.Value;
                    GameInfo.TrumpCaller = player;
                    if (player.IsGoingAlone)
                    {
                        GameInfo.GoingAlone = true;
                        GameInfo.AlonePlayer = player;
                    }

                    // TODO: Inform UI of the order up and if the player is going alone.

                    // Dealer picks up kitty.

                    GameInfo.Dealer.DiscardForKitty(GameInfo.Kitty, GameInfo.Trump.Value);
                    GameInfo.SaveGameData();

                    return true;
                }
                else
                {
                    // TODO: Inform UI that the player is passing.
                }
            }
            else if (round == 2)
            {
                var calledSuit = player.CallTrump(GameInfo.Kitty);
                if (calledSuit.HasValue)
                {
                    GameInfo.Trump = calledSuit.Value;
                    GameInfo.TrumpCaller = player;
                    if (player.IsGoingAlone)
                    {
                        GameInfo.GoingAlone = true;
                        GameInfo.AlonePlayer = player;
                    }

                    GameInfo.SaveGameData();

                    // TODO: Inform UI of the order up and if the player is going alone.

                    return true;
                }
                else
                {
                    // TODO: Inform UI that the player is passing.
                }
            }
        }
        
        return false;
    }

    private void PlayTricksForRound()
    {
        if (GameInfo.Dealer == null)
        {
            throw new InvalidGameConditionException("The dealer must be set before playing the round.");
        }

        // Play up to 5 tricks.

        GameInfo.NextTrickPlayer = GetNextPlayer(GameInfo.Dealer);
        for (int trickNum = 0; trickNum < MAX_NUMBER_OF_TRICKS; trickNum++)
        {
            var trick = PlayTrick(trickNum + 1);
            GameInfo.CurrentRoundTricks.Add(trick);
            GameInfo.NextTrickPlayer = trick.GetWinner();
            GameInfo.SaveGameData();

            // TODO: Add logic to see if the round should end early if one team has already won 3 tricks and
            //       couldn't win any other tricks or be caught.
        }
    }

    private Trick PlayTrick(int trickNumber)
    {
        if (GameInfo.Trump == null)
        {
            throw new InvalidGameConditionException("The trump suit must be set before playing a trick.");
        }
        if (GameInfo.NextTrickPlayer == null)
        {
            throw new InvalidGameConditionException(
                "The next trick player must be set before playing a trick.");
        }

        var trick = new Trick(GameInfo.Trump.Value);
        
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            // Skip partner if going alone.

            if (GameInfo.GoingAlone 
                && GameInfo.AlonePlayer != null 
                && IsPartner(GameInfo.AlonePlayer, GameInfo.NextTrickPlayer) 
                && GameInfo.NextTrickPlayer != GameInfo.AlonePlayer)
            {
                GameInfo.NextTrickPlayer = GetNextPlayer(GameInfo.NextTrickPlayer);
                GameInfo.SaveGameData();
                continue;
            }

            Suit? leadSuit = trick.Cards.Count > 0 ? trick.LeadSuit : null;
            
            var playedCard = GameInfo.NextTrickPlayer.SelectCardToPlay(
                trick, 
                GameInfo.Trump.Value, 
                leadSuit);
            
            // Find the card in hand and play it.

            var cardIndex = GameInfo.NextTrickPlayer.Hand.IndexOf(playedCard);
            playedCard = GameInfo.NextTrickPlayer.PlayCard(cardIndex);
            
            trick.AddCard(GameInfo.NextTrickPlayer, playedCard);

            GameInfo.NextTrickPlayer = GetNextPlayer(GameInfo.NextTrickPlayer);
            GameInfo.SaveGameData();
        }
        
        return trick;
    }

    private void ScoreRound()
    {
        int team0Tricks = 0;
        int team1Tricks = 0;
        
        foreach (var trick in GameInfo.CurrentRoundTricks)
        {
            var winner = trick.GetWinner();
            int team = winner.TeamIndex;
            if (team == 0)
            {
                team0Tricks++;
            }
            else
            {
                team1Tricks++;
            }
        }
        
        int callerTeam = GameInfo.TrumpCaller?.TeamIndex ?? -1;
        int callerTricks = callerTeam == 0 ? team0Tricks : team1Tricks;
        
        if (callerTricks >= 3)
        {
            if (callerTricks == MAX_NUMBER_OF_TRICKS)
            {
                if (GameInfo.GoingAlone)
                {
                    GameInfo.TeamScores[callerTeam] += 4; // Lone march
                }
                else
                {
                    GameInfo.TeamScores[callerTeam] += 2; // March
                }
            }
            else
            {
                GameInfo.TeamScores[callerTeam] += 1; // Made it
            }
        }
        else
        {
            int opposingTeam = callerTeam ^= 1;
            GameInfo.TeamScores[opposingTeam] += 2; // Euchred
        }

        GameInfo.SaveGameData();
    }

    private IPlayer GetNextPlayer(IPlayer current)
    {
        if (GameInfo.Players == null)
        {
            throw new InvalidGameConditionException(
                "The list of players must be set before finding the next player.");
        }
        int currentIndex = Array.IndexOf(GameInfo.Players, current);
        return GameInfo.Players[(currentIndex + 1) % NUMBER_OF_PLAYERS];
    }

    private void AdvanceDealer()
    {
        if (GameInfo.Dealer == null)
        {
            throw new InvalidGameConditionException("The dealer must be set before determining next dealer.");
        }
        GameInfo.Dealer = GetNextPlayer(GameInfo.Dealer);
    }

    private bool IsPartner(IPlayer player1, IPlayer player2)
    {
        return player1.TeamIndex == player2.TeamIndex && player1 != player2;
    }
}
