using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using System.Threading.Tasks;
using System.Windows;

namespace Euchre;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async Task StartGame()
    {
        // Check if there is a game saved that the user wants to continue.

        if (GameDataManager.DataExists())
        {
            // If so, load the saved game.

            EuchreGame game = new();
            await game.RestartGameAsync();
        }
        else
        {
            // If not, start a new game.

            List<string> playerNames = GetPlayerNamesFromUser();
            EuchreGame game = new(playerNames);
            await game.PlayGameAsync();
        }
    }

    private List<string> GetPlayerNamesFromUser()
    {
        return ["Alice", "Bob", "Charlie", "Diana"]; // Placeholder for actual user input
    }
}