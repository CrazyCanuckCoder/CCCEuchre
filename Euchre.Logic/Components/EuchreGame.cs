using Euchre.Logic.Enums;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using log4net;
using log4net.Config;
using System.IO;
using System.Runtime.CompilerServices;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Components;

public class EuchreGame : IEuchreGame
{
    /// <summary>
    /// Logger for this class.
    /// </summary>
    private static readonly ILog Log = LogManager.GetLogger(typeof(EuchreGame));

    /// <summary>
    /// Static constructor to configure logging once per app domain using external config
    /// </summary>
    static EuchreGame()
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
            var configPath = Path.Combine(baseDir, "log4net.config");
            if (File.Exists(configPath))
            {
                XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
                Log.Debug("log4net configured from file: " + configPath);
            }
            else
            {
                // If config not found, fallback to default basic configuration to avoid silent failures.
                // Caller should ensure the file is copied to output.
                Log.Warn("log4net configuration file not found: " + configPath);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to configure log4net: {ex}");
        }
    }

    /// <summary>
    /// Use this constructor when loading a saved game.
    /// </summary>
    public EuchreGame(IGameStateManager gameStateManager)
    {
        Log.Debug("Initializing EuchreGame from saved game.");

        // Load persisted state.

        var tempGameManager = GameStateManager.LoadGameData()
                     ?? throw new InvalidGameConditionException("No saved game found.");
        gameStateManager.CopyFrom(tempGameManager);
        GameStateManager.AddPlayersToGameData(gameStateManager);
        GameInfo = gameStateManager;

        // Flag that we are resuming – the UI can react accordingly.

        GameInfo.RestartGame = true;
    }

    /// <summary>
    /// Use this constructor to start a new game.
    /// </summary>
    /// <param name="playerNames">The list of names for the players where the first name is a human.</param>
    /// <exception cref="InvalidNumberOfPlayersException" />
    public EuchreGame(List<AutomatedPlayerAvatar> playerNames, IGameStateManager gameStateManager)
    {
        Log.Debug("Initializing EuchreGame for a new game.");

        if (playerNames.Count != NUMBER_OF_PLAYERS)
        {
            Log.Error("Invalid number of player names provided to constructor.");
            throw new InvalidNumberOfPlayersException();
        }

        GameInfo = gameStateManager;
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

        Log.Debug("New game initialized and saved initial state.");
    }

    /// <summary>
    /// Stores the game's state, including players, scores, dealer, and current round information.
    /// </summary>
    public IGameStateManager GameInfo { get; private set; }

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
    /// Fired to inform listeners that a player has invoked the No Ace, No Face, No Trump rule.
    /// </summary>
    public event EventHandler<NoAceNoFaceNoTrumpDeclaredEventArgs>? NoAceNoFaceNoTrumpDeclared;

    /// <summary>
    /// Fired to inform listeners that all players passed for the second round of bidding.
    /// </summary>
    public event EventHandler<System.EventArgs>? NoTrumpCalled;

    /// <summary>
    /// Fired when the player has played a card.
    /// </summary>
    public event EventHandler<CardPlayedByPlayerEventArgs>? CardPlayedByPlayer;

    /// <summary>
    /// Fired when a player can take the remaining tricks of a round.
    /// </summary>
    public event EventHandler<PlayerCanTakeRemainingTricksEventArgs>? PlayerCanTakeRemainingTricks;

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

#if DEBUG

    /// <summary>
    /// Fired to get a response from the user to determine if the cards should be chosen instead of dealt to
    /// them in normal fashion.
    /// </summary>
    public event EventHandler<PromptToChooseCardsForPlayersEventArgs>? PromptToChooseCardsForPlayers;

    /// <summary>
    /// Fired to get the cards for each player chosen by the user.
    /// </summary>
    public event EventHandler<GetPlayersCardsEventArgs>? GetPlayersCards;

    /// <summary>
    /// Fired to alert the UI that the player's hands need to be updated.
    /// </summary>
    public event EventHandler<System.EventArgs>? UpdatePlayersHands;
