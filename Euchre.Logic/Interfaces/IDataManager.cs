using Euchre.Logic.Helpers;

namespace Euchre.Logic.Interfaces;

public interface IDataManager
{
    IGameStateManager LoadGameState();
    void AddPlayersToGameState(IGameStateManager gameStateManager);
    void SaveGameState(IGameStateManager gameStateManager);
}