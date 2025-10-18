using Euchre.Logic.Enums;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Components;

public class EuchreGame
{
    /// <summary>
    /// Use this constructor when loading a saved game.
    /// </summary>
    public EuchreGame()
    {
        // Load persisted state.

        GameInfo = GameStateManager.LoadGameData()
                     ?? throw new InvalidGameConditionException("No saved game found.");

        // Flag that we are resuming – the UI can react accordingly.

        GameInfo.RestartGame = true;
    }

    /// <summary>
    /// Use this constructor to start a new game.
    /// </summary>
    /// <param name="playerNames">The list of names for the players where the first name is a human.</param>
    /// <exception cref="InvalidNumberOfPlayersException" />
    public EuchreGame(List<AutomatedPlayerAvatar> playerNames)
    {
        if (playerNames.Count != NUMBER_OF_PLAYERS)
        {
            throw new InvalidNumberOfPlayersException();
        }

        GameInfo = new GameStateManager();
        var players = new IPlayer[NUMBER_OF_PLAYERS];

        // Add the human player as the first player.

        players[0] = new HumanPlayer(playerNames[0].PlayerName, 0, 0, playerNames[0].AvatarNumber);

        // Add automated players.

        for (int i = 1; i < NUMBER_OF_PLAYERS; i++)
        {
            players[i] = new AutomatedPlayer(playerNames[i].PlayerName, i % NUMBER_OF_TEAMS, i, GameInfo,
                playerNames[i].AvatarNumber);
        }

        GameInfo.Players = players;
        GameInfo.Dealer = players[0];
        GameInfo.SaveGameData();
    }

    /// <summary>
    /// Stores the game's state, including players, scores, dealer, and current round information.
    /// </summary>
    public GameStateManager? GameInfo { get; private set; }

    /// <summary>
    /// The event that is raised after a player makes a bid during the bidding round.  This event is raised
    /// whether or not the user passes.
    /// </summary>
    public event EventHandler<PlayerBidEventArgs>? PlayerBidResult;

    /// <summary>
    /// Fired to inform listeners that a dealer has been chosen.
    /// </summary>
    public event EventHandler<DeclareDealerEventArgs>? DeclareDealer;

    /// <summary>
    /// Fired to inform listeners that some cards have been dealt to a specific player.
    /// </summary>
    public event EventHandler<CardsDealtToPlayerEventArgs>? CardsDealtToPlayer;

    /// <summary>
    /// Fired to inform listeners of the card that appears on top of the kitty.
    /// </summary>
    public event EventHandler<DeclareKittyCardEventArgs>? DeclareKittyCard;

    /// <summary>
    /// Fired to inform listeners that the kitty card was not made trump.
    /// </summary>
    public event EventHandler<System.EventArgs>? KittyWasTurnedDown;

    /// <summary>
    /// Fired to inform listeners that a player has called a suit as trump.
    /// </summary>
    public event EventHandler<System.EventArgs>? TrumpCalled;

    /// <summary>
    /// Fired to inform listeners that all players passed for the second round of bidding.
    /// </summary>
    public event EventHandler<System.EventArgs>? NoTrumpCalled;

    /// <summary>
    /// Fired when the player has played a card.
    /// </summary>
    public event EventHandler<CardPlayedByPlayerEventArgs>? CardPlayedByPlayer;

    /// <summary>
    /// Fired to indicate which player won the most recent trick.
    /// </summary>
    public event EventHandler<DeclareTrickWinnerEventArgs>? DeclareTrickWinner;

    /// <summary>
    /// Fired to indicate which players or player won the current round.
    /// </summary>
    public event EventHandler<DeclareRoundWinningPlayersEventArgs>? DeclareRoundWinningPlayers;

    /// <summary>
    /// Occurs when the game has ended and final results are available.
    /// </summary>
    /// <remarks>Subscribe to this event to be notified when the game concludes. The event provides details
    /// about the outcome through the <see cref="GameOverEventArgs"/> parameter.
    /// </remarks>
    public event EventHandler<GameOverEventArgs>? GameOver;

    /// <summary>
    /// Asynchronously runs the main game loop until one of the teams reaches the winning score.
    /// </summary>
    /// <remarks>This method is asynchronous to support integration with user interfaces or other 
    /// asynchronous workflows. The game loop continues until either team achieves the required winning 
    /// score.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task completes when the game has
    /// finished.</returns>
    /// <exception cref="InvalidGameConditionException" />
    public async Task PlayGameAsync()
    {
        if (GameInfo == null)
        {
            throw new InvalidGameConditionException("GameInfo must be initialized before starting the game.");
        }

        // The main game loop is synchronous, but this method is asynchronous for UI integration.

        while (GameInfo.TeamScores[0] < WINNING_SCORE && GameInfo.TeamScores[1] < WINNING_SCORE)
        {
            await Task.Run(PlayRound);
        }

        EndGame();
    }

