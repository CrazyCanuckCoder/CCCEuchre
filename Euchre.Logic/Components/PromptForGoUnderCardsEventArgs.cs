namespace Euchre.Logic.Components;

public class PromptForGoUnderCardsEventArgs : System.EventArgs
{
    public PromptForGoUnderCardsEventArgs()
    {
    }

    /// <summary>
    /// Gets or sets the list of cards the player has chosen to discard from their hand in exchange for the 
    /// 3 cards in the kitty when going under.
    /// </summary>
    public List<Card> DiscardCards { get; set; } = [];
}