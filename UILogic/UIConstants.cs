using System.Windows.Media;

namespace Euchre.UILogic;

public static class UIConstants
{
    /// <summary>
    /// The total number of avatars the user can choose from.
    /// </summary>
    public const int MaximumNumberOfAvatars = 24;

    /// <summary>
    /// The colour that represents the players in team 1.
    /// </summary>
    public static Color Team1Colour { get; } = Colors.Green;

    /// <summary>
    /// The colour that represents the players in team 2.
    /// </summary>
    public static Color Team2Colour { get; } = Colors.Red;
}
