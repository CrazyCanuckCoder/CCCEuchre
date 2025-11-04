using System.Windows;
using System.Windows.Controls;

namespace Euchre.UILogic.Classes;

/// <summary>
/// The base class to use for displaying a playing card's image on a form or user control.
/// </summary>
public class BaseCardDisplay
{
    /// <summary>
    /// Gets/set the image file for the card.
    /// </summary>
    public Image ImageData { get; set; } = new Image();

    /// <summary>
    /// Gets/sets the visibility state for the card.
    /// </summary>
    public Visibility CardVisibility { get; set; } = Visibility.Visible;

    /// <summary>
    /// Gets/sets the margin for the card.
    /// </summary>
    public Thickness CardMargin { get; set; }
}