#endif

    private CancellationTokenSource? _shutdownCts;
    private Task? _gameLoopTask;

    /// <summary>
    /// Request cooperative shutdown of the running game loop.
    /// </summary>
    public void RequestStop()
    {
        try
        {
            Log.Debug("RequestStop called - signalling game loop to stop.");
            _shutdownCts ??= new CancellationTokenSource();
            _shutdownCts.Cancel();
        }
        catch (Exception ex)
        {
            Log.Error("Error while requesting game stop", ex);
        }
    }

    /// <summary>
    /// Run game loop with optional cancellation support.  Replaces previous PlayGameAsync signature.
    /// </summary>
    public async Task PlayGameAsync(CancellationToken externalToken = default)
    {
        Log.Debug("PlayGameAsync started (cancellable).");

        // create linked token that we can cancel locally
        _shutdownCts ??= new CancellationTokenSource();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_shutdownCts.Token, externalToken);
        var ct = linkedCts.Token;

        // store the running task so callers (UI) can await it if needed
        _gameLoopTask = Task.Run(async () =>
        {
            try
            {
                while (!ct.IsCancellationRequested &&
                       GameInfo.TeamScores[0] < WINNING_SCORE &&
                       GameInfo.TeamScores[1] < WINNING_SCORE)
                {
                    // run a synchronous round on thread pool but observe cancellation
                    await Task.Run(() => PlayRound(), ct).ConfigureAwait(false);

                    // small cooperative check point
                    if (ct.IsCancellationRequested) break;
                }
            }
            catch (OperationCanceledException)
            {
                Log.Debug("PlayGameAsync cancelled via token.");
            }
            catch (Exception ex)
            {
                Log.Error("Unhandled exception in PlayGameAsync loop.", ex);
                throw;
            }
        }, ct);

        try
        {
            await _gameLoopTask.ConfigureAwait(false);
        }
        finally
        {
            // ensure state saved on graceful stop
            try
            {
                GameInfo.SaveGameData();
                Log.Debug("Game state saved after PlayGameAsync exit.");
            }
            catch (Exception ex)
            {
                Log.Error("Failed to save game state during shutdown.", ex);
            }
        }

        // call EndGame if finished normally
        if (GameInfo.TeamScores[0] >= WINNING_SCORE || GameInfo.TeamScores[1] >= WINNING_SCORE)
        {
            EndGame();
        }
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
        Log.Debug($"PlayRound starting from checkpoint: {GameInfo.LastCompletedStage}");

        //  Determine where we left off and jump to the next step.

        switch (GameInfo.LastCompletedStage)
        {
            // Fresh round – run everything from the top.

            case RoundStage.None:
                ResetRound();
#if DEBUG
                if (!CardsChosenForUsers())
                {
#endif
                    DealCards();
#if DEBUG
                }
#endif
                if (PlayersChoseTrump())
                {
                    PlayTricksForRound();
                    ScoreRound();
                    AdvanceDealer();
                }
                break;

            case RoundStage.ResetRoundDone:
#if DEBUG
                if (!CardsChosenForUsers())
                {
#endif
                    DealCards();
#if DEBUG
                }
#endif
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
                Log.Error("Unknown round checkpoint.");
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
        Log.Debug("ResetRound: clearing round state.");

        // Reset for new round.

        GameInfo.CurrentRoundTricks.Clear();
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
        Log.Debug("Dealing cards.");
        DeclareDealer?.Invoke(this, new DeclareDealerEventArgs(GameInfo.Dealer!));
        GameInfo.Deck.Shuffle();

        // Clear hands.

        foreach (var player in GameInfo.Players!)
        {
            player.ClearHand();
        }

        // Deal 5 cards to each player.

        DealToPlayers();

        foreach (var player in GameInfo.Players!)
        {
            Log.Debug($"Player {player.PlayerIndex + 1}'s Hand: {player.Hand.PrettyPrint()}");
        }

        // Set turned up card.

        SetKittyCard(GameInfo.Deck.Deal());

        // Record that dealing is done.

        GameInfo.LastCompletedStage = RoundStage.CardsDealt;
        GameInfo.SaveGameData();
    }

    /// <summary>
    /// Deals the cards to each player in the game.
    /// </summary>
    private void DealToPlayers()
    {
        for (int round = 0; round < CARDS_PER_PLAYER; round++)
        {
            int currentPlayerIndex = GetNextPlayer(GameInfo.Dealer!).PlayerIndex;
            for (int playerCount = 0; playerCount < NUMBER_OF_PLAYERS; playerCount++)
            {
                GameInfo.Players![currentPlayerIndex].AddCard(GameInfo.Deck.Deal());
                CardsDealtToPlayer?.Invoke(this,
                    new CardsDealtToPlayerEventArgs(GameInfo.Players[currentPlayerIndex], 1));
                currentPlayerIndex = GetNextPlayer(GameInfo.Players[currentPlayerIndex]).PlayerIndex;
            }
        }
    }

    /// <summary>
    /// Prompts the players to choose the trump suit through a bidding process.
    /// </summary>
    /// <returns>True to indicate the players made a bid.  False indicates the round should be re-dealt.</returns>
    private bool PlayersChoseTrump()
    {
        Log.Debug("PlayersChoseTrump: starting bidding rounds.");

        // Round 1 - Bid for Kitty suit.

        if (BiddingRound(1, GameInfo.Kitty!.Suit))
        {
            Log.Debug(
                $"Trump chosen in round 1: {GameInfo.Trump}{(GameInfo.GoingAlone ? " Alone" : "")} by {GameInfo.TrumpCaller?.Name}");
            if (StopIfNoAceNoFaceNoTrump())
            {
                AdvanceDealer();
                Log.Debug("A player declared No Ace, No Face, No Trump, round cancelled.");
                return false;
            }
            return TrumpWasCalled();
        }

        KittyWasTurnedDown?.Invoke(this, new System.EventArgs());

        // Second round – bid any other suit.

        if (BiddingRound(2, null))
        {
            Log.Debug(
                $"Trump chosen in round 2: {GameInfo.Trump}{(GameInfo.GoingAlone ? " Alone" : "")} by {GameInfo.TrumpCaller?.Name}");
            if (StopIfNoAceNoFaceNoTrump())
            {
                AdvanceDealer();
                Log.Debug("A player declared No Ace, No Face, No Trump, round cancelled.");
                return false;
            }
            return TrumpWasCalled();
        }

        // Nobody called trump – round ends, dealer advances.

        NoTrumpCalled?.Invoke(this, new System.EventArgs());
        AdvanceDealer();
        Log.Debug("No trump called in either round; advancing dealer.");
        return false;
    }

    /// <summary>
    /// Checks each player's hand to see if they meet the conditions for the No Ace, No Face, No Trump rule.
    /// </summary>
    /// <returns>Returns true to indicate a player's hand meets the rule's condition.</returns>
    private bool StopIfNoAceNoFaceNoTrump()
    {
        bool forceStop = false;

        foreach (var player in GameInfo.Players!)
        {
            forceStop = player.HasNoAceNoFaceNoTrump(GameInfo.Trump!.Value);
            if (forceStop)
            {
                NoAceNoFaceNoTrumpDeclared?.Invoke(this, new NoAceNoFaceNoTrumpDeclaredEventArgs(player));
                break; 
            }
        }

        return forceStop;
    }

    /// <summary>
    /// Inform the UI that trump was called and update game state accordingly.
    /// </summary>
    /// <returns>True to indicate that trump was called by a player.</returns>
    private bool TrumpWasCalled()
    {
        TrumpCalled?.Invoke(this, new System.EventArgs());
        GameInfo.LastCompletedStage = RoundStage.TrumpChosen;
        GameInfo.SaveGameData();
        return true;
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
        var player = GameInfo.Dealer!;
        bool isRound1 = round == 1 && forcedSuit.HasValue;

        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            player = GetNextPlayer(player);
            bool isDealer = player == GameInfo.Dealer;

            if (isRound1)
            {
                if (player.OrderUp(GameInfo.Kitty!, isDealer))
                {
                    SetGameToPlayersBid(forcedSuit!.Value, player);

                    // Inform UI of the order up and if the player is going alone.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, true, GameInfo.Trump,
                        player.IsGoingAlone, true));

                    // Dealer picks up kitty unless their partner went alone.

                    if (!GameInfo.TrumpCaller!.IsGoingAlone ||
                        (GameInfo.TrumpCaller!.IsGoingAlone &&
                        !IsPartner(GameInfo.TrumpCaller, GameInfo.Dealer!)))
                    {
                        GameInfo.Dealer!.DiscardForKitty(GameInfo.Kitty!);
                    }
                    GameInfo.SaveGameData();

                    Log.Debug(
                        $"Player {player.Name} ordered up {forcedSuit} (IsDealer={isDealer}, GoingAlone={player.IsGoingAlone})");
                    return true;
                }
                else
                {
                    // Inform UI that the player is passing.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, false, null, false, true));
                    Log.Debug($"Player {player.Name} passed on ordering up.");
                }
            }
            else
            {
                var calledSuit = player.CallTrump(GameInfo.Kitty!, isDealer);
                if (calledSuit.HasValue)
                {
                    SetGameToPlayersBid(calledSuit.Value, player);

                    // Inform UI of the order up and if the player is going alone.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, true, GameInfo.Trump,
                        player.IsGoingAlone, false));

                    GameInfo.SaveGameData();

                    Log.Debug(
                        $"Player {player.Name} called trump {calledSuit} (IsDealer={isDealer}, GoingAlone={player.IsGoingAlone})");
                    return true;
                }
                else
                {
                    // Inform UI that the player is passing.

                    PlayerBidResult?.Invoke(this, new PlayerBidEventArgs(player, false, null, false, false));
                    Log.Debug($"Player {player.Name} passed on calling trump.");
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
        Log.Debug($"SetGameToPlayersBid: {bidSuit} by {player.Name} (GoingAlone={player.IsGoingAlone})");
        GameInfo.Trump = bidSuit;
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
        Log.Debug($"PlayTricksForRound starting (resumeFrom={resumeFrom}).");

        // Determine who leads the first trick.

        GameInfo.NextTrickPlayer = GetNextPlayer(GameInfo.Dealer!);

        // If we are resuming, fast-forward the trick counter and the leader.

        if (resumeFrom > 0)
        {
            GameInfo.NextTrickPlayer = GameInfo.CurrentRoundTricks.Last().GetWinner();
        }

        for (int trickNum = resumeFrom + 1; trickNum <= MAX_NUMBER_OF_TRICKS; trickNum++)
        {
            if (trickNum < MAX_NUMBER_OF_TRICKS)
            {
                if (PlayerCanWinRemainingTricks(GameInfo.NextTrickPlayer!))
                {
                    GameInfo.TricksWonByPlayers[GameInfo.NextTrickPlayer.PlayerIndex] +=
                        MAX_NUMBER_OF_TRICKS - (trickNum - 1);
                    Log.Debug(
                        $"{GameInfo.NextTrickPlayer.Name} can take all remaining tricks; raising event.");
                    PlayerCanTakeRemainingTricks?.Invoke(this,
                        new PlayerCanTakeRemainingTricksEventArgs(GameInfo.NextTrickPlayer));
                    break;
                }
            }

            Log.Debug($"Starting Trick {trickNum}.");
            var trick = PlayTrick();
            GameInfo.CurrentRoundTricks.Add(trick);
            GameInfo.NextTrickPlayer = trick.GetWinner();
            GameInfo.TricksWonByPlayers[GameInfo.NextTrickPlayer.PlayerIndex]++;
            DeclareTrickWinner?.Invoke(this, new DeclareTrickWinnerEventArgs(GameInfo.NextTrickPlayer));

            Log.Debug(
                $"Trick {trickNum} won by {GameInfo.NextTrickPlayer.Name} (Team {GameInfo.NextTrickPlayer.TeamIndex}).");

            // Update checkpoint after each trick – this allows us to resume mid-round.

            GameInfo.CurrentTrickNumber = trickNum; // remember where we stopped
            GameInfo.SaveGameData();

            // Check if the bidding team has the minimum they need to win and can't get anymore points.
            //  Or if the opposition team has enough tricks to end the round.

            if (trickNum >= MIN_NUMBER_TRICKS_TO_SCORE && trickNum < MAX_NUMBER_OF_TRICKS && CanEndRound())
            {
                Log.Debug("Early termination condition met for round; stopping trick play.");
                break;
            }
        }

        // All tricks done – reset the per-round trick counter.

        GameInfo.CurrentTrickNumber = 0;
        GameInfo.LastCompletedStage = RoundStage.TricksPlayed;
        GameInfo.SaveGameData();
    }

    /// <summary>
    /// Determines if the specified player can win all the remaining tricks in the round based on their hand.
    /// </summary>
    /// <param name="leadingPlayer">The current player with the lead.</param>
    /// <returns>True to indicate the player can win the remaining tricks.</returns>
    private bool PlayerCanWinRemainingTricks(IPlayer leadingPlayer)
    {
        bool canWinRest = false;

        Log.Debug($"Starting...");
        Log.Debug($"Player: {leadingPlayer.Name} - Player's Hand: {leadingPlayer.Hand.PrettyPrint()}");

        // Does the player has all trump left in their hand.

        if (CardFinder.CountTrump(leadingPlayer.Hand, GameInfo.Trump!.Value) == leadingPlayer.Hand.Count)
        {
            Log.Debug("Player only has trump left.");

            // Is it all the highest trump?

            var highestTrumpCards = CardHelper.CreateHighestTrumpHand(GameInfo.Trump.Value);
            if (CardHelper.CardListsAreEqual(leadingPlayer.Hand, highestTrumpCards.Take(leadingPlayer.Hand.Count).ToList()))
            {
                Log.Debug($"Player's hand has the highest remaining trump: {highestTrumpCards.Take(leadingPlayer.Hand.Count).ToList().PrettyPrint()}");

                // The rest are mine!

                canWinRest = true;
            }
            else if (CardHelper.TrumpCardsAreSequential(leadingPlayer.Hand, GameInfo.Trump.Value))
            {
                // Has all the trump higher than the trump in their hand been played?

                var highestTrumpInHand = CardFinder.GetHighestTrumpCard(leadingPlayer.Hand,
                    GameInfo.Trump.Value);
                var playedTrumpCards = from trick in GameInfo.CurrentRoundTricks
                                       from card in trick.Cards
                                       where card.Value.EffectiveSuit(GameInfo.Trump.Value) == GameInfo.Trump.Value
                                       select card.Value;
                if (CardHelper.AllCardsAreHigherThanTrumpCard(playedTrumpCards, highestTrumpInHand!))
                {
                    Log.Debug($"All cards higher than the player's trump cards have been played.  Player's highest card: {highestTrumpInHand}.  Played cards: {playedTrumpCards.ToList().PrettyPrint()}.");

                    // The rest are mine!

                    canWinRest = true;
                }
                else if (CardHelper.NoMoreTrumpRemaining(GameInfo.CurrentRoundTricks, GameInfo.Trump.Value))
                {
                    Log.Debug("There are no more trump remaining, so the player has the rest of the trump.");

                    // The rest are mine!

                    canWinRest = true;
                }
            }
        }
        else if (CardHelper.PlayerHasTrumpAndAces(leadingPlayer.Hand, GameInfo.Trump!.Value))
        {
            Log.Debug("Player has trump and aces.");

            // Is the trump in the player's hand, the only remaining trump?

            if (CardHelper.NoMoreTrumpRemaining(GameInfo.CurrentRoundTricks, GameInfo.Trump.Value))
            {
                Log.Debug("There are no more trump remaining, so the player has the highest cards remaining.");

                // The rest are mine!

                canWinRest = true;
            }
        }

        Log.Debug($"Returns {canWinRest}.");

        return canWinRest;
    }

    /// <summary>
    /// Determines if conditions are met where one of the team has already achieved their points and cannot
    /// get any more points.
    /// </summary>
    /// <returns>True to indicate the round can be stopped and false if not.</returns>
    private bool CanEndRound()
    {
        bool endRound = false;

        int numBidderTricks = (from player in GameInfo.Players
                               where player.TeamIndex == GameInfo.TrumpCaller!.TeamIndex
                               select GameInfo.TricksWonByPlayers[player.PlayerIndex])
                              .Sum();
        int numOppositionTricks = (from player in GameInfo.Players
                                   where player.TeamIndex != GameInfo.TrumpCaller!.TeamIndex
                                   select GameInfo.TricksWonByPlayers[player.PlayerIndex])
                                  .Sum();
        if (numBidderTricks == MIN_NUMBER_TRICKS_TO_SCORE)
        {
            endRound = numOppositionTricks > 0;
        }
        else
        {
            endRound = numOppositionTricks == MIN_NUMBER_TRICKS_TO_SCORE;
        }

        Log.Debug(
            $"CanEndRound check: bidderTricks={numBidderTricks}, oppositionTricks={numOppositionTricks}, result={endRound}");
        return endRound;
    }

    /// <summary>
    /// Plays a single trick in the current game round, allowing each eligible player to select and play a 
    /// card in turn.
    /// </summary>
    /// <remarks>If a player is going alone, their partner is skipped during the trick. The method updates 
    /// the game state to reflect the cards played and the next player to act.</remarks>
    /// <returns>A Trick object representing the completed trick, including all cards played and the order in
    /// which they were played.</returns>
    private Trick PlayTrick()
    {
        var trick = new Trick(GameInfo.Trump!.Value);

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

            Log.Debug($"{GameInfo.NextTrickPlayer.Name} played {playedCard} (TrickLead={leadSuit}).");

            GameInfo.NextTrickPlayer = GetNextPlayer(GameInfo.NextTrickPlayer);
        }

        return trick;
    }

    /// <summary>
    /// Determines which team won the round and updates their score accordingly.
    /// </summary>
    private void ScoreRound()
    {
        Log.Debug("Scoring round.");

        // Tally tricks by team through the number of tricks won by each player.

        var tricksByTeam = new Dictionary<int, int>();
        foreach (var player in GameInfo.Players!)
        {
            int teamIndex = player.TeamIndex;
            int tricksWon = GameInfo.TricksWonByPlayers[player.PlayerIndex];
            if (tricksByTeam.ContainsKey(teamIndex))
            {
                tricksByTeam[teamIndex] += tricksWon;
            }
            else
            {
                tricksByTeam[teamIndex] = tricksWon;
            }
        }

        int numTeam0Tricks = tricksByTeam.GetValueOrDefault(0, 0);
        int numTeam1Tricks = tricksByTeam.GetValueOrDefault(1, 0);

        int callerTeamIndex = GameInfo.TrumpCaller?.TeamIndex ?? -1;
        int numCallerTricks = callerTeamIndex == 0 ? numTeam0Tricks : numTeam1Tricks;
        int winningTeamIndex = callerTeamIndex;

        ScoringReason reasonForPoints = ScoringReason.WonHand;
        int numPoints = NUM_POINTS_FOR_WIN;

        if (numCallerTricks >= MIN_NUMBER_TRICKS_TO_SCORE)
        {
            if (numCallerTricks == MAX_NUMBER_OF_TRICKS)
            {
                reasonForPoints = GameInfo.GoingAlone
                    ? ScoringReason.GotAllTricksAlone
                    : ScoringReason.GotAllTricks;
                numPoints = GameInfo.GoingAlone
                    ? NUM_POINTS_FOR_ALL_TRICKS_GOING_ALONE
                    : NUM_POINTS_FOR_ALL_TRICKS;
            }
            GameInfo.TeamScores[callerTeamIndex] += numPoints;
        }
        else
        {
            int opposingTeamIndex = callerTeamIndex ^ 1;
            numPoints = NUM_POINTS_FOR_EUCHRE;
            GameInfo.TeamScores[opposingTeamIndex] += numPoints;
            winningTeamIndex = opposingTeamIndex;
            reasonForPoints = ScoringReason.Euchred;
        }

        DeclareRoundWinningPlayers?.Invoke(this,
            new DeclareRoundWinningPlayersEventArgs(
                  from player in GameInfo.Players
                  where player.TeamIndex == winningTeamIndex
                  select player.Name,
                  numPoints,
                  reasonForPoints));

        Log.Debug(
            $"Round scored. WinningTeam={winningTeamIndex}, Points={numPoints}, Reason={reasonForPoints}." +
            $" Scores: Team0={GameInfo.TeamScores[0]}, Team1={GameInfo.TeamScores[1]}");

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
        Log.Debug("EndGame: game over.");

        // Let the UI know the game is over.

        GameOver?.Invoke(this, new GameOverEventArgs(GameInfo));

        // Game over – delete saved state data.

        GameStateManager.ClearSavedGameData();
    }

    /// <summary>
    /// Sets the kitty card for the current round and raises the appropriate event to inform listeners.
    /// </summary>
    /// <param name="kittyCard">The card to set as the kitty card.</param>
    private void SetKittyCard(Card kittyCard)
    {
        GameInfo.Kitty = kittyCard;
        Log.Debug($"Kitty card for this round: {GameInfo.Kitty}");
        DeclareKittyCard?.Invoke(this, new DeclareKittyCardEventArgs(GameInfo.Kitty));
    }

    /// <summary>
    /// Returns the next player in the list of players according to a specified player. 
    /// </summary>
    /// <param name="current">The player to find the next player for.</param>
    /// <returns>The next player in the list.  If the current player is the last player, returns the first
    /// player in the list.</returns>
    private IPlayer GetNextPlayer(IPlayer current)
    {
        return GameInfo.Players![(current.PlayerIndex + 1) % NUMBER_OF_PLAYERS];
    }

    /// <summary>
    /// Sets the next player after the current dealer as the dealer in the game state manager.
    /// </summary>
    private void AdvanceDealer()
    {
        GameInfo.Dealer = GetNextPlayer(GameInfo.Dealer!);
        GameInfo.LastCompletedStage = RoundStage.None; // ready for next round
        GameInfo.SaveGameData();

        Log.Debug($"AdvanceDealer: new dealer is {GameInfo.Dealer.Name} (index {GameInfo.Dealer.PlayerIndex}).");
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

#if DEBUG

    /// <summary>
    /// Prompts the user whether to choose cards for all the players instead of dealing cards to them.
    /// </summary>
    /// <returns>True to indicate the cards were manually chosen for the players.</returns>
    private bool CardsChosenForUsers()
    {
        // Prompt to see if the cards should be chosen for the players during testing.

        PromptToChooseCardsForPlayersEventArgs e = new();
        PromptToChooseCardsForPlayers?.Invoke(this, e);
        if (e.ChooseCardsForPlayers)
        {
            GetPlayersCardsEventArgs eventArgs = new();
            GetPlayersCards?.Invoke(this, eventArgs);
            if (eventArgs.Player1Cards != null)
            {
                DealChosenCardsToPlayers(eventArgs);
            }
        }

        return e.ChooseCardsForPlayers;
    }


    /// <summary>
    /// Takes the cards chosen for each player and puts them in their hand.
    /// </summary>
    /// <param name="eventArgs">Contains the cards chosen for each player.</param>
    private void DealChosenCardsToPlayers(GetPlayersCardsEventArgs eventArgs)
    {
        // Clear each player's hand.

        foreach (var player in GameInfo.Players!)
        {
            player.ClearHand();
        }

        // Give each player their chosen cards.

        GameInfo.Players![0].ReceiveSeveralCards(eventArgs.Player1Cards);
        GameInfo.Players[1].ReceiveSeveralCards(eventArgs.Player2Cards);
        GameInfo.Players[2].ReceiveSeveralCards(eventArgs.Player3Cards);
        GameInfo.Players[3].ReceiveSeveralCards(eventArgs.Player4Cards);

        UpdatePlayersHands?.Invoke(this, new());

        SetKittyCard(eventArgs.KittyCard);

        GameInfo.LastCompletedStage = RoundStage.CardsDealt;
        GameInfo.SaveGameData();
    }
#endif
}
