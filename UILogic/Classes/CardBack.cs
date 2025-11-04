using System;

namespace Euchre.UILogic.Classes;

/// <summary>
/// The class to hold the information for a card back image.
/// </summary>
public class CardBack : BaseCardDisplay
{
    public CardBack(string cardFileName, bool isSelected)
    {
        CardFileName = cardFileName ?? throw new ArgumentNullException(nameof(cardFileName));
        IsSelected = isSelected;
    }

    /// <summary>
    /// The file name for the card back image.
    /// </summary>
    public string CardFileName { get; set; } = string.Empty;

    /// <summary>
    /// True to indicate the card back is the selected one to use in the game.
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// The string name of the image file for the card back.
    /// </summary>
    public string ImageName { get; set; } = string.Empty;
}
