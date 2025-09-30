using Euchre.Logic.Components;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.Windows;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Euchre;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new();
        ViewModel.Initialize();
        DataContext = ViewModel;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    public MainWindowViewModel ViewModel { get; }


    private async Task StartGame()
    {
        // Set up the event handlers for the main window and each human player.

        AddMainWindowEventHandlers();
        var humanPlayers =    from player in ViewModel.CurrentGame!.GameInfo!.Players
                             where player != null && player.IsHuman
                            select player as HumanPlayer;
        foreach (var humanPlayer in humanPlayers)
        {
            AddHumanPlayerEventHandlers(humanPlayer);
        }

        // Start the game.

        await ViewModel.CurrentGame.PlayGameAsync();
    }

    private void AddMainWindowEventHandlers()
    {
        ViewModel.CurrentGame!.CardPlayedByPlayer += CurrentGame_CardPlayedByPlayer;
        ViewModel.CurrentGame.CardsDealtToPlayer += CurrentGame_CardsDealtToPlayer;
        ViewModel.CurrentGame.DeclareDealer += CurrentGame_DeclareDealer;
        ViewModel.CurrentGame.DeclareRoundWinningPlayers += CurrentGame_DeclareRoundWinningPlayers;
        ViewModel.CurrentGame.DeclareTrickWinner += CurrentGame_DeclareTrickWinner;
        ViewModel.CurrentGame.GameOver += CurrentGame_GameOver;
        ViewModel.CurrentGame.PlayerBidResult += CurrentGame_PlayerBidResult;
    }

    private void AddHumanPlayerEventHandlers(HumanPlayer humanPlayer)
    {
        humanPlayer.PromptForCardToPlay += HumanPlayer_PromptForCardToPlay;
        humanPlayer.PromptForDiscard += HumanPlayer_PromptForDiscard;
        humanPlayer.PromptForTrumpSuit += HumanPlayer_PromptForTrumpSuit;
        humanPlayer.PromptToOrderUp += HumanPlayer_PromptToOrderUp;
    }

    // TODO: Change the return type to be IEnumerable<AutomatedPlayerAvatar>.
    private IEnumerable<AutomatedPlayerAvatar>? GetPlayerNamesFromUser()
    {
        NewGameWindow newGameWindow = new()
        {
            Owner = this,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        if (newGameWindow.ShowDialog() == true)
        {
            return newGameWindow.GetPlayers();
        }

        return null;
    }

    /// <summary>
    /// Ensures the UI is updated.
    /// </summary>
    private static void AllowUIToUpdate()
    {
        DispatcherFrame frame = new();

        // DispatcherPriority set to Input, the highest priority.

        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input,
            new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                Thread.Sleep(20); // Stop all processes to make sure the UI update is perform
                return null;
            }), null);
        Dispatcher.PushFrame(frame);

        // DispatcherPriority set to Input, the highest priority.

        Application.Current?.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
    }


    #region EventHandlers

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // For future need.  Add code similar to the following example:

        if (e.PropertyName == nameof(ViewModel.ContinueMenuEnabled))
        {
        }
    }

    // The event handlers for the main window.

    private void CurrentGame_PlayerBidResult(object? sender, PlayerBidEventArgs e)
    {
    }

    private void CurrentGame_GameOver(object? sender, GameOverEventArgs e)
    {
    }

    private void CurrentGame_DeclareTrickWinner(object? sender, DeclareTrickWinnerEventArgs e)
    {
    }

    private void CurrentGame_DeclareRoundWinningPlayers(object? sender, DeclareRoundWinningPlayersEventArgs e)
    {
    }

    private void CurrentGame_DeclareDealer(object? sender, DeclareDealerEventArgs e)
    {
    }

    private void CurrentGame_CardsDealtToPlayer(object? sender, CardsDealtToPlayerEventArgs e)
    {
    }

    private void CurrentGame_CardPlayedByPlayer(object? sender, CardPlayedByPlayerEventArgs e)
    {
    }

    // The event handlers for the human player.

    private void HumanPlayer_PromptToOrderUp(object? sender, PromptToOrderUpEventArgs e)
    {
    }

    private void HumanPlayer_PromptForTrumpSuit(object? sender, PromptForTrumpSuitEventArgs e)
    {
    }

    private void HumanPlayer_PromptForDiscard(object? sender, PromptForDiscardEventArgs e)
    {
    }

    private void HumanPlayer_PromptForCardToPlay(object? sender, PromptForCardToPlayEventArgs e)
    {
    }

    #endregion EventHandlers

    #region Menu EventHandlers

    private async void MenuNewGame_Click(object sender, RoutedEventArgs e)
    {
        var playerNames = GetPlayerNamesFromUser();
        if (playerNames != null)
        {
            ViewModel.CurrentGame = new(playerNames.ToList());
            await StartGame();
        }
    }

    private async void MenuContinue_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CurrentGame = new();
        await StartGame();
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void menuChooseTrump_Click(object sender, RoutedEventArgs e)
    {

    }

    private void menuChooseCard_Click(object sender, RoutedEventArgs e)
    {

    }

    private void menuChooseCards_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MenuToolsOptions_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MenuHelpAbout_Click(object sender, RoutedEventArgs e)
    {

    }

    #endregion Menu EventHandlers
}