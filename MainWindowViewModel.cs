using CrazyCanuckCoder.Library.Common;
using Euchre.Logic.Components;
using Euchre.Logic.Enums;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Interfaces;
using Euchre.UserControls;
using System.Windows;
using System.Windows.Data;

namespace Euchre;

public class MainWindowViewModel : DependencyObject
{
    #region Fields

    /// <summary>
    /// Tracks the player index value for the previous dealer.
    /// </summary>
    private int _previousDealerPlayerIndex = -1;

    /// <summary>
    /// Used for setting the visibility properties for each of the player cards user controls.
    /// </summary>
    private Action<Visibility>? _visibilitySetter;

    /// <summary>
    /// A reference to the main window.
    /// </summary>
    private MainWindow _mainWindow;

    /// <summary>
    /// Tracks a cards display control to a player.
    /// </summary>
    private readonly Dictionary<int, IBaseCardDisplay> _playerCardDisplayControls = [];

    /// <summary>
    /// Tracks the card backs display control that shows the player receiving cards that are dealt to them.
    /// </summary>
    private readonly Dictionary<int, IBaseCardDisplay> _playerDealtCardsDisplayControls = [];

    /// <summary>
    /// Tracks the played cards display control that shows the cards played by a player.
    /// </summary>
    private readonly Dictionary<int, IBaseCardDisplay> _playerPlayedCardsDisplayControls = [];

    /// <summary>
    /// Tracks the visibility of the regular cards display controls.
    /// </summary>
    private readonly Dictionary<int, Action<Visibility>> _playerCardDisplayControlsVisibility = [];

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

    public static readonly DependencyProperty ContinueMenuEnabledProperty =
        DependencyProperty.Register(nameof(ContinueMenuEnabled), typeof(bool), typeof(MainWindowViewModel), 
            new PropertyMetadata(false));

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
    /// Enables the continue game menu option when a game data file exists.
    /// </summary>
    public bool ContinueMenuEnabled
    {
        get => (bool)GetValue(ContinueMenuEnabledProperty);
        set => SetValue(ContinueMenuEnabledProperty, value);
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
        ContinueMenuEnabled = GameStateManager.DataExists();
        IconMenuVisibility = GameSettingsManager.Instance.UseStandardMenu ? 
            Visibility.Collapsed : Visibility.Visible;
        StandardMenuVisibility = GameSettingsManager.Instance.UseStandardMenu ? 
            Visibility.Visible : Visibility.Collapsed;
        PlayLastCardInHand = GameSettingsManager.Instance.PlayLastCardInHand;
    }

    /// <summary>
    /// Call this method to initialize the controls for the game on the main window.
    /// </summary>
    /// <param name="mainWindow">A reference to the main window to update.</param>
    public void SetupUserInterface(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        SetupCurrentRoundInfoControl(_mainWindow.CurrentRoundInfoUserControl);
        SetupCurrentScoreControl(_mainWindow.CurrentPlayersScoresUserControl);
        InitializeCardDisplayControlCollection();
        ShowGameBoard();
        SetupPlayerDisplayControls();
    }

    public void DeclareDealer(IPlayer dealer)
    {
        SetPlayerDealerIconVisibility(dealer.PlayerIndex, true);
        SetPlayerDealerIconVisibility(_previousDealerPlayerIndex, false);

        _previousDealerPlayerIndex = dealer.PlayerIndex;

        UpdateGameInformation($"{dealer.Name} is the dealer.");
    }

    public void DealCardsToPlayer(int indexOfPlayer, int numberOfCardsDealt)
    {
        _playerDealtCardsDisplayControls[indexOfPlayer].DisplayCards(numberOfCardsDealt);
        SetPlayerVisibility(indexOfPlayer, Visibility.Visible, Visibility.Collapsed);

        PauseGame(1);

        SetPlayerVisibility(indexOfPlayer, Visibility.Collapsed, Visibility.Collapsed);
        _playerDealtCardsDisplayControls[indexOfPlayer].ClearCards();

        UpdatePlayersHandAfterDeal(indexOfPlayer, numberOfCardsDealt);
    }

    public void SetKittyCard(Card kitty)
    {
        _mainWindow.TrumpDisplayUserControl.SetKittyCard(kitty);
    }

    public void PlayerPassed(IPlayer player, bool isKittyRound)
    {
        string message = "Pass";

        if (isKittyRound && CurrentGame!.GameInfo!.Dealer == player)
        {
            message = "Turning down the kitty card.";
        }

        DisplayPlayerMessage(player, message, 1, false);
    }

