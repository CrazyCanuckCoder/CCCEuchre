using Euchre.Logic.Components;
using Euchre.Logic.Helpers;

namespace Euchre.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (GameStateManager.DataExists())
        {
            // If so, load the saved game.

            var game = new EuchreGame();
            await game.PlayGameAsync();
        }
        else
        {
            List<AutomatedPlayerAvatar> playerNames = 
                [
                    new AutomatedPlayerAvatar() { PlayerName = "Stephen", AvatarNumber = 0 },
                    new AutomatedPlayerAvatar() { PlayerName = "Alice",   AvatarNumber = 0 },
                    new AutomatedPlayerAvatar() { PlayerName = "Charlie", AvatarNumber = 0 },
                    new AutomatedPlayerAvatar() { PlayerName = "Diana",   AvatarNumber = 0 }
                ];
            var game = new EuchreGame(playerNames);
            await game.PlayGameAsync();
        }
    }
}