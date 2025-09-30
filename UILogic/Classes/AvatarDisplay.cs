using System.Windows.Controls;

namespace Euchre.UILogic.Classes;

/// <summary>
/// Used by the AvatarUserControl to display an avatar image.
/// </summary>
public class AvatarDisplay
{
    /// <summary>
    /// The image associated with the avatar number.
    /// </summary>
    public Image ImageData { get; set; }

    /// <summary>
    /// The number associated to an image of an avatar.
    /// </summary>
    public int AvatarNumber { get; set; }
}