    public void KittyWasTurnedDown()
    {
        UpdateGameInformation("The kitty card was turned down.");
        ClearPlayerMessages();
    }

    public void PlayerMadeTrump(IPlayer player, Suit? trump, bool isGoingAlone, bool isKittyRound)
    {
        string message = string.Empty;

        if (isKittyRound)
        {
            string goingAlone = isGoingAlone ? "and I am going alone" : "";
            message = CurrentGame!.GameInfo!.Dealer == player
                ? $"I am picking up the kitty card{goingAlone}."
                : $"Pick it up{goingAlone}.";
        }
        else
        {
            message = $"{trump!.Value} {(isGoingAlone ? " alone" : "")}";
        }

        DisplayPlayerMessage(player, message, 1, false);
        _mainWindow.CurrentRoundInfoUserControl.SetBidInformation(player, trump!.Value, isGoingAlone);
        ClearPlayerMessages();
    }


    public void DisplayCardPlayedByPlayer(IPlayer player, ICard cardPlayed)
    {
        if (player is HumanPlayer humanPlayer)
        {
            _playerCardDisplayControls[player.PlayerIndex].SetupCards(humanPlayer.Hand,
                CurrentGame!.GameInfo!.Trump!.Value);
        }
        else
        {
#if DEBUG
            if (player is AutomatedPlayer automatedPlayer)
            {
                _playerCardDisplayControls[player.PlayerIndex].SetupCards(automatedPlayer.Hand,
                    CurrentGame!.GameInfo!.Trump!.Value);
            }
#else
            _playerCardDisplayControls[player.PlayerIndex].RemoveCard();
#endif
        }
        SetPlayersPlayedCardDisplayControlVisibility(player.PlayerIndex, Visibility.Visible);
        var cardPlayedClone = cardPlayed.Clone();
        cardPlayedClone.IsPlayed = false;
        _playerPlayedCardsDisplayControls[player.PlayerIndex].SetupCards([(Card)cardPlayedClone]);

        PauseGame(2);
    }


    public void EndOfTrick(IPlayer trickWinningPlayer)
    {
        // Update the trick winner's number of tricks won on the display for the user and the list of tricks
        //  for each player for the game.

        switch (trickWinningPlayer.PlayerIndex)
        {
            case 0:
                _mainWindow.Player1DisplayUserControl.UpdateNumberOfTricks(
                    CurrentGame!.GameInfo!.TricksWonByPlayers[trickWinningPlayer]);
                break;

            case 1:
                _mainWindow.Player2DisplayUserControl.UpdateNumberOfTricks(
                    CurrentGame!.GameInfo!.TricksWonByPlayers[trickWinningPlayer]);
                break;

            case 2:
                _mainWindow.Player3DisplayUserControl.UpdateNumberOfTricks(
                    CurrentGame!.GameInfo!.TricksWonByPlayers[trickWinningPlayer]);
                break;

            case 3:
                _mainWindow.Player4DisplayUserControl.UpdateNumberOfTricks(
                    CurrentGame!.GameInfo!.TricksWonByPlayers[trickWinningPlayer]);
                break;
        }
        UpdateGameInformation($"{trickWinningPlayer.Name} won the trick.");

        // Add the trick to the list of previous tricks and clear the played cards from the game board.

        _mainWindow.PreviousTricksUserControl.AddTrick(CurrentGame!.GameInfo!.CurrentRoundTricks.Last());
        ClearPlayedCards();
    }

    public void EndOfRoundUpdate(List<string> winningPlayers)
    {
        // Display the winning round message to the user.

        UpdateGameInformation($"{winningPlayers.ToListedString()} won the round.");

        // The round is over, reset the board for the next round.

        ResetCardDisplayControlsVisibility();
        ClearPlayersHands();
        UpdateGameScores();
        ResetTrickCounters();
        _mainWindow.PreviousTricksUserControl.ClearTricks();
        _mainWindow.TrumpDisplayUserControl.ClearImage();
        _mainWindow.CurrentRoundInfoUserControl.ResetBidInformation();
    }


