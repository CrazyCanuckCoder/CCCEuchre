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
        InitializeGame(playerNames);
    }

    public GameDataManager GameInfo { get; private set; }

    private void InitializeGame(string[] playerNames)
    {
        if (playerNames.Length != NUMBER_OF_PLAYERS)
        {
            throw new InvalidNumberOfPlayersException();
        }

        GameInfo = new GameDataManager();
        var players = new IPlayer[NUMBER_OF_PLAYERS];
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            players[i] = new AutomatedPlayer(playerNames[i], i % NUMBER_OF_PLAYERS, GameInfo); // All AI for now
        }

        GameInfo.Players = players;
        GameInfo.Dealer = players[0];
    }

    public void PlayGame()
    {
        while (GameInfo.TeamScores[0] < WINNING_SCORE && GameInfo.TeamScores[1] < WINNING_SCORE)
        {
            PlayRound();
        }
        
        int winningTeam = GameInfo.TeamScores[0] >= WINNING_SCORE ? 0 : 1;
    }

    private void PlayRound()
    {
        // Reset for new round.

        GameInfo.CurrentRoundTricks.Clear();
        GameInfo.Trump = null;
        GameInfo.TrumpCaller = null;
        GameInfo.GoingAlone = false;
        GameInfo.AlonePlayer = null;
        
        // Deal cards.

        DealCards();

        // Bidding phase.

        if (!BiddingPhase())
        {
            // No one ordered up. Dealing new round.

            AdvanceDealer();
            return;
        }

        // Play up to 5 tricks.

        IPlayer leader = GetNextPlayer(GameInfo.Dealer);
        for (int trickNum = 0; trickNum < MAX_NUMBER_OF_TRICKS; trickNum++)
        {
            var trick = PlayTrick(leader, trickNum + 1);
            GameInfo.CurrentRoundTricks.Add(trick);
            leader = trick.GetWinner();

            // TODO: Add logic to see if the round should end early if one team has already won 3 tricks and
            //       couldn't win any other tricks or be caught.
        }

        // Score the hand.

        ScoreRound();
        
        // Determine next dealer.

        AdvanceDealer();
    }

    private void DealCards()
    {
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
    }

    private bool BiddingPhase()
    {
        // Round 1 - Bid for Kitty suit.

        if (BiddingRound(1, GameInfo.Kitty.Suit))
            return true;
        
        // Round 2 - Bid the other suits.

        return BiddingRound(2, null);
    }

    private bool BiddingRound(int round, Suit? forcedSuit)
    {
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

    private Trick PlayTrick(IPlayer leader, int trickNumber)
    {
        var trick = new Trick(GameInfo.Trump.Value);
        
        IPlayer currentPlayer = leader;
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            // Skip partner if going alone.

            if (GameInfo.GoingAlone && IsPartner(GameInfo.AlonePlayer, currentPlayer) && currentPlayer != GameInfo.AlonePlayer)
            {
                Console.WriteLine($"{currentPlayer.Name} sits out (partner going alone)");
                currentPlayer = GetNextPlayer(currentPlayer);
                continue;
            }

            Suit? leadSuit = trick.Cards.Count > 0 ? trick.LeadSuit : null;
            
            var playedCard = currentPlayer.SelectCardToPlay(
                trick, 
                GameInfo.Trump.Value, 
                leadSuit);
            
            // Find the card in hand and play it.

            var cardIndex = currentPlayer.Hand.IndexOf(playedCard);
            playedCard = currentPlayer.PlayCard(cardIndex);
            
            trick.AddCard(currentPlayer, playedCard);

            currentPlayer = GetNextPlayer(currentPlayer);
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
            if (team == 0) team0Tricks++;
            else team1Tricks++;
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
            int opposingTeam = 1 - callerTeam;
            GameInfo.TeamScores[opposingTeam] += 2; // Euchred
        }

        Console.ReadLine();
    }

    private IPlayer GetNextPlayer(IPlayer current)
    {
        int currentIndex = Array.IndexOf(GameInfo.Players, current);
        return GameInfo.Players[(currentIndex + 1) % NUMBER_OF_PLAYERS];
    }

    private void AdvanceDealer()
    {
        GameInfo.Dealer = GetNextPlayer(GameInfo.Dealer);
    }

    private bool IsPartner(IPlayer player1, IPlayer player2)
    {
        return player1.TeamIndex == player2.TeamIndex && player1 != player2;
    }
}
