using Euchre.Logic.Components;

namespace Euchre.Logic.EventArgs;

public class PromptForTrumpSuitEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptForTrumpSuitEventArgs"/> class.
    /// </summary>
    /// <param name="kitty">The card in the kitty that is being considered for calling trump.</param>
    public PromptForTrumpSuitEventArgs(Card kitty)
    {
        Kitty = kitty;
    }

    /// <summary>
    /// Gets the card in the kitty that is being considered for calling trump.
    /// </summary>
    public Card Kitty { get; set; }

    /// <summary>
    /// Gets or sets the suit that the player has chosen as trump.
    /// </summary>
    public Suit? TrumpSuit { get; set; }

    /// <summary>
    /// Indicates whether the player is going alone.
    /// </summary>
    public bool GoAlone { get; set; } = false;
}