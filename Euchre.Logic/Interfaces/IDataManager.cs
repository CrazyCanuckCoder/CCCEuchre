using Euchre.Logic.Helpers;

namespace Euchre.Logic.Interfaces;

internal interface IDataManager
{
    static abstract GameStateManager LoadGameState();
    static abstract void SaveGameState(GameStateManager gameStateManager);
}