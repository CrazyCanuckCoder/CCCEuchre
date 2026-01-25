using Euchre.Logic.Helpers;

namespace Euchre.Logic.Interfaces;

internal interface IDataManager
{
    static abstract IGameStateManager LoadGameState();
    static abstract void AddPlayersToGameState(IGameStateManager gameStateManager);
    static abstract void SaveGameState(IGameStateManager gameStateManager);
}