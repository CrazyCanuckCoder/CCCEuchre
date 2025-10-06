using Euchre.Logic.Exceptions;
using Euchre.Logic.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for CurrentScoreUserControl.xaml
/// </summary>
public partial class CurrentScoreUserControl : UserControl
{
    public CurrentScoreUserControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for Team1Names.
    /// </summary>
    public static readonly DependencyProperty Team1NamesProperty =
        DependencyProperty.Register(nameof(Team1Names), typeof(string), typeof(CurrentScoreUserControl),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Using a DependencyProperty as the backing store for Team2Names.
    /// </summary>
    public static readonly DependencyProperty Team2NamesProperty =
        DependencyProperty.Register(nameof(Team2Names), typeof(string), typeof(CurrentScoreUserControl),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// The names of the players on Team 1.
    /// </summary>
    public string Team1Names
    {
        get => (string)GetValue(Team1NamesProperty);
        set => SetValue(Team1NamesProperty, value); 
    }

    /// <summary>
    /// The names of the players on Team 2.
    /// </summary>
    public string Team2Names
    {
        get => (string)GetValue(Team2NamesProperty); 
        set => SetValue(Team2NamesProperty, value);
    }

    /// <summary>
    /// Sets the names of the teams from a list of players names.
    /// </summary>
    /// <param name="playerNames">A list of strings containing the names of the players.</param>
    public void SetTeamNames(List<string> playerNames)
    {
        ArgumentNullException.ThrowIfNull(playerNames, nameof(playerNames));
        if (playerNames.Count != Constants.NUMBER_OF_PLAYERS)
        {
            throw new InvalidNumberOfPlayersException();
        }

        Team1Names = $"{playerNames[0]} and {playerNames[3]}";
        Team2Names = $"{playerNames[2]} and {playerNames[4]}";
    }

    /// <summary>
    /// Updates the team score with a specified value.
    /// </summary>
    /// <param name="teamNumber">Specify 1 for Team 1 and 2 for Team 2.</param>
    /// <param name="teamScore">The new score for the specified team.</param>
    public void UpdateTeamScore(int teamNumber, int teamScore)
    {
        if (teamNumber == 1)
        {
            labelledProgressBarUserControlTeam1.ProgressValue = teamScore;
        }
        else
        {
            labelledProgressBarUserControlTeam2.ProgressValue = teamScore;
        }
    }
}
