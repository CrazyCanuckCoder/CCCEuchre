namespace Euchre.Logic;

public class EuchreGame
{
    public EuchreGame(string[] playerNames)
    {
        if (playerNames.Length != NUMBER_OF_PLAYERS)
            throw new ArgumentException($"Euchre requires exactly {NUMBER_OF_PLAYERS} players");
        
        players = new Player[NUMBER_OF_PLAYERS];
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            players[i] = new Player(playerNames[i], false); // All AI for now
        }
        
        deck = new Deck();
        teamScores = new int[NUMBER_OF_TEAMS];
        currentHandTricks = new List<Trick>();
        dealer = players[0];
    }

    public const int NUMBER_OF_PLAYERS = 4;
    public const int CARDS_PER_PLAYER = 5;
    public const int NUMBER_OF_TEAMS = 2;
    public const int MAX_NUMBER_OF_TRICKS = 5;
    public const int WINNING_SCORE = 10;

    private Player[] players;
    private Deck deck;
    private Card kitty;
    private Suit? trump;
    private Player dealer;
    private Player trumpCaller;
    private int[] teamScores; // [Team 0 (Players 0,2), Team 1 (Players 1,3)]
    private List<Trick> currentHandTricks;
    private bool goingAlone;
    private Player alonePlayer;

    public void PlayGame()
    {
        Console.WriteLine("Starting Euchre Game!");
        PrintTeams();
        
        while (teamScores[0] < WINNING_SCORE && teamScores[1] < WINNING_SCORE)
        {
            PlayHand();
            
            Console.WriteLine($"\nScores - Team 1: {teamScores[0]}, Team 2: {teamScores[1]}");
            Console.WriteLine(new string('=', 50));
            Console.ReadLine();
        }
        
        int winningTeam = teamScores[0] >= WINNING_SCORE ? 0 : 1;
        Console.WriteLine($"\nGame Over! Team {winningTeam + 1} wins!");
        Console.WriteLine($"Final Score - Team 1: {teamScores[0]}, Team 2: {teamScores[1]}");
    }

    private void PrintTeams()
    {
        Console.WriteLine($"Team 1: {players[0].Name} & {players[2].Name}");
        Console.WriteLine($"Team 2: {players[1].Name} & {players[3].Name}");
        Console.ReadLine();
    }

    private void PlayHand()
    {
        // Reset for new hand

        currentHandTricks.Clear();
        trump = null;
        trumpCaller = null;
        goingAlone = false;
        alonePlayer = null;
        
        // Deal cards

        DealCards();
        
        Console.WriteLine($"\nDealer: {dealer.Name}");
        Console.WriteLine($"Kitty: {kitty}");
        Console.ReadLine();

        // Bidding phase

        if (!BiddingPhase())
        {
            Console.WriteLine("No one ordered up. Dealing new hand.");
            AdvanceDealer();
            Console.ReadLine();
            return;
        }
        
        Console.WriteLine($"\nTrump: {trump}");
        Console.WriteLine($"Called by: {trumpCaller.Name}");
        if (goingAlone)
            Console.WriteLine($"{alonePlayer.Name} is going alone!");
        Console.ReadLine();

        // Play 5 tricks
        Player leader = GetPlayerAfterDealer();
        for (int trickNum = 0; trickNum < MAX_NUMBER_OF_TRICKS; trickNum++)
        {
            var trick = PlayTrick(leader, trickNum + 1);
            currentHandTricks.Add(trick);
            leader = trick.GetWinner();
            
            Console.WriteLine($"Trick {trickNum + 1} won by: {leader.Name}");
            Console.ReadLine();
        }

        // Score the hand
        ScoreHand();
        
        // Advance dealer
        AdvanceDealer();
    }

    private void DealCards()
    {
        // TODO: Creating a new deck each time is inefficient, consider reusing the deck.
        deck = new Deck();
        deck.Shuffle();
        
        // Clear hands
        foreach (var player in players)
        {
            player.ClearHand();
        }
        
        // Deal 5 cards to each player (3-2 or 2-3 pattern)
        for (int round = 0; round < 2; round++)
        {
            for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
            {
                int cardsToDeal = (round == 0) ? 3 : 2;
                if (i % 2 == 1) cardsToDeal = CARDS_PER_PLAYER - cardsToDeal; // Alternate pattern
                
                var playerIndex = (Array.IndexOf(players, dealer) + 1 + i) % 4;
                for (int j = 0; j < cardsToDeal; j++)
                {
                    players[playerIndex].AddCard(deck.Deal());
                }
            }
        }
        
        // Set kitty
        kitty = deck.Deal();
    }

    private bool BiddingPhase()
    {
        // Round 1 - Kitty suit
        if (BiddingRound(1, kitty.Suit))
            return true;
        
        // Round 2 - Other suits
        return BiddingRound(2, null);
    }

    private bool BiddingRound(int round, Suit? forcedSuit)
    {
        int startPlayerIndex = (Array.IndexOf(players, dealer) + 1) % NUMBER_OF_PLAYERS;
        
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            var playerIndex = (startPlayerIndex + i) % NUMBER_OF_PLAYERS;
            var player = players[playerIndex];
            bool isDealer = player == dealer;
            
            if (round == 1 && forcedSuit.HasValue)
            {
                if (player.OrderUp(kitty, forcedSuit.Value, round, isDealer))
                {
                    trump = forcedSuit.Value;
                    trumpCaller = player;
                    
                    // Dealer picks up kitty
                    dealer.DiscardForKitty(kitty, trump.Value);
                    
                    Console.WriteLine($"{player.Name} ordered up {trump}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"{player.Name} passes");
                }
            }
            else if (round == 2)
            {
                var calledSuit = player.CallTrump(kitty);
                if (calledSuit.HasValue)
                {
                    trump = calledSuit.Value;
                    trumpCaller = player;
                    Console.WriteLine($"{player.Name} calls {trump}");
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

    private Trick PlayTrick(Player leader, int trickNumber)
    {
        var trick = new Trick(trump.Value);
        
        Console.WriteLine($"\n--- Trick {trickNumber} ---");
        Console.WriteLine($"Leader: {leader.Name}");
        
        Player currentPlayer = leader;
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            // Skip partner if going alone
            if (goingAlone && IsPartner(alonePlayer, currentPlayer) && currentPlayer != alonePlayer)
            {
                Console.WriteLine($"{currentPlayer.Name} sits out (partner going alone)");
                currentPlayer = GetNextPlayer(currentPlayer);
                continue;
            }
            
            Suit? leadSuit = trick.Cards.Count > 0 ? trick.LeadSuit : null;
            var validCards = currentPlayer.GetValidCards(leadSuit ?? trump.Value, trump.Value);
            
            Console.WriteLine($"\n{currentPlayer.Name}'s turn");
            Console.WriteLine($"Hand: {string.Join(", ", currentPlayer.Hand.Select((c, idx) => $"{idx}: {c}"))}");
            if (leadSuit.HasValue)
                Console.WriteLine($"Must follow {leadSuit}");
            
            var playedCard = currentPlayer.SelectCardToPlay(
                trick.Cards.Select(c => c.Card).ToList(), 
                trump.Value, 
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

    private void ScoreHand()
    {
        int team0Tricks = 0;
        int team1Tricks = 0;
        
        foreach (var trick in currentHandTricks)
        {
            var winner = trick.GetWinner();
            int team = GetPlayerTeam(winner);
            if (team == 0) team0Tricks++;
            else team1Tricks++;
        }
        
        int callerTeam = GetPlayerTeam(trumpCaller);
        int callerTricks = callerTeam == 0 ? team0Tricks : team1Tricks;
        
        Console.WriteLine($"\nTricks won - Team 1: {team0Tricks}, Team 2: {team1Tricks}");
        
        if (callerTricks >= 3)
        {
            if (callerTricks == MAX_NUMBER_OF_TRICKS)
            {
                if (goingAlone)
                {
                    teamScores[callerTeam] += 4; // Lone march
                    Console.WriteLine($"Lone march! Team {callerTeam + 1} scores 4 points");
                }
                else
                {
                    teamScores[callerTeam] += 2; // March
                    Console.WriteLine($"March! Team {callerTeam + 1} scores 2 points");
                }
            }
            else
            {
                teamScores[callerTeam] += 1; // Made it
                Console.WriteLine($"Team {callerTeam + 1} makes it and scores 1 point");
            }
        }
        else
        {
            int opposingTeam = 1 - callerTeam;
            teamScores[opposingTeam] += 2; // Euchred
            Console.WriteLine($"Euchred! Team {opposingTeam + 1} scores 2 points");
        }

        Console.ReadLine();
    }

    private Player GetPlayerAfterDealer()
    {
        int dealerIndex = Array.IndexOf(players, dealer);
        return players[(dealerIndex + 1) % NUMBER_OF_PLAYERS];
    }

    private Player GetNextPlayer(Player current)
    {
        int currentIndex = Array.IndexOf(players, current);
        return players[(currentIndex + 1) % NUMBER_OF_PLAYERS];
    }

    private void AdvanceDealer()
    {
        dealer = GetNextPlayer(dealer);
    }

    private int GetPlayerTeam(Player player)
    {
        int index = Array.IndexOf(players, player);
        return index % NUMBER_OF_PLAYERS; // Players 0,2 are team 0; Players 1,3 are team 1
    }

    private bool IsPartner(Player player1, Player player2)
    {
        return GetPlayerTeam(player1) == GetPlayerTeam(player2) && player1 != player2;
    }
}
