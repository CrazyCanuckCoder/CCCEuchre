using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic.Interfaces;
using Euchre.UserControls;
using System.Windows;

namespace Euchre.UILogic.Classes;

internal class MainWindowController
{
    public MainWindowController(MainWindow mainWindow, GameStateManager gameStateManager)
    {
        _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
        _gameStateManager = gameStateManager ?? throw new ArgumentNullException(nameof(gameStateManager));
        Initialize();
    }

    /// <summary>
    /// A reference to the main window.
    /// </summary>
    private readonly MainWindow _mainWindow;

    /// <summary>
    /// A reference to the game state manager of the current game.
    /// </summary>
    private readonly GameStateManager _gameStateManager;

    /// <summary>
    /// Used for setting the visibility properties for each of the player cards user controls.
    /// </summary>
    private Action<Visibility>? _visibilitySetter;

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

    /// <summary>
    /// Establishes an action to be used to set the display of a player's card control.
    /// </summary>
    /// <param name="controlIdx">The index of the player's control.</param>
    /// <param name="visibilityAction">The action to use to set the control's visibility.</param>
    public void SetPlayerCardDisplayControlVisibility(int controlIdx, Action<Visibility> visibilityAction)
    {
        _playerCardDisplayControlsVisibility.Add(controlIdx, visibilityAction);
    }

    /// <summary>
    /// Hides or shows the dealer icon for a specified player.
    /// </summary>
    /// <param name="playerIndex">The index of the player.</param>
    /// <param name="isVisible">True to show the icon and false to hide it.</param>
    public void SetPlayerDealerIconVisibility(int playerIndex, bool isVisible)
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
    /// Updates the player's dealt cards control with the number of cards.
    /// </summary>
    /// <param name="indexOfPlayer">The player's index.</param>
    /// <param name="numberOfCardsDealt">The number of cards to display on the control.</param>
    public void DealCardsToPlayer(int indexOfPlayer, int numberOfCardsDealt)
    {
        _playerDealtCardsDisplayControls[indexOfPlayer].DisplayCards(numberOfCardsDealt);
    }

    /// <summary>
    /// Clears the player's dealt cards control.
    /// </summary>
    /// <param name="indexOfPlayer">The player's index.</param>
    public void ClearDealtCards(int indexOfPlayer)
    {
        _playerDealtCardsDisplayControls[indexOfPlayer].ClearCards();
    }

    /// <summary>
    /// Displays the kitty card on the interface.
    /// </summary>
    /// <param name="kitty">The card at the top of the kitty pile.</param>
    public void DisplayKittyCard(Card kitty)
    {
        _mainWindow.TrumpDisplayUserControl.SetKittyCard(kitty);
    }

    /// <summary>
    /// Clears the image displayed as either the kitty card or the trump suit.
    /// </summary>
    public void ClearTrumpDisplay()
    {
        _mainWindow.TrumpDisplayUserControl.ClearImage();
    }

