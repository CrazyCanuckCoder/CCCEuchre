using Euchre.Logic.Components;

namespace Euchre.Logic.EventArgs;

public class PromptForDiscardEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptForDiscardEventArgs"/> class.
    /// </summary>
    /// <param name="kitty">The card in the kitty that is being considered for discarding.</param>
    public PromptForDiscardEventArgs(Card kitty)
    {
        Kitty = kitty;
    }
    
    /// <summary>
    /// The kitty card to be swapped.
    /// </summary>
    public Card Kitty { get; set; }
    
    /// <summary>
    /// Gets or sets the card that the player has chosen to discard.
    /// </summary>
    public Card? DiscardedCard { get; set; }
}