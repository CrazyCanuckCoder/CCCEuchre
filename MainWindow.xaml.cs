using CrazyCanuckCoder.Library.WPF;
using Euchre.Logic.Components;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Helpers;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
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
        _placementManager = new WindowPlacementManager(this);
        ViewModel = new();
        ViewModel.Initialize();
        DataContext = ViewModel;
    }

    private readonly WindowPlacementManager _placementManager;

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
        ViewModel.CurrentGame.TrumpCalled += CurrentGame_TrumpCalled;
        ViewModel.CurrentGame.NoTrumpCalled += CurrentGame_NoTrumpCalled;
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
        humanPlayer.UserHandUpdated += HumanPlayer_UserHandUpdated;
        humanPlayer.PromptForTrumpSuit += HumanPlayer_PromptForTrumpSuit;
        humanPlayer.PromptToOrderUp += HumanPlayer_PromptToOrderUp;
    }

    /// <summary>
    /// Removes event handlers to all of the events fired by the class running the game.
    /// </summary>
    private void RemoveMainWindowEventHandlers()
    {
        ViewModel.CurrentGame!.CardPlayedByPlayer -= CurrentGame_CardPlayedByPlayer;
        ViewModel.CurrentGame.CardsDealtToPlayer -= CurrentGame_CardsDealtToPlayer;
        ViewModel.CurrentGame.DeclareDealer -= CurrentGame_DeclareDealer;
        ViewModel.CurrentGame.DeclareKittyCard -= CurrentGame_DeclareKittyCard;
        ViewModel.CurrentGame.KittyWasTurnedDown -= CurrentGame_KittyWasTurnedDown;
        ViewModel.CurrentGame.TrumpCalled -= CurrentGame_TrumpCalled;
        ViewModel.CurrentGame.NoTrumpCalled -= CurrentGame_NoTrumpCalled;
        ViewModel.CurrentGame.DeclareRoundWinningPlayers -= CurrentGame_DeclareRoundWinningPlayers;
        ViewModel.CurrentGame.DeclareTrickWinner -= CurrentGame_DeclareTrickWinner;
        ViewModel.CurrentGame.GameOver -= CurrentGame_GameOver;
        ViewModel.CurrentGame.PlayerBidResult -= CurrentGame_PlayerBidResult;
    }

    /// <summary>
    /// Removes event handlers for all of the events a human player's class may fire.
    /// </summary>
    /// <param name="humanPlayer">The human player from which to remove events.</param>
    private void RemoveHumanPlayerEventHandlers(HumanPlayer humanPlayer)
    {
        humanPlayer.PromptForCardToPlay -= HumanPlayer_PromptForCardToPlay;
        humanPlayer.PromptForDiscard -= HumanPlayer_PromptForDiscard;
        humanPlayer.UserHandUpdated -= HumanPlayer_UserHandUpdated;
        humanPlayer.PromptForTrumpSuit -= HumanPlayer_PromptForTrumpSuit;
        humanPlayer.PromptToOrderUp -= HumanPlayer_PromptToOrderUp;
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
        UIHelpers.RunOnUIThread(() =>
        {
            if (!e.MadeTrump)
            {
                ViewModel.PlayerPassed(e.Player, e.IsKittyRound);
            }
            else
            {
                ViewModel.PlayerMadeTrump(e.Player, e.Trump, e.IsGoingAlone, e.IsKittyRound);
            }
        });
    }

    private void CurrentGame_GameOver(object? sender, GameOverEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            ViewModel.EndOfGameUpdate(e.GameInfo);

            // Remove the event handlers for the main window and each human player.

            RemoveMainWindowEventHandlers();
            var humanPlayers =   from player in ViewModel.CurrentGame!.GameInfo!.Players
                                where player != null && player.IsHuman
                               select player as HumanPlayer;
            foreach (var humanPlayer in humanPlayers)
            {
                RemoveHumanPlayerEventHandlers(humanPlayer);
            }
        });
    }

    private void CurrentGame_DeclareTrickWinner(object? sender, DeclareTrickWinnerEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            ViewModel.EndOfTrick(e.TrickWinningPlayer);
        });
    }

    private void CurrentGame_DeclareRoundWinningPlayers(object? sender, DeclareRoundWinningPlayersEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            ViewModel.EndOfRoundUpdate(e.WinningPlayers, e.Points, e.ReasonForPoints);
        });
    }

    private void CurrentGame_DeclareDealer(object? sender, DeclareDealerEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            ViewModel.DeclareDealer(e.Dealer);
        });
    }

    private void CurrentGame_CardsDealtToPlayer(object? sender, CardsDealtToPlayerEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            ViewModel.DealCardsToPlayer(e.Player.PlayerIndex, e.NumberOfCardsDealt);
        });
    }

    private void CurrentGame_DeclareKittyCard(object? sender, DeclareKittyCardEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            ViewModel.SetKittyCard(e.Kitty);
        });
    }

    private void CurrentGame_KittyWasTurnedDown(object? sender, EventArgs e)
    {
        UIHelpers.RunOnUIThread(ViewModel.KittyWasTurnedDown);
    }

    private void CurrentGame_TrumpCalled(object? sender, EventArgs e)
    {
        UIHelpers.RunOnUIThread(ViewModel.TrumpCalled);
    }

    private void CurrentGame_NoTrumpCalled(object? sender, EventArgs e)
    {
        UIHelpers.RunOnUIThread(ViewModel.NoTrumpWasCalled);
    }

    private void CurrentGame_CardPlayedByPlayer(object? sender, CardPlayedByPlayerEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            ViewModel.DisplayCardPlayedByPlayer(e.Player, e.CardPlayed);
        });
    }

    // The event handlers for the human player.

    private void HumanPlayer_PromptToOrderUp(object? sender, PromptToOrderUpEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            if (sender is HumanPlayer player)
            {
                e.OrderedUp = ViewModel.PromptUserToOrderUp(e.Kitty, player, out bool goAlone);
                e.GoAlone = goAlone;
            }
        });
    }

    private void HumanPlayer_PromptForTrumpSuit(object? sender, PromptForTrumpSuitEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            if (sender is HumanPlayer player)
            {
                e.TrumpSuit = ViewModel.PromptUserForTrump(e.Kitty, player, out bool goAlone);
                e.GoAlone = goAlone;
            }
        });
    }

    private void HumanPlayer_PromptForDiscard(object? sender, PromptForDiscardEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            if (sender is HumanPlayer player)
            {
                e.DiscardedCard = ViewModel.PromptUserForDiscard(player);
            }
        });
    }

    private void HumanPlayer_UserHandUpdated(object? sender, EventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            if (sender is HumanPlayer player)
            {
                ViewModel.RedisplayUserHand(player);
            }
        });
    }

    private void HumanPlayer_PromptForCardToPlay(object? sender, PromptForCardToPlayEventArgs e)
    {
        UIHelpers.RunOnUIThread(() =>
        {
            if (sender is HumanPlayer player)
            {
                e.PlayedCard = ViewModel.GetCardFromUser(player, e.LeadSuit, e.Trump);
            }
        });
    }

    #endregion EventHandlers

    #region Menu EventHandlers

    private async void MenuNewGame_Click(object sender, RoutedEventArgs e)
    {
        var playerNames = GetPlayerNamesFromUser();
        if (playerNames != null)
        {
            ViewModel.CurrentGame = new([.. playerNames]);
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
        List<Suit> trumpSuits =
        [
            Suit.Hearts,
            //Suit.Diamonds,
            Suit.Spades,
        ];
        PickTrumpSuitWindow pickTrump = new(trumpSuits, 
                                        new HumanPlayer("human", 0, 0, 0), 
                                        new AutomatedPlayer("auto", 0, 0, null, 0), 
                                        false)
        {
            Owner = this,
        };
        if (pickTrump.ShowDialog() == true)
        {
            string message = "Selected to pass.";
            if (pickTrump.SelectedSuit != null)
            {
                message = $"Selected {pickTrump.SelectedSuit}" + $"{(pickTrump.GoAlone ? " alone" : "")}.";
            }
            DialogBoxes.InformationDialog(message, this);
        }
    }

    private void menuChooseCard_Click(object sender, RoutedEventArgs e)
    {
        List<Card> playerCards = new()
        {
            new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Clubs, Rank.Ace),
            new Card(Suit.Clubs, Rank.King),
            new Card(Suit.Spades, Rank.Ace),
            new Card(Suit.Spades, Rank.King),
        };

        ChooseDiscardWindow cardWindow = new(playerCards, new Card(Suit.Clubs, Rank.Nine))
        {
            Owner = this
        };
        if (cardWindow.ShowDialog() == true)
        {
            DialogBoxes.InformationDialog($"Card selected was {cardWindow.ChosenCard}.", this);
        }
    }

    private void menuChooseCards_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MenuToolsOptions_Click(object sender, RoutedEventArgs e)
    {
        OptionsWindow optionsWindow = new OptionsWindow()
        {
            Owner = this
        };
        optionsWindow.ShowDialog();
    }

    private void MenuHelpAbout_Click(object sender, RoutedEventArgs e)
    {

    }

    #endregion Menu EventHandlers
}