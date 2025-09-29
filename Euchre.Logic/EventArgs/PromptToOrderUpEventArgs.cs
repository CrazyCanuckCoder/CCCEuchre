using Euchre.Logic.Components;

namespace Euchre.Logic.EventArgs;

public class PromptToOrderUpEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptToOrderUpEventArgs"/> class.
    /// </summary>
    /// <param name="kitty">The card in the kitty that is being considered for ordering up.</param>
    public PromptToOrderUpEventArgs(Card kitty)
    {
        Kitty = kitty;
    }

    /// <summary>
    /// Gets the card in the kitty that is being considered for ordering up.
    /// </summary>
    public Card Kitty { get; set; }

    /// <summary>
    /// Indicates whether the player has ordered up the card.
    /// </summary>
    public bool OrderedUp { get; set; } = false;

    /// <summary>
    /// Indicates whether the player is going alone.
    /// </summary>
    public bool GoAlone { get; set; } = false;
}