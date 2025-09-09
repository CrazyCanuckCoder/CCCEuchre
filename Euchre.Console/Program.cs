using Euchre.Logic.Components;

namespace Euchre.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        var playerNames = new[] { "Alice", "Bob", "Charlie", "Diana" };
        var game = new EuchreGame(playerNames);
        await game.PlayGameAsync();
    }
}