    /// <summary>
    /// Updates the collection that references each player's card display control and the collection that
    /// references each player's cards that are dealt to them.
    /// </summary>
    private void InitializeCardDisplayControlCollection()
    {
        _playerCardDisplayControls.Add(0, _mainWindow.Player1CardDisplayUserControl);
        _playerCardDisplayControls.Add(1, _mainWindow.Player2CardDisplayUserControl);
        _playerCardDisplayControls.Add(2, _mainWindow.Player3CardDisplayUserControl);
        _playerCardDisplayControls.Add(3, _mainWindow.Player4CardDisplayUserControl);

        _playerCardDisplayControlsVisibility.Add(1, c => Player2CardDisplayUserControlVisibility = c);
        _playerCardDisplayControlsVisibility.Add(2, c => Player3CardDisplayUserControlVisibility = c);
        _playerCardDisplayControlsVisibility.Add(3, c => Player4CardDisplayUserControlVisibility = c);

        _playerDealtCardsDisplayControls.Add(0, _mainWindow.Player1DealtCardsDisplayUserControl);
        _playerDealtCardsDisplayControls.Add(1, _mainWindow.Player2DealtCardsDisplayUserControl);
        _playerDealtCardsDisplayControls.Add(2, _mainWindow.Player3DealtCardsDisplayUserControl);
        _playerDealtCardsDisplayControls.Add(3, _mainWindow.Player4DealtCardsDisplayUserControl);

        _playerPlayedCardsDisplayControls.Add(0, _mainWindow.Player1PlayedCardsDisplayUserControl);
        _playerPlayedCardsDisplayControls.Add(1, _mainWindow.Player2PlayedCardsDisplayUserControl);
        _playerPlayedCardsDisplayControls.Add(2, _mainWindow.Player3PlayedCardsDisplayUserControl);
        _playerPlayedCardsDisplayControls.Add(3, _mainWindow.Player4PlayedCardsDisplayUserControl);
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
    /// Adds the players names to the correct teams on the current round information user control.
    /// </summary>
    /// <param name="currentRoundInfoUserControl"></param>
    private void SetupCurrentRoundInfoControl(CurrentRoundInfoUserControl currentRoundInfoUserControl)
    {
        currentRoundInfoUserControl.Team1List.Add(CurrentGame!.GameInfo!.Players![0]);
        currentRoundInfoUserControl.Team1List.Add(CurrentGame.GameInfo.Players[2]);
        currentRoundInfoUserControl.Team2List.Add(CurrentGame.GameInfo.Players[1]);
        currentRoundInfoUserControl.Team2List.Add(CurrentGame.GameInfo.Players[3]);
    }

    private void SetupCurrentScoreControl(CurrentScoreUserControl currentPlayersScoresUserControl)
    {
        currentPlayersScoresUserControl.SetTeamNames(
            (  from player in CurrentGame!.GameInfo!.Players!
            orderby player.PlayerIndex
             select player.Name).ToList()
            );
        currentPlayersScoresUserControl.UpdateTeamScore(1, 0);
        currentPlayersScoresUserControl.UpdateTeamScore(2, 0);
    }

    /// <summary>
    /// Sets each player's name and avatar on the main window.
    /// </summary>
    private void SetupPlayerDisplayControls()
    {
        _mainWindow.Player1DisplayUserControl.SetActivePlayer(CurrentGame!.GameInfo!.Players![0],
            CurrentGame.GameInfo.Players[0].AvatarNumber);
        _mainWindow.Player2DisplayUserControl.SetActivePlayer(CurrentGame.GameInfo.Players[1],
            CurrentGame.GameInfo.Players[1].AvatarNumber);
        _mainWindow.Player3DisplayUserControl.SetActivePlayer(CurrentGame.GameInfo.Players[2],
            CurrentGame.GameInfo.Players[2].AvatarNumber);
        _mainWindow.Player4DisplayUserControl.SetActivePlayer(CurrentGame.GameInfo.Players[3],
            CurrentGame.GameInfo.Players[3].AvatarNumber);
    }

    /// <summary>
    /// Hides or shows the dealer icon for a specified player.
    /// </summary>
    /// <param name="playerIndex">The index of the player.</param>
    /// <param name="isVisible">True to show the icon and false to hide it.</param>
    private void SetPlayerDealerIconVisibility(int playerIndex, bool isVisible)
    {
        switch (playerIndex)
        {
            case 0:
                _mainWindow.Player1DisplayUserControl.SetDealerIconVisibility(isVisible);
                break;

            case 1:
                _mainWindow.Player2DisplayUserControl.SetDealerIconVisibility(isVisible);
                break;

            case 2:
                _mainWindow.Player3DisplayUserControl.SetDealerIconVisibility(isVisible);
                break;

            case 3:
                _mainWindow.Player4DisplayUserControl.SetDealerIconVisibility(isVisible);
                break;
        }
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
    private void UpdateGameInformation(string message)
    {
        GameInformation = message;
        GameInformationVisibility = Visibility.Visible;

        // Wait for two seconds.

        PauseGame(2);

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
    /// <param name="indexOfPlayer">The index of the player from the GameManager's Players list.</param>
    /// <param name="numberOfCardsDealt">The number of cards dealt to an automated player.</param>
    private void UpdatePlayersHandAfterDeal(int indexOfPlayer, int numberOfCardsDealt)
    {
        if (CurrentGame!.GameInfo!.Players![indexOfPlayer] is HumanPlayer humanPlayer)
        {
            var sortedCards = humanPlayer.Hand.Sort(trump: null);
            _playerCardDisplayControls[indexOfPlayer].SetupCards(sortedCards);
        }
        else
        {
#if DEBUG
            if (CurrentGame.GameInfo.Players[indexOfPlayer] is AutomatedPlayer automatedPlayer)
            {
                var sortedCards = automatedPlayer.Hand.Sort(trump: null);
                _playerCardDisplayControls[indexOfPlayer].SetupCards(sortedCards);
            }
#else
                _playerCardDisplayControls[indexOfPlayer].AddCards(numberOfCardsDealt);
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
    /// Clears the controls that displayed the cards played for the current round.
    /// </summary>
    private void ClearPlayedCards()
    {
        for (int index = 0; index < _playerPlayedCardsDisplayControls.Count; index++)
        {
            _playerPlayedCardsDisplayControls[index].ClearCards();
            SetPlayersPlayedCardDisplayControlVisibility(index, Visibility.Collapsed);
        }
    }

    /// <summary>
    /// Blanks out the cards in each player's card display control to get the control ready to receive new
    /// cards for the next round.
    /// </summary>
    private void ClearPlayersHands()
    {
        foreach (var playersHandsControl in _playerCardDisplayControls.Values)
        {
            playersHandsControl?.ClearCards();
        }
    }

    private void ResetCardDisplayControlsVisibility()
    {
        _playerCardDisplayControls[0] = _mainWindow.Player1CardDisplayUserControl;
        Player1CardDisplayUserControlVisibility = Visibility.Visible;

        // Update the visibility values for the automated players.

        for (int playerIndex = 1; playerIndex < Constants.NUMBER_OF_PLAYERS; playerIndex++)
        {
            if (_playerCardDisplayControlsVisibility.ContainsKey(playerIndex))
            {
                switch (playerIndex)
                {
                    case 1:
                        _playerCardDisplayControls[playerIndex] = _mainWindow.Player2CardDisplayUserControl;
                        break;

                    case 2:
                        _playerCardDisplayControls[playerIndex] = _mainWindow.Player3CardDisplayUserControl;
                        break;

                    case 3:
                        _playerCardDisplayControls[playerIndex] = _mainWindow.Player4CardDisplayUserControl;
                        break;
                }
                _visibilitySetter = _playerCardDisplayControlsVisibility[playerIndex];
                _visibilitySetter(Visibility.Visible);
            }
        }
    }

    /// <summary>
    /// Resets each player's number of tricks won to zero.
    /// </summary>
    private void ResetTrickCounters()
    {
        for (int playerIndex = 0; playerIndex < Constants.NUMBER_OF_PLAYERS; playerIndex++)
        {
            switch (playerIndex)
            {
                case 0:
                    _mainWindow.Player1DisplayUserControl.UpdateNumberOfTricks(0);
                    break;

                case 1:
                    _mainWindow.Player2DisplayUserControl.UpdateNumberOfTricks(0);
                    break;

                case 2:
                    _mainWindow.Player3DisplayUserControl.UpdateNumberOfTricks(0);
                    break;

                case 3:
                    _mainWindow.Player4DisplayUserControl.UpdateNumberOfTricks(0);
                    break;
            }
        }
    }

    /// <summary>
    /// Updates the player score display with the scores for each team.
    /// </summary>
    private void UpdateGameScores()
    {
        _mainWindow.CurrentPlayersScoresUserControl.UpdateTeamScore(1, CurrentGame!.GameInfo!.TeamScores[0]);
        _mainWindow.CurrentPlayersScoresUserControl.UpdateTeamScore(2, CurrentGame.GameInfo.TeamScores[1]);
    }
}
