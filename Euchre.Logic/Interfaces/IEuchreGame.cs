using Euchre.Logic.EventArgs;
using Euchre.Logic.Interfaces;

namespace Euchre.Logic.Components;

public interface IEuchreGame
{
    IGameStateManager GameInfo { get; }

    event EventHandler<CardPlayedByPlayerEventArgs>? CardPlayedByPlayer;
    event EventHandler<CardsDealtToPlayerEventArgs>? CardsDealtToPlayer;
    event EventHandler<DeclareDealerEventArgs>? DeclareDealer;
    event EventHandler<DeclareKittyCardEventArgs>? DeclareKittyCard;
    event EventHandler<DeclareRoundWinningPlayersEventArgs>? DeclareRoundWinningPlayers;
    event EventHandler<DeclareTrickWinnerEventArgs>? DeclareTrickWinner;
    event EventHandler<GameOverEventArgs>? GameOver;
    event EventHandler<System.EventArgs>? KittyWasTurnedDown;
    event EventHandler<System.EventArgs>? NoTrumpCalled;
    event EventHandler<PlayerBidEventArgs>? PlayerBidResult;
    event EventHandler<PlayerCanTakeRemainingTricksEventArgs>? PlayerCanTakeRemainingTricks;
    event EventHandler<System.EventArgs>? TrumpCalled;

#if DEBUG
    event EventHandler<GetPlayersCardsEventArgs>? GetPlayersCards;
    event EventHandler<PromptToChooseCardsForPlayersEventArgs>? PromptToChooseCardsForPlayers;
    event EventHandler<System.EventArgs>? UpdatePlayersHands;
#endif

    Task PlayGameAsync(CancellationToken externalToken = default);
    void RequestStop();
}