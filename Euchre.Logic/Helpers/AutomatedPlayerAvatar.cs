namespace Euchre.Logic.Helpers;

/// <summary>
/// Tracks which avatar image is associated to an automated player.
/// </summary>
public class AutomatedPlayerAvatar
{
    /// <summary>
    /// The name for the automated player.
    /// </summary>
    public string PlayerName { get; set; } = string.Empty;

    /// <summary>
    /// The number associated to an image for the automated player.
    /// </summary>
    public int AvatarNumber { get; set; }

    /// <summary>
    /// Creates a brand new copy of the current AutomatedPlayerAvatar.
    /// </summary>
    /// <returns>An AutomatedPlayerAvatar with the same values as the current object.</returns>
    public AutomatedPlayerAvatar Clone()
    {
        return new AutomatedPlayerAvatar()
        {
            PlayerName = PlayerName,
            AvatarNumber = AvatarNumber
        };
    }
}