    /// <summary>
    /// Sets the visibility of each player's card display control to visible.
    /// </summary>
    public void ResetCardDisplayControlsVisibility()
    {
        for (int playerIndex = 0; playerIndex < Constants.NUMBER_OF_PLAYERS; playerIndex++)
        {
            if (_playerCardDisplayControlsVisibility.ContainsKey(playerIndex))
            {
                switch (playerIndex)
                {
                    case 0:
                        _playerCardDisplayControls[playerIndex] = _mainWindow.Player1CardDisplayUserControl;
                        break;

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
    /// Clears the controls that displayed the cards played for the current round.
    /// </summary>
    public void ClearPlayedCards()
    {
        for (int index = 0; index < _playerPlayedCardsDisplayControls.Count; index++)
        {
            _playerPlayedCardsDisplayControls[index].ClearCards();
        }
    }

    /// <summary>
    /// Blanks out the cards in each player's card display control to get the control ready to receive new
    /// cards for the next round.
    /// </summary>
    public void ClearPlayersHands()
    {
        foreach (var playersHandsControl in _playerCardDisplayControls.Values)
        {
            playersHandsControl?.ClearCards();
        }
    }

    /// <summary>
    /// Displays the trump suit on the main window.
    /// </summary>
    /// <param name="trump">The suit to display as trump.</param>
    public void SetTrump(Suit trump)
    {
        _mainWindow.TrumpDisplayUserControl.SetTrump(trump);
    }

    /// <summary>
    /// Displays the bidding information on the main form.
    /// </summary>
    /// <param name="player">The player who called the bid.</param>
    /// <param name="trump">The suit called as trump.</param>
    /// <param name="isGoingAlone">True to indicate the player is going alone.</param>
    public void SetBidInformation(IPlayer player, Suit trump, bool isGoingAlone)
    {
        _mainWindow.CurrentRoundInfoUserControl.SetBidInformation(player, trump, isGoingAlone);
    }

    /// <summary>
    /// Sets the icon indicating the trump a player called.
    /// </summary>
    /// <param name="playerIndex">The index in the list of players for the player.</param>
    /// <param name="trump">The suit called as trump.</param>
    public void SetPlayerTrumpSuitIcon(int playerIndex, Suit trump)
    {
        switch (playerIndex)
        {
            case 0:
                _mainWindow.Player1DisplayUserControl.SetTrumpSuit(trump);
                break;

            case 1:
                _mainWindow.Player2DisplayUserControl.SetTrumpSuit(trump);
                break;

            case 2:
                _mainWindow.Player3DisplayUserControl.SetTrumpSuit(trump);
                break;

            case 3:
                _mainWindow.Player4DisplayUserControl.SetTrumpSuit(trump);
                break;
        }
    }

    /// <summary>
    /// Sets a player's partner's hand to disabled in the user interface to indicate the player is going
    /// alone.
    /// </summary>
    /// <param name="player">The player going alone.</param>
    public void SetPartnerDisabled(IPlayer player)
    {
        var partner = (  from anyPlayer in _gameStateManager.Players
                        where anyPlayer.TeamIndex == player.TeamIndex
                           && anyPlayer.PlayerIndex != player.PlayerIndex
                       select anyPlayer)
                      .First();
        if (partner is HumanPlayer humanPlayer)
        {
            _playerCardDisplayControls[humanPlayer.PlayerIndex].DisableCards(humanPlayer.Hand,
                _gameStateManager.Trump!.Value);
        }
        else
        {
#if DEBUG
            _playerCardDisplayControls[partner.PlayerIndex].DisableCards(partner.Hand,
                _gameStateManager.Trump!.Value);
#else
            _playerCardDisplayControls[partner.PlayerIndex].DisableCards(partner.Hand.Count);
#endif
        }
    }

    /// <summary>
    /// Redisplays the cards in the player's hand based on the current trump suit.
    /// </summary>
    /// <param name="player">The player containing the hand to update.</param>
    public void SetupPlayerCards(IPlayer player)
    {
        if (_gameStateManager.Trump == null)
        {
            _playerCardDisplayControls[player.PlayerIndex].SetupCards(player.Hand);
        }
        else
        {
            _playerCardDisplayControls[player.PlayerIndex].SetupCards(player.Hand,
                _gameStateManager.Trump!.Value);
        }
    }

    /// <summary>
    /// Displays the card on the main form for a specified player.
    /// </summary>
    /// <param name="playerIndex">The index of the player in the list of players.</param>
    /// <param name="cardPlayed">The card played by the player.</param>
    public void DisplayPlayedCard(int playerIndex, ICard cardPlayed)
    {
        _playerPlayedCardsDisplayControls[playerIndex].SetupCards([(Card)cardPlayed]);
    }

    public void UpdatePlayersNumberOfTricksWon(int playerIndex)
    {
        // Update the trick winner's number of tricks won on the display for the user and the list of tricks
        //  for each player for the game.

        switch (playerIndex)
        {
            case 0:
                _mainWindow.Player1DisplayUserControl.UpdateNumberOfTricks(
                    _gameStateManager.TricksWonByPlayers[playerIndex]);
                break;

            case 1:
                _mainWindow.Player2DisplayUserControl.UpdateNumberOfTricks(
                    _gameStateManager.TricksWonByPlayers[playerIndex]);
                break;

            case 2:
                _mainWindow.Player3DisplayUserControl.UpdateNumberOfTricks(
                    _gameStateManager.TricksWonByPlayers[playerIndex]);
                break;

            case 3:
                _mainWindow.Player4DisplayUserControl.UpdateNumberOfTricks(
                    _gameStateManager.TricksWonByPlayers[playerIndex]);
                break;
        }
    }

    /// <summary>
    /// Updates the player score display with the scores for each team.
    /// </summary>
    public void UpdateGameScores()
    {
        _mainWindow.CurrentPlayersScoresUserControl.UpdateTeamScore(1, _gameStateManager.TeamScores[0]);
        _mainWindow.CurrentPlayersScoresUserControl.UpdateTeamScore(2, _gameStateManager.TeamScores[1]);
    }

    /// <summary>
    /// Resets each player's number of tricks won to zero.
    /// </summary>
    public void ResetTrickCounters()
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
    /// Clears the trump suit icon for a specified player.
    /// </summary>
    /// <param name="playerIndex">The index in the list of players for the player.</param>
    public void ResetPlayerTrumpSuitIcon(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                _mainWindow.Player1DisplayUserControl.ClearTrumpSuit();
                break;

            case 1:
                _mainWindow.Player2DisplayUserControl.ClearTrumpSuit();
                break;

            case 2:
                _mainWindow.Player3DisplayUserControl.ClearTrumpSuit();
                break;

            case 3:
                _mainWindow.Player4DisplayUserControl.ClearTrumpSuit();
                break;
        }
    }

    /// <summary>
    /// Resets the list of played tricks, what suit was trump, and the bidding information for the round.
    /// </summary>
    public void ResetTricksTrumpAndBidInformation()
    {
        _mainWindow.PreviousTricksUserControl.ClearTricks();
        _mainWindow.TrumpDisplayUserControl.ClearImage();
        _mainWindow.CurrentRoundInfoUserControl.ResetBidInformation();
    }

    /// <summary>
    /// Clears the score tracking control of all text.
    /// </summary>
    public void ClearScoresAndBidInformation()
    {
        _mainWindow.CurrentPlayersScoresUserControl.Reset();
        _mainWindow.CurrentRoundInfoUserControl.Clear();
    }

    /// <summary>
    /// Gets the card from a human player.
    /// </summary>
    /// <param name="user">The human player to obtain the card from.</param>
    /// <param name="trickSuit">The suit lead in the current trick.</param>
    /// <returns>The card chosen by the player.</returns>
    public Card? GetCardFromUser(IPlayer user, Suit? trickSuit)
    {
        Card? chosenCard = null;

        // Get the card from the user.

        if (_playerCardDisplayControls[user.PlayerIndex] is CardDisplayUserControl playerControl)
        {
            playerControl.GetCardFromUser(user.Hand, trickSuit, trickSuit! == _gameStateManager.Trump!.Value, 
                _gameStateManager.Trump.Value);
            chosenCard = playerControl.ChosenCard;
        }

        return chosenCard;
    }

    /// <summary>
    /// Adds a specified trick to the control tracking tricks for the current round.
    /// </summary>
    /// <param name="trick">The trick to add to the list of tricks that have been played.</param>
    public void AddTrick(Trick trick)
    {
        _mainWindow.PreviousTricksUserControl.AddTrick(trick);
    }

    /// <summary>
    /// Resets each player's number of tricks to the stored values.
    /// </summary>
    public void UpdatePlayersTrickCounters()
    {
        foreach (var player in _gameStateManager.Players!)
        {
            switch (player.PlayerIndex)
            {
                case 0:
                    _mainWindow.Player1DisplayUserControl.UpdateNumberOfTricks(
                        _gameStateManager.TricksWonByPlayers[player.PlayerIndex]);
                    break;

                case 1:
                    _mainWindow.Player2DisplayUserControl.UpdateNumberOfTricks(
                        _gameStateManager.TricksWonByPlayers[player.PlayerIndex]);
                    break;

                case 2:
                    _mainWindow.Player3DisplayUserControl.UpdateNumberOfTricks(
                        _gameStateManager.TricksWonByPlayers[player.PlayerIndex]);
                    break;

                case 3:
                    _mainWindow.Player4DisplayUserControl.UpdateNumberOfTricks(
                        _gameStateManager.TricksWonByPlayers[player.PlayerIndex]);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets up the references to the controls on the form and initializes some of the controls.
    /// </summary>
    private void Initialize()
    {
        SetupCurrentRoundInfoControl();
        SetupCurrentScoreControl();
        InitializeCardDisplayControlCollection();
        SetupPlayerDisplayControls();
    }

    /// <summary>
    /// Adds the players names to the correct teams on the current round information user control.
    /// </summary>
    private void SetupCurrentRoundInfoControl()
    {
        _mainWindow.CurrentRoundInfoUserControl.Team1List.Add(_gameStateManager.Players![0]);
        _mainWindow.CurrentRoundInfoUserControl.Team1List.Add(_gameStateManager.Players[2]);
        _mainWindow.CurrentRoundInfoUserControl.Team2List.Add(_gameStateManager.Players[1]);
        _mainWindow.CurrentRoundInfoUserControl.Team2List.Add(_gameStateManager.Players[3]);
        if (_gameStateManager.TrumpCaller != null)
        {
            _mainWindow.CurrentRoundInfoUserControl.SetBidInformation(_gameStateManager.TrumpCaller,
                _gameStateManager.Trump!.Value, _gameStateManager.TrumpCaller.IsGoingAlone);
        }
    }

    /// <summary>
    /// Initializes the control showing the teams scores.
    /// </summary>
    private void SetupCurrentScoreControl()
    {
        _mainWindow.CurrentPlayersScoresUserControl.SetTeamNames(
            (from player in _gameStateManager.Players!
             orderby player.PlayerIndex
             select player.Name).ToList()
            );
        _mainWindow.CurrentPlayersScoresUserControl.UpdateTeamScore(1, _gameStateManager.TeamScores[0]);
        _mainWindow.CurrentPlayersScoresUserControl.UpdateTeamScore(2, _gameStateManager.TeamScores[1]);
    }

    /// <summary>
    /// Updates the collection that references each player's card display control and the collection that
    /// references each player's cards that are dealt to them.
    /// </summary>
    private void InitializeCardDisplayControlCollection()
    {
        if (_playerCardDisplayControls.Count == 0)
        {
            _playerCardDisplayControls.Add(0, _mainWindow.Player1CardDisplayUserControl);
            _playerCardDisplayControls.Add(1, _mainWindow.Player2CardDisplayUserControl);
            _playerCardDisplayControls.Add(2, _mainWindow.Player3CardDisplayUserControl);
            _playerCardDisplayControls.Add(3, _mainWindow.Player4CardDisplayUserControl);

            _playerDealtCardsDisplayControls.Add(0, _mainWindow.Player1DealtCardsDisplayUserControl);
            _playerDealtCardsDisplayControls.Add(1, _mainWindow.Player2DealtCardsDisplayUserControl);
            _playerDealtCardsDisplayControls.Add(2, _mainWindow.Player3DealtCardsDisplayUserControl);
            _playerDealtCardsDisplayControls.Add(3, _mainWindow.Player4DealtCardsDisplayUserControl);

            _playerPlayedCardsDisplayControls.Add(0, _mainWindow.Player1PlayedCardsDisplayUserControl);
            _playerPlayedCardsDisplayControls.Add(1, _mainWindow.Player2PlayedCardsDisplayUserControl);
            _playerPlayedCardsDisplayControls.Add(2, _mainWindow.Player3PlayedCardsDisplayUserControl);
            _playerPlayedCardsDisplayControls.Add(3, _mainWindow.Player4PlayedCardsDisplayUserControl);
        }
    }

    /// <summary>
    /// Sets each player's name and avatar on the main window.
    /// </summary>
    private void SetupPlayerDisplayControls()
    {
        _mainWindow.Player1DisplayUserControl.SetActivePlayer(_gameStateManager.Players![0],
            _gameStateManager.Players[0].AvatarNumber);
        _mainWindow.Player2DisplayUserControl.SetActivePlayer(_gameStateManager.Players[1],
            _gameStateManager.Players[1].AvatarNumber);
        _mainWindow.Player3DisplayUserControl.SetActivePlayer(_gameStateManager.Players[2],
            _gameStateManager.Players[2].AvatarNumber);
        _mainWindow.Player4DisplayUserControl.SetActivePlayer(_gameStateManager.Players[3],
            _gameStateManager.Players[3].AvatarNumber);
    }
}
