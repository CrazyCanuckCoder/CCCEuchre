using Euchre.Logic;
using Euchre.Logic.Components;

namespace Euchre.Console;

public class Program
{
    public static void Main(string[] args)
    {
        var playerNames = new[] { "Alice", "Bob", "Charlie", "Diana" };
        var game = new EuchreGame(playerNames);
        game.PlayGame();
    }
}