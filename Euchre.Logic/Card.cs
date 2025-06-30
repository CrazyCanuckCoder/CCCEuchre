using Euchre.Logic.Interfaces;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

public class Card : ICard
{
    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    /// <summary>
    /// Gets the suit of the card, such as Hearts, Diamonds, Clubs, or Spades.
    /// </summary>
    public Suit Suit { get; }

    /// <summary>
    /// Gets the rank associated with the card.
    /// </summary>
    public Rank Rank { get; }

    /// <summary>
    /// Returns true or false indicating whether the card's rank is Jack.
    /// </summary>
    public bool IsJack => Rank == Rank.Jack;

    /// <summary>
    /// Gets the value used to shuffle cards in a deck.
    /// </summary>
    public int ShuffleValue { get; internal set; }

    /// <summary>
    /// Determines whether the card is the right bower in the specified trump suit.
    /// </summary>
    /// <param name="trump">The trump suit to evaluate against.</param>
    /// <returns><see langword="true"/> if the card is a Jack and matches the specified trump suit; 
    /// otherwise, <see langword="false"/>.</returns>
    public bool IsRightBower(Suit trump) => IsJack && Suit == trump;

    /// <summary>
    /// Determines whether the card is the left bower in the specified trump suit.
    /// </summary>
    /// <remarks>A card is considered the left bower if it is a Jack and belongs to the suit of the same 
    /// colour as the specified trump suit (e.g., Diamonds for Hearts, Spades for Clubs).</remarks>
    /// <param name="trump">The trump suit to evaluate against.</param>
    /// <returns><see langword="true"/> if the card is the left bower for the specified trump suit; 
    /// otherwise, <see langword="false"/>.</returns>
    public bool IsLeftBower(Suit trump)
    {
        if (!IsJack) return false;
        return trump switch
        {
            Suit.Hearts => Suit == Suit.Diamonds,
            Suit.Diamonds => Suit == Suit.Hearts,
            Suit.Clubs => Suit == Suit.Spades,
            Suit.Spades => Suit == Suit.Clubs,
            _ => false
        };
    }

    /// <summary>
    /// Determines whether the current card is a Bower (either the Right Bower or the Left Bower) based on 
    /// the specified trump suit.
    /// </summary>
    /// <param name="trump">The trump suit used to evaluate whether the card is a Bower.</param>
    /// <returns><see langword="true"/> if the card is either the Right Bower or the Left Bower for the 
    /// specified trump suit; otherwise, <see langword="false"/>.</returns>
    public bool IsBower(Suit trump) => IsRightBower(trump) || IsLeftBower(trump);

    /// <summary>
    /// Determines the effective suit of the card, taking into account the trump suit.  If the card is the 
    /// left bower of the current trump suit, it is considered to be of the trump suit.
    /// </summary>
    /// <param name="trump">The trump suit to evaluate against.</param>
    /// <returns>The effective suit of the card. If the card is the left bower, the method returns the trump 
    /// suit; otherwise, it returns the card's original suit.</returns>
    public Suit EffectiveSuit(Suit trump)
    {
        if (IsLeftBower(trump)) return trump;
        return Suit;
    }

    /// <summary>
    /// Determines the value of the card in the context of a trick, based on the trump suit and the lead suit.
    /// </summary>
    /// <remarks>This method evaluates the card's trick value based on its relationship to the trump and lead
    /// suits. Cards that are neither trump nor lead suit are assigned a value of 0, as they cannot win the
    /// trick.</remarks>
    /// <param name="trump">The suit designated as trump for the current round.</param>
    /// <param name="leadSuit">The suit that was led in the current trick.</param>
    /// <returns>The value of the card for the trick: <list type="bullet"> <item><description><see langword="RIGHT_BOWER_VALUE"/>
    /// if the card is the right bower of the trump suit.</description></item> <item><description><see
    /// langword="LEFT_BOWER_VALUE"/> if the card is the left bower of the trump suit.</description></item>
    /// <item><description>The trump value plus the rank of the card if the card matches the trump
    /// suit.</description></item> <item><description>The rank of the card if the card matches the lead
    /// suit.</description></item> <item><description><see langword="0"/> if the card does not match the trump suit or
    /// the lead suit.</description></item> </list></returns>
    public int GetTrickValue(Suit trump, Suit leadSuit)
    {
        if (IsRightBower(trump)) return RIGHT_BOWER_VALUE;
        if (IsLeftBower(trump)) return LEFT_BOWER_VALUE;

        if (Suit == trump) return TRUMP_VALUE_ADD + (int)Rank;
        if (Suit == leadSuit) return (int)Rank;

        return 0; // Can't win if not trump or lead suit
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
