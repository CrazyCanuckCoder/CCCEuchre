using Euchre.Logic.Components;
using System.Windows;

namespace Euchre.UILogic.Classes;

/// <summary>
/// A class to use for displaying a playing card's image on a form or user control.
/// </summary>
public class CardDisplay : BaseCardDisplay
{
    /// <summary>
    /// Gets/sets the enabled state for the card.
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Gets/sets the representation of a playing card.
    /// </summary>
    public Card? Card { get; set; }

    /// <summary>
    /// Gets/sets the visibility status of the image for the card when it's disabled.
    /// </summary>
    public Visibility ImageVisibility { get; set; }
}
