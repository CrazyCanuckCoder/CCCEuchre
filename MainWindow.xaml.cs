using Euchre.Logic.Components;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Helpers;
using Euchre.Windows;
using System.Windows;

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
    }

    /// <summary>
    /// The view model class to use to manage the data properties.
    /// </summary>
    public MainWindowViewModel ViewModel { get; }

    /// <summary>
    /// Starts the game of Euchre asynchronously.  Assumes the players have been created before the method
    /// has been called.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation.</returns>
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

        // Set up the elements on the game board.

        ViewModel.SetupUserInterface(this);

        // Start the game.

        await ViewModel.CurrentGame.PlayGameAsync();
    }

    /// <summary>
    /// Adds event handlers to all of the events fired by the class running the game.
    /// </summary>
    private void AddMainWindowEventHandlers()
    {
        ViewModel.CurrentGame!.CardPlayedByPlayer += CurrentGame_CardPlayedByPlayer;
        ViewModel.CurrentGame.CardsDealtToPlayer += CurrentGame_CardsDealtToPlayer;
        ViewModel.CurrentGame.DeclareDealer += CurrentGame_DeclareDealer;
        ViewModel.CurrentGame.DeclareKittyCard += CurrentGame_DeclareKittyCard;
        ViewModel.CurrentGame.KittyWasTurnedDown += CurrentGame_KittyWasTurnedDown;
        ViewModel.CurrentGame.DeclareRoundWinningPlayers += CurrentGame_DeclareRoundWinningPlayers;
        ViewModel.CurrentGame.DeclareTrickWinner += CurrentGame_DeclareTrickWinner;
        ViewModel.CurrentGame.GameOver += CurrentGame_GameOver;
        ViewModel.CurrentGame.PlayerBidResult += CurrentGame_PlayerBidResult;
    }

    /// <summary>
    /// Adds event handlers for all of the events a human player's class may fire.
    /// </summary>
    /// <param name="humanPlayer">The human player to attach events.</param>
    private void AddHumanPlayerEventHandlers(HumanPlayer humanPlayer)
    {
        humanPlayer.PromptForCardToPlay += HumanPlayer_PromptForCardToPlay;
        humanPlayer.PromptForDiscard += HumanPlayer_PromptForDiscard;
        humanPlayer.PromptForTrumpSuit += HumanPlayer_PromptForTrumpSuit;
        humanPlayer.PromptToOrderUp += HumanPlayer_PromptToOrderUp;
    }

    /// <summary>
    /// Prompts the user to enter their name and avatar plus select the automated players.
    /// </summary>
    /// <returns>A list of player names of avatar numbers.</returns>
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

        // User closed the window to indicate they cancelled the operation.

        return null;
    }


    #region EventHandlers

    // The event handlers for the main window.

    private void CurrentGame_PlayerBidResult(object? sender, PlayerBidEventArgs e)
    {
        if (!e.MadeTrump)
        {
            ViewModel.PlayerPassed(e.Player, e.IsKittyRound);
        }
        else
        {
            ViewModel.PlayerMadeTrump(e.Player, e.Trump, e.IsGoingAlone, e.IsKittyRound);
        }
    }

    private void CurrentGame_GameOver(object? sender, GameOverEventArgs e)
    {
    }

    private void CurrentGame_DeclareTrickWinner(object? sender, DeclareTrickWinnerEventArgs e)
    {
        // Update the previous tricks control with the current trick.
    }

    private void CurrentGame_DeclareRoundWinningPlayers(object? sender, DeclareRoundWinningPlayersEventArgs e)
    {
        // Clear the previous tricks control.
    }

    private void CurrentGame_DeclareDealer(object? sender, DeclareDealerEventArgs e)
    {
        ViewModel.DeclareDealer(e.Dealer);
    }

    private void CurrentGame_CardsDealtToPlayer(object? sender, CardsDealtToPlayerEventArgs e)
    {
        ViewModel.DealCardsToPlayer(e.Player.PlayerIndex, e.NumberOfCardsDealt);
    }

    private void CurrentGame_DeclareKittyCard(object? sender, DeclareKittyCardEventArgs e)
    {
        ViewModel.SetKittyCard(e.Kitty);
    }

    private void CurrentGame_KittyWasTurnedDown(object? sender, EventArgs e)
    {
        ViewModel.KittyWasTurnedDown();
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