    /// <summary>
    /// Advances the game through the stages of a single round, resuming from the last completed stage as 
    /// needed.
    /// </summary>
    /// <remarks>This method is intended to be called internally to progress the game state. It resumes the
    /// round from the appropriate stage based on the last completed checkpoint, allowing for interrupted 
    /// rounds to continue seamlessly. This method should not be called concurrently from multiple threads.</remarks>
    /// <exception cref="InvalidGameConditionException" />
    private void PlayRound()
    {
        //  Determine where we left off and jump to the next step.
        
        switch (GameInfo!.LastCompletedStage)
        {
            // Fresh round – run everything from the top.

            case RoundStage.None:
                ResetRound();
                DealCards();
                if (PlayersChoseTrump())
                {
                    PlayTricksForRound();
                    ScoreRound();
                    AdvanceDealer();
                }
                break;

            case RoundStage.ResetRoundDone:
                DealCards();
                goto case RoundStage.CardsDealt;

            case RoundStage.CardsDealt:
                if (!PlayersChoseTrump()) return;
                goto case RoundStage.TrumpChosen;

            case RoundStage.TrumpChosen:
                // We may have been stopped in the middle of playing tricks.
                PlayTricksForRound(GameInfo.CurrentTrickNumber);
                goto case RoundStage.TricksPlayed;

            case RoundStage.TricksPlayed:
                ScoreRound();
                goto case RoundStage.Scored;

            case RoundStage.Scored:
                AdvanceDealer();
                break;

            case RoundStage.DealerAdvanced:
                GameInfo.LastCompletedStage = RoundStage.None; // ready for next round
                break;

            default:
                throw new InvalidGameConditionException("Unknown round checkpoint.");
        }
    }

    /// <summary>
    /// Resets the game state to prepare for a new round.
    /// </summary>
    /// <remarks>Call this method at the start of each round to clear round-specific data and initialize the
    /// game for continued play. This method resets trick history, trump information, and other round-related
    /// properties. It also updates the game checkpoint and persists the current game state.</remarks>
    private void ResetRound()
    {
        // Reset for new round.

        GameInfo!.CurrentRoundTricks.Clear();
        GameInfo.Trump = null;
        GameInfo.TrumpCaller = null;
        GameInfo.GoingAlone = false;
        GameInfo.AlonePlayer = null;
        GameInfo.Kitty = null;
        
        // Reset checkpoint for a brand-new round.

        GameInfo.ResetRoundCheckpoint();
        GameInfo.ResetTricksWonByPlayers();
        GameInfo.SaveGameData();
    }

