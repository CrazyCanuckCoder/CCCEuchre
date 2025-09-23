using Euchre.Logic.Components;
using Euchre.Logic.Helpers;

namespace Euchre.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (GameDataManager.DataExists())
        {
            // If so, load the saved game.

            var game = new EuchreGame();
            await game.RestartGameAsync();
        }
        else
        {
            List<string> playerNames = ["Stephen", "Alice", "Charlie", "Diana"];
            var game = new EuchreGame(playerNames);
            await game.PlayGameAsync();
        }
    }
}