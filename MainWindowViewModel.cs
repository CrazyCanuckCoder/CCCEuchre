using CrazyCanuckCoder.Library.WPF;
using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Interfaces;
using Euchre.UserControls;
using System.Windows;

namespace Euchre;

public class MainWindowViewModel : ViewModelBase
{
    #region Fields

    // The fields used for the properties.

    private bool _continueMenuEnabled;
    private bool _playLastCardInHand;
    private Visibility _gameBoardVisibility = Visibility.Collapsed;
    private Visibility _iconMenuVisibility = Visibility.Collapsed;
    private Visibility _standardMenuVisibility = Visibility.Visible;
    private Visibility _player1DealtCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private Visibility _player2DealtCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private Visibility _player3DealtCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private Visibility _player4DealtCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private Visibility _player1PlayedCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private Visibility _player2PlayedCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private Visibility _player3PlayedCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private Visibility _player4PlayedCardsDisplayUserControlVisibility = Visibility.Collapsed;
    private string _gameInformation = string.Empty;
    private Visibility _gameInformationVisibility = Visibility.Collapsed;
    private Visibility _player1TextVisibility = Visibility.Collapsed;
    private Visibility _player2TextVisibility = Visibility.Collapsed;
    private Visibility _player3TextVisibility = Visibility.Collapsed;
    private Visibility _player4TextVisibility = Visibility.Collapsed;
    private string _player4Text = string.Empty;
    private string _player3Text = string.Empty;
    private string _player2Text = string.Empty;
    private string _player1Text = string.Empty;
    private Visibility _player1CardDisplayUserControlVisibility = Visibility.Visible;
    private Visibility _player2CardDisplayUserControlVisibility = Visibility.Visible;
    private Visibility _player3CardDisplayUserControlVisibility = Visibility.Visible;
    private Visibility _player4CardDisplayUserControlVisibility = Visibility.Visible;



    /// <summary>
    /// Used for setting the visibility properties for each of the player cards user controls.
    /// </summary>
    private Action<Visibility>? _visibilitySetter;
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
    /// Tracks the cards display control for unknown partner card.
    /// </summary>
    private readonly Dictionary<int, IBaseCardDisplay> _playerUnknownCardDisplayControls = [];

    /// <summary>
    /// Tracks the visibility of the regular cards display controls.
    /// </summary>
    private readonly Dictionary<int, Action<Visibility>> _playerCardDisplayControlsVisibility = [];

    #endregion Fields

    #region Properties

    public Visibility Player3CardDisplayUserControlVisibility
    {
        get => _player3CardDisplayUserControlVisibility;
        set
        {
            _player3CardDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player4CardDisplayUserControlVisibility
    {
        get => _player4CardDisplayUserControlVisibility;
        set
        {
            _player4CardDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player1DealtCardsDisplayUserControlVisibility
    {
        get => _player1DealtCardsDisplayUserControlVisibility;
        set
        {
            _player1DealtCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player2DealtCardsDisplayUserControlVisibility
    {
        get => _player2DealtCardsDisplayUserControlVisibility;
        set
        {
            _player2DealtCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player3DealtCardsDisplayUserControlVisibility
    {
        get => _player3DealtCardsDisplayUserControlVisibility;
        set
        {
            _player3DealtCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player4DealtCardsDisplayUserControlVisibility
    {
        get => _player4DealtCardsDisplayUserControlVisibility;
        set
        {
            _player4DealtCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player1PlayedCardsDisplayUserControlVisibility
    {
        get => _player1PlayedCardsDisplayUserControlVisibility;
        set
        {
            _player1PlayedCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player2PlayedCardsDisplayUserControlVisibility
    {
        get => _player2PlayedCardsDisplayUserControlVisibility;
        set
        {
            _player2PlayedCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player3PlayedCardsDisplayUserControlVisibility
    {
        get => _player3PlayedCardsDisplayUserControlVisibility;
        set
        {
            _player3PlayedCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player4PlayedCardsDisplayUserControlVisibility
    {
        get => _player4PlayedCardsDisplayUserControlVisibility;
        set
        {
            _player4PlayedCardsDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }


    public Visibility GameBoardVisibility
    {
        get => _gameBoardVisibility;
        set
        {
            _gameBoardVisibility = value;
            OnPropertyChanged();
        }
    }

    public string GameInformation
    {
        get => _gameInformation;
        set
        {
            _gameInformation = value;
            OnPropertyChanged();
        }
    }

    public Visibility GameInformationVisibility
    {
        get => _gameInformationVisibility;
        set
        {
            _gameInformationVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player1TextVisibility
    {
        get => _player1TextVisibility;
        set
        {
            _player1TextVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player2TextVisibility
    {
        get => _player2TextVisibility;
        set
        {
            _player2TextVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player3TextVisibility
    {
        get => _player3TextVisibility;
        set
        {
            _player3TextVisibility = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player4TextVisibility
    {
        get => _player4TextVisibility;
        set
        {
            _player4TextVisibility = value;
            OnPropertyChanged();
        }
    }

    public string Player1Text
    {
        get => _player1Text;
        set
        {
            _player1Text = value;
            OnPropertyChanged();
        }
    }

    public string Player2Text
    {
        get => _player2Text;
        set
        {
            _player2Text = value;
            OnPropertyChanged();
        }
    }

    public string Player3Text
    {
        get => _player3Text;
        set
        {
            _player3Text = value;
            OnPropertyChanged();
        }
    }

    public string Player4Text
    {
        get => _player4Text;
        set
        {
            _player4Text = value;
            OnPropertyChanged();
        }
    }

    public Visibility Player1CardDisplayUserControlVisibility
    {
        get => _player1CardDisplayUserControlVisibility;
        set
        {
            _player1CardDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }


    /// <summary>
    /// The visibility setting for the controls that show the card when showing the regular cards.
    /// </summary>
    public Visibility Player2CardDisplayUserControlVisibility
    {
        get => _player2CardDisplayUserControlVisibility;
        set
        {
            _player2CardDisplayUserControlVisibility = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Enables the continue game menu option when a game data file exists.
    /// </summary>
    public bool ContinueMenuEnabled
    {
        get => _continueMenuEnabled;
        set 
        { 
            _continueMenuEnabled = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Displays or collapses the regular menu.
    /// </summary>
    public Visibility StandardMenuVisibility
    {
        get => _standardMenuVisibility; 
        set 
        { 
            _standardMenuVisibility = value; 
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Displays or collapses the icon menu.
    /// </summary>
    public Visibility IconMenuVisibility
    {
        get => _iconMenuVisibility;
        set 
        { 
            _iconMenuVisibility = value; 
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// True to indicate the last card in a human player's hand can be automatically played.
    /// </summary>
    public bool PlayLastCardInHand
    {
        get => _playLastCardInHand;
        set 
        { 
            _playLastCardInHand = value; 
            OnPropertyChanged();
        }
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
        ShowGameBoard();
        SetupPlayerDisplayControls();
    }

    public void DeclareDealer(IPlayer dealer)
    {
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
}
