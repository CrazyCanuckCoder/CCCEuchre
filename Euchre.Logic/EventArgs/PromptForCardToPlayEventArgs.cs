using Euchre.Logic.Components;

namespace Euchre.Logic.EventArgs;

public class PromptForCardToPlayEventArgs : System.EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptForCardToPlayEventArgs"/> class.
    /// </summary>
    /// <param name="trump">The suit designated as trump for the current trick.</param>
    /// <param name="leadSuit">The suit that was led in the current trick, if any.</param>
    /// <param name="trick">The current trick.</param>
    public PromptForCardToPlayEventArgs(Suit trump, Suit? leadSuit, Trick trick)
    {
        PlayedCard = null;
        Trump = trump;
        LeadSuit = leadSuit;
        Trick = trick;
    }

    /// <summary>
    /// The current trick, including the cards already played.
    /// </summary>
    public Trick Trick { get; }

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