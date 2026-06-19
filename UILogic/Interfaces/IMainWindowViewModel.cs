using Euchre.Logic.Components;
using Euchre.Logic.Enums;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using System.Windows;

namespace Euchre.UILogic.Interfaces;

public interface IMainWindowViewModel
{
    Visibility ContinueMenuVisibility { get; set; }
    IEuchreGame? CurrentGame { get; set; }
    Visibility GameBoardVisibility { get; set; }
    string GameInformation { get; set; }
    Visibility GameInformationVisibility { get; set; }
    Visibility IconMenuVisibility { get; set; }
    Visibility Player1CardDisplayUserControlVisibility { get; set; }
    Visibility Player1DealtCardsDisplayUserControlVisibility { get; set; }
    Visibility Player1PlayedCardsDisplayUserControlVisibility { get; set; }
    string Player1Text { get; set; }
    Visibility Player1TextVisibility { get; set; }
    Visibility Player2CardDisplayUserControlVisibility { get; set; }
    Visibility Player2DealtCardsDisplayUserControlVisibility { get; set; }
    Visibility Player2PlayedCardsDisplayUserControlVisibility { get; set; }
    string Player2Text { get; set; }
    Visibility Player2TextVisibility { get; set; }
    Visibility Player3CardDisplayUserControlVisibility { get; set; }
    Visibility Player3DealtCardsDisplayUserControlVisibility { get; set; }
    Visibility Player3PlayedCardsDisplayUserControlVisibility { get; set; }
    string Player3Text { get; set; }
    Visibility Player3TextVisibility { get; set; }
    Visibility Player4CardDisplayUserControlVisibility { get; set; }
    Visibility Player4DealtCardsDisplayUserControlVisibility { get; set; }
    Visibility Player4PlayedCardsDisplayUserControlVisibility { get; set; }
    string Player4Text { get; set; }
    Visibility Player4TextVisibility { get; set; }
    bool PlayLastCardInHand { get; set; }
    Visibility StandardMenuVisibility { get; set; }

    void DealCardsToPlayer(int indexOfPlayer, int numberOfCardsDealt);
    void DeclareDealer(IPlayer dealer);
    void DisplayCardPlayedByPlayer(IPlayer player, ICard cardPlayed);
    void EndOfGameUpdate(IGameStateManager gameInfo);
    void EndOfRoundUpdate(List<string> winningPlayers, int points, ScoringReason reasonForPoints);
    void EndOfTrick(IPlayer trickWinningPlayer);
    Card? GetCardFromUser(IPlayer user, Suit? trickSuit, Suit trump);
    void ShowNoAceNoFaceNoTrump(IPlayer player);
    void Initialize();
    void KittyWasTurnedDown();
    void NoTrumpWasCalled();
    void PlayerMadeTrump(IPlayer player, Suit? trump, bool isGoingAlone, bool isKittyRound);
    void PlayerPassed(IPlayer player, bool isKittyRound, bool wentUnder);
    void PlayerTakesRemainingTricks(IPlayer playerToTakeTricks);
    Card? PromptUserForDiscard(HumanPlayer player);
    Suit? PromptUserForTrump(Card kitty, HumanPlayer player, out bool goAlone);
    bool PromptUserToOrderUp(Card kitty, HumanPlayer player, out bool goAlone, out bool goUnder);
    void RedisplayUserHand(HumanPlayer player);
    void SetKittyCard(Card kitty);
    void SetupUserInterface();
    void TrumpCalled();
    bool PromptUserToInvokeNoAceNoFaceNoTrumpRule(HumanPlayer player);
    void UpdatePlayersHandAfterDeal(int indexOfPlayer, int numberOfCardsDealt);
    List<Card> PromptUserForGoUnderCards(HumanPlayer player);
}