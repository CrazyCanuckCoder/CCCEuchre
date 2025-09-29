using Euchre.Logic.Components;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using System.Windows;
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
    }

    private readonly MainWindowViewModel _viewModel = new();

    private async Task StartGame()
    {
        // Check if there is a game saved that the user wants to continue.

        if (GameStateManager.DataExists())
        {
            // If so, load the saved game.

            _viewModel.CurrentGame = new();
        }
        else
        {
            // If not, start a new game by getting the names of the players.

            List<string> playerNames = GetPlayerNamesFromUser();
            _viewModel.CurrentGame = new(playerNames);
        }

        // Set up the event handlers for the main window and each human player.

        AddMainWindowEventHandlers();
        var humanPlayers = (  from player in _viewModel.CurrentGame.GameInfo!.Players
                             where player != null && player.IsHuman
                            select player as HumanPlayer);
        foreach (var humanPlayer in humanPlayers)
        {
            AddHumanPlayerEventHandlers(humanPlayer);
        }
        
        // Start the game.

        await _viewModel.CurrentGame.PlayGameAsync();
    }

    private void AddMainWindowEventHandlers()
    {
        _viewModel.CurrentGame!.CardPlayedByPlayer += CurrentGame_CardPlayedByPlayer;
        _viewModel.CurrentGame.CardsDealtToPlayer += CurrentGame_CardsDealtToPlayer;
        _viewModel.CurrentGame.DeclareDealer += CurrentGame_DeclareDealer;
        _viewModel.CurrentGame.DeclareRoundWinningPlayers += CurrentGame_DeclareRoundWinningPlayers;
        _viewModel.CurrentGame.DeclareTrickWinner += CurrentGame_DeclareTrickWinner;
        _viewModel.CurrentGame.GameOver += CurrentGame_GameOver;
        _viewModel.CurrentGame.PlayerBidResult += CurrentGame_PlayerBidResult;
    }

    private void AddHumanPlayerEventHandlers(HumanPlayer humanPlayer)
    {
        humanPlayer.PromptForCardToPlay += HumanPlayer_PromptForCardToPlay;
        humanPlayer.PromptForDiscard += HumanPlayer_PromptForDiscard;
        humanPlayer.PromptForTrumpSuit += HumanPlayer_PromptForTrumpSuit;
        humanPlayer.PromptToOrderUp += HumanPlayer_PromptToOrderUp;
    }

    private List<string> GetPlayerNamesFromUser()
    {
        return ["Alice", "Bob", "Charlie", "Diana"]; // Placeholder for actual user input
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
}