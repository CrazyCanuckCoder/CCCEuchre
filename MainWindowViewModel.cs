using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Interfaces;
using Euchre.UserControls;
using System.Windows;

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
        InitializeCardDisplayControlCollection();
        ShowGameBoard();
        SetupPlayerDisplayControls();
    }

    public void DeclareDealer(IPlayer dealer)
    {
        _mainWindow.Dispatcher.Invoke(() =>
        {
            SetPlayerDealerIconVisibility(dealer.PlayerIndex, true);
            SetPlayerDealerIconVisibility(_previousDealerPlayerIndex, false);
        });

        _previousDealerPlayerIndex = dealer.PlayerIndex;

        UpdateGameInformation($"{dealer.Name} is the dealer.");
    }

    public void DealCardsToPlayer(int indexOfPlayer, int numberOfCardsDealt)
    {
        _mainWindow.Dispatcher.Invoke(() =>
        {
            _playerDealtCardsDisplayControls[indexOfPlayer].DisplayCards(numberOfCardsDealt);
            SetPlayerVisibility(indexOfPlayer, Visibility.Visible, Visibility.Collapsed);

            PauseGame(1);

            SetPlayerVisibility(indexOfPlayer, Visibility.Collapsed, Visibility.Collapsed);
            _playerDealtCardsDisplayControls[indexOfPlayer].ClearCards();

            UpdatePlayersHandAfterDeal(indexOfPlayer, numberOfCardsDealt);
        });
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

    private void ShowGameBoard()
    {
        GameBoardVisibility = Visibility.Visible;
        StandardMenuVisibility = Visibility.Collapsed;
        IconMenuVisibility = Visibility.Collapsed;
    }

    private void SetupCurrentRoundInfoControl(CurrentRoundInfoUserControl currentRoundInfoUserControl)
    {
        currentRoundInfoUserControl.Team1List.Add(CurrentGame!.GameInfo!.Players![0]);
        currentRoundInfoUserControl.Team1List.Add(CurrentGame.GameInfo.Players[2]);
        currentRoundInfoUserControl.Team2List.Add(CurrentGame.GameInfo.Players[1]);
        currentRoundInfoUserControl.Team2List.Add(CurrentGame.GameInfo.Players[3]);
    }

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
        _mainWindow.Dispatcher.Invoke(() =>
        {
            GameInformation = message;
            GameInformationVisibility = Visibility.Visible;

            // Wait for two seconds.

            PauseGame(2);

            // Hide the text.

            GameInformation = string.Empty;
            GameInformationVisibility = Visibility.Hidden;
        });
    }

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
}
