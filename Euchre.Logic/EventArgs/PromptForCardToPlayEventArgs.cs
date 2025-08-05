namespace Euchre.Logic.EventArgs;

public class PromptForCardToPlayEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptForCardToPlayEventArgs"/> class.
    /// </summary>
    /// <param name="hand">The player's hand from which to choose a card to play.</param>
    public PromptForCardToPlayEventArgs(Suit trump, Suit? leadSuit)
    {
        PlayedCard = null;
        Trump = trump;
        LeadSuit = leadSuit;
    }

    /// <summary>
    /// The suit designated as trump for the current trick.
    /// </summary>
    public Suit Trump { get; set; }

    /// <summary>
    /// The suit that was led in the current trick, or <see langword="null"/> if no suit has been led yet.
    /// </summary>
    public Suit? LeadSuit { get; set; }

    /// <summary>
    /// Gets or sets the card that the player has chosen to play.
    /// </summary>
    public Card? PlayedCard { get; set; }
}