    /// <summary>
    /// Deals the cards to each player in the game.
    /// </summary>
    private void DealCards()
    {
        DeclareDealer?.Invoke(this, new DeclareDealerEventArgs(GameInfo!.Dealer!));
        GameInfo!.Deck.Shuffle();
        
        // Clear hands.

        foreach (var player in GameInfo.Players!)
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
                CardsDealtToPlayer?.Invoke(this, 
                    new CardsDealtToPlayerEventArgs(GameInfo.Players[playerIndex], 1));
            }
        }

        // Set turned up card.

        GameInfo.Kitty = GameInfo.Deck.Deal();
        DeclareKittyCard?.Invoke(this, new DeclareKittyCardEventArgs(GameInfo.Kitty));

        // Record that dealing is done.

        GameInfo.LastCompletedStage = RoundStage.CardsDealt;
        GameInfo.SaveGameData();
    }

    /// <summary>
    /// Prompts the players to choose the trump suit through a bidding process.
    /// </summary>
    /// <returns>True to indicate the players made a bid.  False indicates the round should be re-dealt.</returns>
    private bool PlayersChoseTrump()
    {
        // Round 1 - Bid for Kitty suit.

        if (BiddingRound(1, GameInfo!.Kitty!.Suit))
        {
            TrumpCalled?.Invoke(this, new System.EventArgs());
            GameInfo.LastCompletedStage = RoundStage.TrumpChosen;
            GameInfo.SaveGameData();
            return true;
        }

        KittyWasTurnedDown?.Invoke(this, new System.EventArgs());

        // Second round – bid any other suit.

        if (BiddingRound(2, null))
        {
            TrumpCalled?.Invoke(this, new System.EventArgs());
            GameInfo.LastCompletedStage = RoundStage.TrumpChosen;
            GameInfo.SaveGameData();
            return true;
        }

        // Nobody called trump – round ends, dealer advances.

        NoTrumpCalled?.Invoke(this, new System.EventArgs());
        AdvanceDealer();
        return false;
    }

    /// <summary>
    /// Conducts a bidding round for all players, determining whether any player orders up or calls trump
    /// based on the current round and bidding rules.
    /// </summary>
    /// <remarks>This method iterates through all players in turn order, starting with the player to the left
    /// of the dealer. In the first round, players may order up the forced suit; in the second round, players
    /// may call a trump suit. If a player makes a successful bid, the game state is updated accordingly and 
    /// the method returns immediately. If no player bids, the method returns false.</remarks>
    /// <param name="round">The current bidding round. Use 1 for the first round (order up phase) and 2 for 
    /// the second round (call trump phase).</param>
    /// <param name="forcedSuit">The suit of the Kitty that must be ordered up during the first round, or 
    /// null to indicate the bidding is in the second round where any suit can be called.</param>
    /// <returns>true if a player successfully orders up or calls trump during the round; otherwise, false.</returns>
    private bool BiddingRound(int round, Suit? forcedSuit)
    {
        int startPlayerIndex = (Array.IndexOf(GameInfo!.Players!, GameInfo.Dealer) + 1) % NUMBER_OF_PLAYERS;
        
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            var playerIndex = (startPlayerIndex + i) % NUMBER_OF_PLAYERS;
            var player = GameInfo.Players![playerIndex];
            bool isDealer = player == GameInfo.Dealer;
            
            if (round == 1 && forcedSuit.HasValue)
            {
                if (player.OrderUp(GameInfo.Kitty!, isDealer))
                {
                    SetGameToPlayersBid(forcedSuit.Value, player);

                    // Inform UI of the order up and if the player is going alone.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, true, GameInfo.Trump,
                        player.IsGoingAlone, true));

                    // Dealer picks up kitty.

                    GameInfo.Dealer!.DiscardForKitty(GameInfo.Kitty!);
                    GameInfo.SaveGameData();

                    return true;
                }
                else
                {
                    // Inform UI that the player is passing.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, false, null, false, true));
                }
            }
            else if (round == 2)
            {
                var calledSuit = player.CallTrump(GameInfo.Kitty!);
                if (calledSuit.HasValue)
                {
                    SetGameToPlayersBid(calledSuit.Value, player);

                    // Inform UI of the order up and if the player is going alone.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, true, GameInfo.Trump,
                        player.IsGoingAlone, false));

                    GameInfo.SaveGameData();

                    return true;
                }
                else
                {
                    // Inform UI that the player is passing.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, false, null, false, false));
                }
            }
        }
        
        return false;
    }

    /// <summary>
    /// Sets the current game's trump suit and updates game state based on the specified player's bid.
    /// </summary>
    /// <param name="bidSuit">The suit selected as the trump for the current game.</param>
    /// <param name="player">The player who made the bid. The player's properties determine whether they are
    /// going alone and update related game state.</param>
    private void SetGameToPlayersBid(Suit bidSuit, IPlayer player)
    {
        GameInfo!.Trump = bidSuit;
        GameInfo.TrumpCaller = player;
        GameInfo.GoingAlone = player.IsGoingAlone;
        GameInfo.AlonePlayer = player.IsGoingAlone ? player : null;
    }

    /// <summary>
    /// Plays all tricks for the current round.
    /// </summary>
    /// <param name="resumeFrom">
    /// If > 0, we start at that trick number (1-based) – used when resuming after a crash.
    /// </param>
    private void PlayTricksForRound(int resumeFrom = 0)
    {
        // Determine who leads the first trick.

        GameInfo!.NextTrickPlayer = GetNextPlayer(GameInfo.Dealer!);

        // If we are resuming, fast-forward the trick counter and the leader.

        if (resumeFrom > 0)
        {
            GameInfo.NextTrickPlayer = GameInfo.CurrentRoundTricks.Last().GetWinner();
        }

        for (int trickNum = resumeFrom + 1; trickNum <= MAX_NUMBER_OF_TRICKS; trickNum++)
        {
            var trick = PlayTrick(trickNum);
            GameInfo.CurrentRoundTricks.Add(trick);
            GameInfo.NextTrickPlayer = trick.GetWinner();
            GameInfo.TricksWonByPlayers[GameInfo.NextTrickPlayer]++;
            DeclareTrickWinner?.Invoke(this, new DeclareTrickWinnerEventArgs(GameInfo.NextTrickPlayer));

            // Update checkpoint after each trick – this allows us to resume mid-round.

            GameInfo.CurrentTrickNumber = trickNum; // remember where we stopped
            GameInfo.SaveGameData();
        }

        // All tricks done – reset the per-round trick counter.

        GameInfo.CurrentTrickNumber = 0;
        GameInfo.LastCompletedStage = RoundStage.TricksPlayed;
        GameInfo.SaveGameData();
    }

    /// <summary>
    /// Plays a single trick in the current game round, allowing each eligible player to select and play a 
    /// card in turn.
    /// </summary>
    /// <remarks>If a player is going alone, their partner is skipped during the trick. The method updates 
    /// the game state to reflect the cards played and the next player to act.</remarks>
    /// <param name="trickNumber">The zero-based index of the trick within the current round. Used to track 
    /// the sequence of tricks played.</param>
    /// <returns>A Trick object representing the completed trick, including all cards played and the order in
    /// which they were played.</returns>
    private Trick PlayTrick(int trickNumber)
    {
        var trick = new Trick(GameInfo!.Trump!.Value);
        
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            // Skip partner if going alone.

            if (GameInfo.AlonePlayer != null && IsPartner(GameInfo.AlonePlayer, GameInfo.NextTrickPlayer!))
            {
                GameInfo.NextTrickPlayer = GetNextPlayer(GameInfo.NextTrickPlayer!);
                continue;
            }

            Suit? leadSuit = trick.Cards.Count > 0 ? trick.LeadSuit : null;
            
            var playedCard = GameInfo.NextTrickPlayer!.SelectCardToPlay(
                trick, 
                GameInfo.Trump.Value, 
                leadSuit);
            
            // Find the card in hand and play it.

            var cardIndex = GameInfo.NextTrickPlayer.Hand.IndexOf(playedCard);
            playedCard = GameInfo.NextTrickPlayer.PlayCard(cardIndex);
            
            trick.AddCard(GameInfo.NextTrickPlayer, playedCard);
            CardPlayedByPlayer?.Invoke(this, 
                new CardPlayedByPlayerEventArgs(GameInfo.NextTrickPlayer, playedCard));

            GameInfo.NextTrickPlayer = GetNextPlayer(GameInfo.NextTrickPlayer);
        }
        
        return trick;
    }

    /// <summary>
    /// Determines which team won the round and updates their score accordingly.
    /// </summary>
    private void ScoreRound()
    {
        int team0Tricks = 0;
        int team1Tricks = 0;
        
        foreach (var trick in GameInfo!.CurrentRoundTricks)
        {
            var winner = trick.GetWinner();
            if (winner.TeamIndex == 0)
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
        int winningTeamIndex = callerTeam;

        if (callerTricks >= 3)
        {
            if (callerTricks == MAX_NUMBER_OF_TRICKS)
            {
                GameInfo.TeamScores[callerTeam] += GameInfo.GoingAlone ? 4 : 2;
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
            winningTeamIndex = opposingTeam;
        }

        DeclareRoundWinningPlayers?.Invoke(this, 
            new DeclareRoundWinningPlayersEventArgs(
                  from player in GameInfo.Players
                 where player.TeamIndex == winningTeamIndex
                select player.Name));

        // Record that scoring is done.

        GameInfo.LastCompletedStage = RoundStage.Scored;
        GameInfo.SaveGameData();
    }

    /// <summary>
    /// Ends the current game session and performs necessary cleanup operations.
    /// </summary>
    /// <remarks>This method notifies subscribers that the game has ended and clears any saved game state
    /// data. It should be called when the game is over to ensure proper resource management and state
    /// consistency.</remarks>
    private void EndGame()
    {
        // Let the UI know the game is over.

        GameOver?.Invoke(this, new GameOverEventArgs(GameInfo!));

        // Game over – delete saved state data.

        GameStateManager.ClearSavedGameData();
    }

    /// <summary>
    /// Returns the next player in the list of players according to a specified player. 
    /// </summary>
    /// <param name="current">The player to find the next player for.</param>
    /// <returns>The next player in the list.  If the current player is the last player, returns the first
    /// player in the list.</returns>
    private IPlayer GetNextPlayer(IPlayer current)
    {
        return GameInfo!.Players![(current.PlayerIndex + 1) % NUMBER_OF_PLAYERS];
    }

    /// <summary>
    /// Sets the next player after the current dealer as the dealer in the game state manager.
    /// </summary>
    private void AdvanceDealer()
    {
        GameInfo!.Dealer = GetNextPlayer(GameInfo.Dealer!);
        GameInfo.LastCompletedStage = RoundStage.DealerAdvanced;
        GameInfo.SaveGameData();
        GameInfo.LastCompletedStage = RoundStage.None; // ready for next round
    }

    /// <summary>
    /// Returns true if two players are partners.
    /// </summary>
    /// <param name="player1">The first player to check.</param>
    /// <param name="player2">The second player to check.</param>
    /// <returns>True to indicate the two players are on the same team.</returns>
    private bool IsPartner(IPlayer player1, IPlayer player2)
    {
        return player1.TeamIndex == player2.TeamIndex && player1 != player2;
    }
}
