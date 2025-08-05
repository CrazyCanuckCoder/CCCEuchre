using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

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
            players[i] = new AutomatedPlayer(playerNames[i], GameInfo); // All AI for now
        }

        GameInfo.Players = players;
        GameInfo.Dealer = players[0];
    }

    public void PlayGame()
    {
        Console.WriteLine("Starting Euchre Game!");
        PrintTeams();
        
        while (GameInfo.TeamScores[0] < WINNING_SCORE && GameInfo.TeamScores[1] < WINNING_SCORE)
        {
            PlayRound();
            
            Console.WriteLine($"\nScores - Team 1: {GameInfo.TeamScores[0]}, Team 2: {GameInfo.TeamScores[1]}");
            Console.WriteLine(new string('=', 50));
            Console.ReadLine();
        }
        
        int winningTeam = GameInfo.TeamScores[0] >= WINNING_SCORE ? 0 : 1;
        Console.WriteLine($"\nGame Over! Team {winningTeam + 1} wins!");
        Console.WriteLine($"Final Score - Team 1: {GameInfo.TeamScores[0]}, Team 2: {GameInfo.TeamScores[1]}");
    }

    private void PrintTeams()
    {
        Console.WriteLine($"Team 1: {GameInfo.Players[0].Name} & {GameInfo.Players[2].Name}");
        Console.WriteLine($"Team 2: {GameInfo.Players[1].Name} & {GameInfo.Players[3].Name}");
        Console.ReadLine();
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
        
        Console.WriteLine($"\nDealer: {GameInfo.Dealer.Name}");
        Console.WriteLine($"Kitty: {GameInfo.Kitty}");
        Console.ReadLine();

        // Bidding phase.

        if (!BiddingPhase())
        {
            Console.WriteLine("No one ordered up. Dealing new round.");
            AdvanceDealer();
            Console.ReadLine();
            return;
        }
        
        Console.WriteLine($"\nTrump: {GameInfo.Trump}");
        Console.WriteLine($"Called by: {GameInfo.TrumpCaller?.Name}");
        if (GameInfo.GoingAlone)
            Console.WriteLine($"{GameInfo.AlonePlayer?.Name} is going alone!");
        Console.ReadLine();

        // Play 5 tricks.

        IPlayer leader = GetPlayerAfterDealer();
        for (int trickNum = 0; trickNum < MAX_NUMBER_OF_TRICKS; trickNum++)
        {
            var trick = PlayTrick(leader, trickNum + 1);
            GameInfo.CurrentRoundTricks.Add(trick);
            leader = trick.GetWinner();
            
            Console.WriteLine($"Trick {trickNum + 1} won by: {leader.Name}");
            Console.ReadLine();
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
        // Round 1 - Kitty suit
        if (BiddingRound(1, GameInfo.Kitty.Suit))
            return true;
        
        // Round 2 - Other suits
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

                    // Dealer picks up kitty
                    GameInfo.Dealer.DiscardForKitty(GameInfo.Kitty, GameInfo.Trump.Value);
                    
                    Console.WriteLine($"{player.Name} ordered up {GameInfo.Trump}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"{player.Name} passes");
                }
            }
            else if (round == 2)
            {
                var calledSuit = player.CallTrump(GameInfo.Kitty);
                if (calledSuit.HasValue)
                {
                    GameInfo.Trump = calledSuit.Value;
                    GameInfo.TrumpCaller = player;
                    Console.WriteLine($"{player.Name} calls {GameInfo.Trump}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"{player.Name} passes");
                }
            }
        }
        
        return false;
    }

    private Trick PlayTrick(IPlayer leader, int trickNumber)
    {
        var trick = new Trick(GameInfo.Trump.Value);
        
        Console.WriteLine($"\n--- Trick {trickNumber} ---");
        Console.WriteLine($"Leader: {leader.Name}");
        
        IPlayer currentPlayer = leader;
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            // Skip partner if going alone
            if (GameInfo.GoingAlone && IsPartner(GameInfo.AlonePlayer, currentPlayer) && currentPlayer != GameInfo.AlonePlayer)
            {
                Console.WriteLine($"{currentPlayer.Name} sits out (partner going alone)");
                currentPlayer = GetNextPlayer(currentPlayer);
                continue;
            }

            // TODO: The call to GetValidCards should be moved to the Player class and into the SelectCardToPlay method.
            Suit? leadSuit = trick.Cards.Count > 0 ? trick.LeadSuit : null;
            
            Console.WriteLine($"\n{currentPlayer.Name}'s turn");
            Console.WriteLine($"Hand: {string.Join(", ", currentPlayer.Hand.Select((c, idx) => $"{idx}: {c}"))}");
            if (leadSuit.HasValue)
                Console.WriteLine($"Must follow {leadSuit}");
            
            var playedCard = currentPlayer.SelectCardToPlay(
                trick, 
                GameInfo.Trump.Value, 
                leadSuit);
            
            // For AI, find the card in hand and play it
            var cardIndex = currentPlayer.Hand.IndexOf(playedCard);
            playedCard = currentPlayer.PlayCard(cardIndex);
            
            trick.AddCard(currentPlayer, playedCard);
            Console.WriteLine($"{currentPlayer.Name} plays: {playedCard}");
            Console.ReadLine();

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
            int team = GetPlayerTeam(winner);
            if (team == 0) team0Tricks++;
            else team1Tricks++;
        }
        
        int callerTeam = GetPlayerTeam(GameInfo.TrumpCaller);
        int callerTricks = callerTeam == 0 ? team0Tricks : team1Tricks;
        
        Console.WriteLine($"\nTricks won - Team 1: {team0Tricks}, Team 2: {team1Tricks}");
        
        if (callerTricks >= 3)
        {
            if (callerTricks == MAX_NUMBER_OF_TRICKS)
            {
                if (GameInfo.GoingAlone)
                {
                    GameInfo.TeamScores[callerTeam] += 4; // Lone march
                    Console.WriteLine($"Lone march! Team {callerTeam + 1} scores 4 points");
                }
                else
                {
                    GameInfo.TeamScores[callerTeam] += 2; // March
                    Console.WriteLine($"March! Team {callerTeam + 1} scores 2 points");
                }
            }
            else
            {
                GameInfo.TeamScores[callerTeam] += 1; // Made it
                Console.WriteLine($"Team {callerTeam + 1} makes it and scores 1 point");
            }
        }
        else
        {
            int opposingTeam = 1 - callerTeam;
            GameInfo.TeamScores[opposingTeam] += 2; // Euchred
            Console.WriteLine($"Euchred! Team {opposingTeam + 1} scores 2 points");
        }

        Console.ReadLine();
    }

    private IPlayer GetPlayerAfterDealer()
    {
        int dealerIndex = Array.IndexOf(GameInfo.Players, GameInfo.Dealer);
        return GameInfo.Players[(dealerIndex + 1) % NUMBER_OF_PLAYERS];
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

    private int GetPlayerTeam(IPlayer player)
    {
        int index = Array.IndexOf(GameInfo.Players, player);
        return index % NUMBER_OF_PLAYERS; // Players 0,2 are team 0; Players 1,3 are team 1
    }

    private bool IsPartner(IPlayer player1, IPlayer player2)
    {
        return GetPlayerTeam(player1) == GetPlayerTeam(player2) && player1 != player2;
    }
}
