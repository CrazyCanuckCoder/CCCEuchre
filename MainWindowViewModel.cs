using CrazyCanuckCoder.Library.Common;
using Euchre.Logic.Components;
using Euchre.Logic.Enums;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using Euchre.Windows;
using System.Windows;
using Extensions = Euchre.Logic.Helpers.Extensions;

namespace Euchre;

public class MainWindowViewModel : DependencyObject
{
    #region Fields

    /// <summary>
    /// Tracks the player index value for the previous dealer.
    /// </summary>
    private int _previousDealerPlayerIndex = -1;

    /// <summary>
    /// A reference to the main window.
    /// </summary>
    private MainWindow _mainWindow;

    /// <summary>
    /// The class that directly updates the main window.
    /// </summary>
    private MainWindowController _mainWindowController;

    #endregion Fields

    #region Properties

    #region Dependency Properties

    public static readonly DependencyProperty Player3CardDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player3CardDisplayUserControlVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty Player4CardDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player4CardDisplayUserControlVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty Player1DealtCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player1DealtCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player2DealtCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player2DealtCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player3DealtCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player3DealtCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player4DealtCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player4DealtCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player1PlayedCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player1PlayedCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player2PlayedCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player2PlayedCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player3PlayedCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player3PlayedCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player4PlayedCardsDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player4PlayedCardsDisplayUserControlVisibility), 
            typeof(Visibility), typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty GameBoardVisibilityProperty =
        DependencyProperty.Register(nameof(GameBoardVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty GameInformationProperty =
        DependencyProperty.Register(nameof(GameInformation), typeof(string), typeof(MainWindowViewModel), 
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty GameInformationVisibilityProperty =
        DependencyProperty.Register(nameof(GameInformationVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player1TextVisibilityProperty =
        DependencyProperty.Register(nameof(Player1TextVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player2TextVisibilityProperty =
        DependencyProperty.Register(nameof(Player2TextVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player3TextVisibilityProperty =
        DependencyProperty.Register(nameof(Player3TextVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player4TextVisibilityProperty =
        DependencyProperty.Register(nameof(Player4TextVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty Player1TextProperty =
        DependencyProperty.Register(nameof(Player1Text), typeof(string), typeof(MainWindowViewModel), 
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty Player2TextProperty =
        DependencyProperty.Register(nameof(Player2Text), typeof(string), typeof(MainWindowViewModel), 
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty Player3TextProperty =
        DependencyProperty.Register(nameof(Player3Text), typeof(string), typeof(MainWindowViewModel), 
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty Player4TextProperty =
        DependencyProperty.Register(nameof(Player4Text), typeof(string), typeof(MainWindowViewModel), 
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty Player1CardDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player1CardDisplayUserControlVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty Player2CardDisplayUserControlVisibilityProperty =
        DependencyProperty.Register(nameof(Player2CardDisplayUserControlVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty ContinueMenuVisibilityProperty =
        DependencyProperty.Register(nameof(ContinueMenuVisibility), typeof(Visibility),
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty StandardMenuVisibilityProperty =
        DependencyProperty.Register(nameof(StandardMenuVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Visible));

    public static readonly DependencyProperty IconMenuVisibilityProperty =
        DependencyProperty.Register(nameof(IconMenuVisibility), typeof(Visibility), 
            typeof(MainWindowViewModel), new PropertyMetadata(Visibility.Collapsed));

    public static readonly DependencyProperty PlayLastCardInHandProperty =
        DependencyProperty.Register(nameof(PlayLastCardInHand), typeof(bool), typeof(MainWindowViewModel), 
            new PropertyMetadata(false));

    #endregion Dependency Properties

    // Properties
    public Visibility Player3CardDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player3CardDisplayUserControlVisibilityProperty);
        set => SetValue(Player3CardDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player4CardDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player4CardDisplayUserControlVisibilityProperty);
        set => SetValue(Player4CardDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player1DealtCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player1DealtCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player1DealtCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player2DealtCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player2DealtCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player2DealtCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player3DealtCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player3DealtCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player3DealtCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player4DealtCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player4DealtCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player4DealtCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player1PlayedCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player1PlayedCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player1PlayedCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player2PlayedCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player2PlayedCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player2PlayedCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player3PlayedCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player3PlayedCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player3PlayedCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player4PlayedCardsDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player4PlayedCardsDisplayUserControlVisibilityProperty);
        set => SetValue(Player4PlayedCardsDisplayUserControlVisibilityProperty, value);
    }

    public Visibility GameBoardVisibility
    {
        get => (Visibility)GetValue(GameBoardVisibilityProperty);
        set => SetValue(GameBoardVisibilityProperty, value);
    }

    public string GameInformation
    {
        get => (string)GetValue(GameInformationProperty);
        set => SetValue(GameInformationProperty, value);
    }

    public Visibility GameInformationVisibility
    {
        get => (Visibility)GetValue(GameInformationVisibilityProperty);
        set => SetValue(GameInformationVisibilityProperty, value);
    }

    public Visibility Player1TextVisibility
    {
        get => (Visibility)GetValue(Player1TextVisibilityProperty);
        set => SetValue(Player1TextVisibilityProperty, value);
    }

    public Visibility Player2TextVisibility
    {
        get => (Visibility)GetValue(Player2TextVisibilityProperty);
        set => SetValue(Player2TextVisibilityProperty, value);
    }

    public Visibility Player3TextVisibility
    {
        get => (Visibility)GetValue(Player3TextVisibilityProperty);
        set => SetValue(Player3TextVisibilityProperty, value);
    }

    public Visibility Player4TextVisibility
    {
        get => (Visibility)GetValue(Player4TextVisibilityProperty);
        set => SetValue(Player4TextVisibilityProperty, value);
    }

    public string Player1Text
    {
        get => (string)GetValue(Player1TextProperty);
        set => SetValue(Player1TextProperty, value);
    }

    public string Player2Text
    {
        get => (string)GetValue(Player2TextProperty);
        set => SetValue(Player2TextProperty, value);
    }

    public string Player3Text
    {
        get => (string)GetValue(Player3TextProperty);
        set => SetValue(Player3TextProperty, value);
    }

    public string Player4Text
    {
        get => (string)GetValue(Player4TextProperty);
        set => SetValue(Player4TextProperty, value);
    }

    public Visibility Player1CardDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player1CardDisplayUserControlVisibilityProperty);
        set => SetValue(Player1CardDisplayUserControlVisibilityProperty, value);
    }

    public Visibility Player2CardDisplayUserControlVisibility
    {
        get => (Visibility)GetValue(Player2CardDisplayUserControlVisibilityProperty);
        set => SetValue(Player2CardDisplayUserControlVisibilityProperty, value);
    }

    /// <summary>
    /// Shows/hides the continue game menu option when a game data file exists.
    /// </summary>
    public Visibility ContinueMenuVisibility
    { 
        get => (Visibility)GetValue(ContinueMenuVisibilityProperty);
        set => SetValue(ContinueMenuVisibilityProperty, value);
    }

    /// <summary>
    /// Displays or collapses the regular menu.
    /// </summary>
    public Visibility StandardMenuVisibility
    {
        get => (Visibility)GetValue(StandardMenuVisibilityProperty);
        set => SetValue(StandardMenuVisibilityProperty, value);
    }

    /// <summary>
    /// Displays or collapses the icon menu.
    /// </summary>
    public Visibility IconMenuVisibility
    {
        get => (Visibility)GetValue(IconMenuVisibilityProperty);
        set => SetValue(IconMenuVisibilityProperty, value);
    }

    /// <summary>
    /// True to indicate the last card in a human player's hand can be automatically played.
    /// </summary>
    public bool PlayLastCardInHand
    {
        get => (bool)GetValue(PlayLastCardInHandProperty);
        set => SetValue(PlayLastCardInHandProperty, value);
    }


    /// <summary>
    /// The reference to the object running the game logic.
    /// </summary>
    public EuchreGame? CurrentGame { get; set; }

    #endregion Properties


    /// <summary>
    /// Sets up some of the properties for this view model class.
    /// </summary>
    public void Initialize()
    {
        ContinueMenuVisibility = GameStateManager.DataExists() ? Visibility.Visible : Visibility.Collapsed;
        SetMenuVisibility();
        PlayLastCardInHand = GameSettingsManager.Instance.PlayLastCardInHand;
    }

    /// <summary>
    /// Call this method to initialize the controls for the game on the main window.
    /// </summary>
    /// <param name="mainWindow">A reference to the main window to update.</param>
    public void SetupUserInterface(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        _mainWindowController = new(mainWindow, CurrentGame!.GameInfo);
        _mainWindowController.SetupCurrentRoundInfoControl();
        _mainWindowController.SetupCurrentScoreControl();
        _mainWindowController.InitializeCardDisplayControlCollection();
        SetVisibilityActionsOfPlayerCardDisplayControl();
        ShowGameBoard();
        _mainWindowController.SetupPlayerDisplayControls();
        if (CurrentGame!.GameInfo.RestartGame)
        {
            RestoreGameUI();
        }
    }

    public void DeclareDealer(IPlayer dealer)
    {
        _mainWindowController.SetPlayerDealerIconVisibility(dealer.PlayerIndex, true);
        _mainWindowController.SetPlayerDealerIconVisibility(_previousDealerPlayerIndex, false);

        _previousDealerPlayerIndex = dealer.PlayerIndex;

        UpdateGameInformation($"{dealer.Name} is the dealer.");
    }

    public void DealCardsToPlayer(int indexOfPlayer, int numberOfCardsDealt)
    {
        _mainWindowController.DealCardsToPlayer(indexOfPlayer, numberOfCardsDealt);
        SetPlayerVisibility(indexOfPlayer, Visibility.Visible, Visibility.Collapsed);

        PauseGame(1);

        SetPlayerVisibility(indexOfPlayer, Visibility.Collapsed, Visibility.Collapsed);
        _mainWindowController.ClearDealtCards(indexOfPlayer);

        UpdatePlayersHandAfterDeal(indexOfPlayer, numberOfCardsDealt);
    }

    public void SetKittyCard(Card kitty)
    {
        _mainWindowController.DisplayKittyCard(kitty);
    }

    public void PlayerPassed(IPlayer player, bool isKittyRound)
    {
        string message = "Pass";

        if (isKittyRound && CurrentGame!.GameInfo.Dealer == player)
        {
            message = "Turning down the kitty card.";
        }

        DisplayPlayerMessage(player, message, 1, false);
    }

    public void KittyWasTurnedDown()
    {
        UpdateGameInformation("The kitty card was turned down.");
        _mainWindowController.ClearTrumpDisplay();
        ClearPlayerMessages();
    }

    public void TrumpCalled()
    {
        //foreach (IPlayer player in CurrentGame!.GameInfo.Players!)
        //{
        //    UpdatePlayersHandAfterTrumpSet(player.PlayerIndex, CurrentGame.GameInfo.Trump!.Value);
        //}
    }

    public void NoTrumpWasCalled()
    {
        ClearPlayerMessages();
        _mainWindowController.ResetCardDisplayControlsVisibility();
        _mainWindowController.ClearPlayersHands();
        foreach (IPlayer player in CurrentGame!.GameInfo.Players!)
        {
            SetPlayersPlayedCardDisplayControlVisibility(player.PlayerIndex, Visibility.Collapsed);
        }
    }

    public void PlayerMadeTrump(IPlayer player, Suit? trump, bool isGoingAlone, bool isKittyRound)
    {
        string message = string.Empty;

        if (isKittyRound)
        {
            string goingAlone = isGoingAlone ? " and I am going alone" : "";
            message = CurrentGame!.GameInfo.Dealer == player
                ? $"I am picking up the kitty card{goingAlone}."
                : $"Pick it up{goingAlone}.";
        }
        else
        {
            message = $"{trump!.Value} {(isGoingAlone ? " alone" : "")}";
        }

        DisplayPlayerMessage(player, message, 2, false);
        foreach (IPlayer updatePlayer in CurrentGame!.GameInfo.Players!)
        {
            UpdatePlayersHandAfterTrumpSet(updatePlayer.PlayerIndex, trump!.Value);
        }
        _mainWindowController.SetTrump(trump!.Value);
        _mainWindowController.SetBidInformation(player, trump.Value, isGoingAlone);
        _mainWindowController.SetPlayerTrumpSuitIcon(player.PlayerIndex, trump.Value);
        if (isGoingAlone) 
        {
            _mainWindowController.SetPartnerDisabled(player);
        }
        ClearPlayerMessages();
    }

    public void DisplayCardPlayedByPlayer(IPlayer player, ICard cardPlayed)
    {
        if (player is HumanPlayer)
        {
            _mainWindowController.SetupPlayerCards(player);
        }
        else
        {
#if DEBUG
            _mainWindowController.SetupPlayerCards(player);
#else
            _playerCardDisplayControls[player.PlayerIndex].RemoveCard();
#endif
        }
        SetPlayersPlayedCardDisplayControlVisibility(player.PlayerIndex, Visibility.Visible);
        var cardPlayedClone = cardPlayed.Clone();
        _mainWindowController.DisplayPlayedCard(player.PlayerIndex, cardPlayedClone);

        PauseGame(2);
    }


    public void EndOfTrick(IPlayer trickWinningPlayer)
    {
        _mainWindowController.UpdatePlayersNumberOfTricksWon(trickWinningPlayer.PlayerIndex);
        UpdateGameInformation($"{trickWinningPlayer.Name} won the trick.");

        // Add the trick to the list of previous tricks and clear the played cards from the game board.

        _mainWindowController.AddTrick(CurrentGame!.GameInfo.CurrentRoundTricks.Last());
        _mainWindowController.ClearPlayedCards();
    }

    public void EndOfRoundUpdate(List<string> winningPlayers)
    {
        // Display the winning round message to the user.

        UpdateGameInformation($"{winningPlayers.ToListedString()} won the round.");

        // The round is over, reset the board for the next round.

        _mainWindowController.ResetCardDisplayControlsVisibility();
        _mainWindowController.ClearPlayersHands();
        _mainWindowController.UpdateGameScores();
        _mainWindowController.ResetTrickCounters();
        _mainWindowController.ResetPlayerTrumpSuitIcon(CurrentGame!.GameInfo.TrumpCaller!.PlayerIndex);
        _mainWindowController.ResetTricksTrumpAndBidInformation();
    }

    public void EndOfGameUpdate(GameStateManager gameInfo)
    {
        int winningTeamIndex = gameInfo.TeamScores[0] > gameInfo.TeamScores[1] ? 0 : 1;
        string gameWinners =
            (  from player in CurrentGame!.GameInfo.Players!
              where player.TeamIndex == winningTeamIndex
             select player.Name)
            .ToList()
            .ToListedString();
        UpdateGameInformation($"Game is over! {gameWinners} are the winners!", 5);
        ContinueMenuVisibility = Visibility.Collapsed;
        ResetGameUI();
    }

    public bool PromptUserToOrderUp(Card kitty, out bool goAlone)
    {
        goAlone = false;
        bool orderedUp = false;

        List<Suit> suits = [kitty.Suit];
        PickTrumpSuitWindow pickTrumpSuitWindow = new(suits)
        {
            Owner = _mainWindow
        };
        if (pickTrumpSuitWindow.ShowDialog() == true)
        {
            orderedUp = pickTrumpSuitWindow.SelectedSuit != null;
            goAlone = pickTrumpSuitWindow.GoAlone;
        }

        return orderedUp;
    }

    public Suit? PromptUserForTrump(Card kitty, out bool goAlone)
    {
        goAlone = false;
        Suit? chosenSuit = null;

        List<Suit> suits = Extensions.GetComplementarySuits(kitty.Suit);
        suits.Remove(kitty.Suit);
        PickTrumpSuitWindow pickTrumpSuitWindow = new(suits)
        {
            Owner = _mainWindow
        };
        if (pickTrumpSuitWindow.ShowDialog() == true)
        {
            chosenSuit = pickTrumpSuitWindow.SelectedSuit;
            goAlone = pickTrumpSuitWindow.GoAlone;
        }

        return chosenSuit;
    }

    public Card? PromptUserForDiscard(HumanPlayer player)
    {
        Card? discard = null;

        ChooseDiscardWindow cardWindow = new(player.Hand)
        {
            Owner = _mainWindow,
        };
        if (cardWindow.ShowDialog() == true)
        {
            discard = (Card?)cardWindow.ChosenCard;
        }

        return discard;
    }

    public void RedisplayUserHand(HumanPlayer player)
    {
        _mainWindowController.SetupPlayerCards(player);
    }

    public Card? GetCardFromUser(IPlayer user, Suit? trickSuit, Suit trump)
    {
        // Let the user know they need to play a card.

        DisplayPlayerMessage(user, "It's your turn.", 2, true);

        return _mainWindowController.GetCardFromUser(user, trickSuit);
    }

    /// <summary>
    /// Displays the game board and hides the menus.
    /// </summary>
    private void ShowGameBoard()
    {
        GameBoardVisibility = Visibility.Visible;
        StandardMenuVisibility = Visibility.Collapsed;
        IconMenuVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Pauses the game for a specified number of seconds to update the UI.
    /// </summary>
    /// <param name="numSeconds">The number of seconds to pause.</param>
    private static void PauseGame(int numSeconds)
    {
        var waitTime = TimeSpan.FromSeconds(numSeconds);
        DateTime start = DateTime.Now;

        while (DateTime.Now - start <= waitTime)
        {
            UIHelpers.AllowUIToUpdate();
        }
    }

    /// <summary>
    /// Displays a string in the middle of the game board.
    /// </summary>
    /// <param name="message">The text to display on the game board.</param>
    /// <param name="pauseSeconds">The number of seconds to pause the game, default is 2 seconds.</param>
    private void UpdateGameInformation(string message, int pauseSeconds = 2)
    {
        GameInformation = message;
        GameInformationVisibility = Visibility.Visible;

        // Wait for the specified number of seconds.

        PauseGame(pauseSeconds);

        // Hide the text.

        GameInformation = string.Empty;
        GameInformationVisibility = Visibility.Hidden;
    }

    /// <summary>
    /// Sets the visibility of the dealt and played cards for a specified player.
    /// </summary>
    /// <param name="playerIndex">The index of the player in the list of players.</param>
    /// <param name="dealtVisibility">The visibility setting to use for the player's dealt cards.</param>
    /// <param name="playedVisibility">The visibility setting to use for the player's played cards.</param>
    private void SetPlayerVisibility(int playerIndex, Visibility dealtVisibility, Visibility playedVisibility)
    {
        switch (playerIndex)
        {
            case 0:
                Player1DealtCardsDisplayUserControlVisibility = dealtVisibility;
                Player1PlayedCardsDisplayUserControlVisibility = playedVisibility;
                break;
            case 1:
                Player2DealtCardsDisplayUserControlVisibility = dealtVisibility;
                Player2PlayedCardsDisplayUserControlVisibility = playedVisibility;
                break;
            case 2:
                Player3DealtCardsDisplayUserControlVisibility = dealtVisibility;
                Player3PlayedCardsDisplayUserControlVisibility = playedVisibility;
                break;
            case 3:
                Player4DealtCardsDisplayUserControlVisibility = dealtVisibility;
                Player4PlayedCardsDisplayUserControlVisibility = playedVisibility;
                break;
        }
    }

    /// <summary>
    /// Displays the cards that have been dealt to a specified player.
    /// </summary>
    /// <param name="indexOfPlayer">The index of the player from the GameStateManager's Players list.</param>
    /// <param name="numberOfCardsDealt">The number of cards dealt to an automated player.</param>
    private void UpdatePlayersHandAfterDeal(int indexOfPlayer, int numberOfCardsDealt)
    {
        if (CurrentGame!.GameInfo.Players![indexOfPlayer] is HumanPlayer humanPlayer)
        {
            humanPlayer.SortPlayerCards(trump: null);
            _mainWindowController.SetupPlayerCards(humanPlayer);
        }
        else
        {
#if DEBUG
            if (CurrentGame.GameInfo.Players[indexOfPlayer] is AutomatedPlayer automatedPlayer)
            {
                automatedPlayer.SortPlayerCards(trump: null);
                _mainWindowController.SetupPlayerCards(automatedPlayer);
            }
#else
            _playerCardDisplayControls[indexOfPlayer].AddCards(numberOfCardsDealt);
#endif
        }
    }

    /// <summary>
    /// Resorts a specified player's hand based on a trump suit, then displays the sorted hand on the UI.
    /// </summary>
    /// <param name="indexOfPlayer">The index of the player from the GameStateManager's Players list.</param>
    /// <param name="trump">The suit designated as the trump suit.</param>
    private void UpdatePlayersHandAfterTrumpSet(int indexOfPlayer, Suit trump)
    {
        if (CurrentGame!.GameInfo.Players![indexOfPlayer] is HumanPlayer humanPlayer)
        {
            humanPlayer.SortPlayerCards(trump);
            _mainWindowController.SetupPlayerCards(humanPlayer);
        }
        else
        {
#if DEBUG
            if (CurrentGame.GameInfo.Players[indexOfPlayer] is AutomatedPlayer automatedPlayer)
            {
                automatedPlayer.SortPlayerCards(trump);
                _mainWindowController.SetupPlayerCards(automatedPlayer);
            }
#endif
        }
    }

    /// <summary>
    /// Displays a specified message in front of the player for a specified number of seconds.
    /// </summary>
    /// <param name="player">The player who displays the message.</param>
    /// <param name="message">The text to display for the player.</param>
    /// <param name="pauseLength">The number of seconds to pause after the message is displayed.</param>
    /// <param name="hideText">True to hide the text after the message is displayed.</param>
    private void DisplayPlayerMessage(IPlayer player, string message, int pauseLength, bool hideText)
    {
        switch (player.PlayerIndex)
        {
            case 0:
                Player1Text = message;
                Player1TextVisibility = Visibility.Visible;
                PauseGame(pauseLength);
                if (hideText) Player1TextVisibility = Visibility.Collapsed;
                break;

            case 1:
                Player2Text = message;
                Player2TextVisibility = Visibility.Visible;
                PauseGame(pauseLength);
                if (hideText) Player2TextVisibility = Visibility.Collapsed;
                break;

            case 2:
                Player3Text = message;
                Player3TextVisibility = Visibility.Visible;
                PauseGame(pauseLength);
                if (hideText) Player3TextVisibility = Visibility.Collapsed;
                break;

            case 3:
                Player4Text = message;
                Player4TextVisibility = Visibility.Visible;
                PauseGame(pauseLength);
                if (hideText) Player4TextVisibility = Visibility.Collapsed;
                break;
        }
    }

    /// <summary>
    /// Resets each player's message text to blank and hides the control.
    /// </summary>
    private void ClearPlayerMessages()
    {
        Player1Text = string.Empty;
        Player1TextVisibility = Visibility.Collapsed;
        Player2Text = string.Empty;
        Player2TextVisibility = Visibility.Collapsed;
        Player3Text = string.Empty;
        Player3TextVisibility = Visibility.Collapsed;
        Player4Text = string.Empty;
        Player4TextVisibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Sets the visibility property for a specified player's displayed cards control.
    /// </summary>
    /// <param name="playerIndex">The index of the player.</param>
    /// <param name="newVisibility">The new visibility value to set for the player's control.</param>
    private void SetPlayersPlayedCardDisplayControlVisibility(int playerIndex, Visibility newVisibility)
    {
        switch (playerIndex)
        {
            case 0:
                Player1PlayedCardsDisplayUserControlVisibility = newVisibility;
                break;

            case 1:
                Player2PlayedCardsDisplayUserControlVisibility = newVisibility;
                break;

            case 2:
                Player3PlayedCardsDisplayUserControlVisibility = newVisibility;
                break;

            case 3:
                Player4PlayedCardsDisplayUserControlVisibility = newVisibility;
                break;
        }
    }

    /// <summary>
    /// Restores the interface of the main form to prompting the user for a new game.
    /// </summary>
    private void ResetGameUI()
    {
        GameBoardVisibility = Visibility.Hidden;
        SetMenuVisibility();
        _mainWindowController.ClearScoresAndBidInformation();
    }

    /// <summary>
    /// Sets the visibility of the menus on the main form.
    /// </summary>
    private void SetMenuVisibility()
    {
        IconMenuVisibility = GameSettingsManager.Instance.UseStandardMenu ?
            Visibility.Collapsed : Visibility.Visible;
        StandardMenuVisibility = GameSettingsManager.Instance.UseStandardMenu ?
            Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Restores the UI when a saved game is restored.
    /// </summary>
    private void RestoreGameUI()
    {
        if (CurrentGame!.GameInfo.LastCompletedStage == RoundStage.CardsDealt)
        {
            SetUIAfterCardsDealt();
            if (CurrentGame.GameInfo.Kitty != null)
            {
                _mainWindowController.DisplayKittyCard(CurrentGame.GameInfo.Kitty);
            }
        }
        else if (CurrentGame.GameInfo.LastCompletedStage == RoundStage.TrumpChosen)
        {
            SetUIAfterCardsDealt();
            _mainWindowController.SetTrump(CurrentGame.GameInfo.Trump!.Value);
            _mainWindowController.SetBidInformation(CurrentGame.GameInfo.TrumpCaller!,
                CurrentGame.GameInfo.Trump.Value, CurrentGame.GameInfo.TrumpCaller!.IsGoingAlone);
            _mainWindowController.SetPlayerTrumpSuitIcon(CurrentGame.GameInfo.TrumpCaller.PlayerIndex, 
                CurrentGame.GameInfo.Trump.Value);
            if (CurrentGame.GameInfo.TrumpCaller.IsGoingAlone)
            {
                _mainWindowController.SetPartnerDisabled(CurrentGame.GameInfo.TrumpCaller);
            }
            _mainWindowController.UpdatePlayersTrickCounters();
            ReloadTricks();
        }
    }

    /// <summary>
    /// Called after a saved game has been restored, updates the dealer icon and each player's hand.
    /// </summary>
    private void SetUIAfterCardsDealt()
    {
        _previousDealerPlayerIndex = CurrentGame!.GameInfo.Dealer!.PlayerIndex;
        _mainWindowController.SetPlayerDealerIconVisibility(CurrentGame!.GameInfo.Dealer!.PlayerIndex, true);
        UpdateAllPlayersHands();
    }

    /// <summary>
    /// Updates the control that displays the played tricks.
    /// </summary>
    private void ReloadTricks()
    {
        foreach (var trick in CurrentGame!.GameInfo.CurrentRoundTricks)
        {
            _mainWindowController.AddTrick(trick);
        }
    }

    /// <summary>
    /// Redisplays the cards in each player's hand based on the current trump suit.
    /// </summary>
    private void UpdateAllPlayersHands()
    {
        foreach (var player in CurrentGame!.GameInfo.Players!)
        {
            UpdatePlayersHandAfterTrumpSet(player.PlayerIndex, CurrentGame.GameInfo.Trump!.Value);
        }
    }

    /// <summary>
    /// Sets the action to use when setting the visibility of each player's card displaying control.
    /// </summary>
    private void SetVisibilityActionsOfPlayerCardDisplayControl()
    {
        _mainWindowController.SetPlayerCardDisplayControlVisibility(0,
            c => Player1CardDisplayUserControlVisibility = c);
        _mainWindowController.SetPlayerCardDisplayControlVisibility(1,
            c => Player2CardDisplayUserControlVisibility = c);
        _mainWindowController.SetPlayerCardDisplayControlVisibility(2,
            c => Player3CardDisplayUserControlVisibility = c);
        _mainWindowController.SetPlayerCardDisplayControlVisibility(3,
            c => Player4CardDisplayUserControlVisibility = c);
    }
}
