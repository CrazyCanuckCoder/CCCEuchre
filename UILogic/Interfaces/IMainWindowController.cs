using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;
using System.Windows;

namespace Euchre.UILogic.Interfaces;

public interface IMainWindowController
{
    void AddPlayerCards(int indexOfPlayer, int numberOfCardsDealt);
    void AddTrick(Trick trick);
    void ClearDealtCards(int indexOfPlayer);
    void ClearPlayedCards();
    void ClearPlayersHands();
    void ClearScoresAndBidInformation();
    void ClearTrumpDisplay();
    void DealCardsToPlayer(int indexOfPlayer, int numberOfCardsDealt);
    void DisplayBidInformation(IPlayer player, Suit trump, bool isGoingAlone);
    void DisplayKittyCard(Card kitty);
    void DisplayPlayedCard(int playerIndex, ICard cardPlayed);
    void DisplayTrump(Suit trump);
    Card? GetCardFromUser(IPlayer user, Suit? trickSuit);
    void RemovePlayerCard(IPlayer player);
    void ResetCardDisplayControlsVisibility();
    void ResetPlayerTrumpSuitIcon(int playerIndex);
    void ResetTrickCounters();
    void ResetTricksTrumpAndBidInformation();
    void SetPartnerDisabled(IPlayer player);
    void SetPlayerCardDisplayControlVisibility(int controlIdx, Action<Visibility> visibilityAction);
    void SetPlayerDealerIconVisibility(int playerIndex, bool isVisible);
    void SetPlayerTrumpSuitIcon(int playerIndex, Suit trump);
    void SetupPlayerCards(IPlayer player);
    void UpdateGameScores();
    void UpdatePlayersNumberOfTricksWon(int playerIndex);
    void UpdatePlayersTrickCounters();
}