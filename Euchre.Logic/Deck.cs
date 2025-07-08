using Euchre.Logic.Exceptions;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

/// <summary>
/// Represents a standard deck of playing cards, providing functionality for shuffling and dealing cards.
/// </summary>
public class Deck
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Deck"/> class, creating a deck of cards for use in the 
    /// game of Euchre.
    /// </summary>
    public Deck()
    {
        _randomizer = new Random();
        InitializeDeck();
    }

    /// <summary>
    /// Represents a random number generator used for shuffling the cards.
    /// </summary>
    private readonly Random _randomizer;

    /// <summary>
    /// Represents the stack of cards used internally by the game.
    /// </summary>
    /// <remarks>This stack is used to manage the collection of cards in a last-in, first-out (LIFO) order.</remarks>
    private Stack<Card> _cardStack = new();

    /// <summary>
    /// Gets the collection of cards associated with this instance.
    /// </summary>
    public List<Card> Cards { get; private set; } = [];

    /// <summary>
    /// Initializes the deck by populating it with a set of cards for the game of Euchre.
    /// </summary>
    private void InitializeDeck()
    {
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                Cards.Add(new Card(suit, rank));
            }
        }
    }

    /// <summary>
    /// Randomizes the order of the cards in the deck.
    /// </summary>
    /// <remarks>This method assigns a random value to each card in the deck and then reorders the cards 
    /// based on these values. The reordered cards are stored in a stack, which can be used for dealing cards
    /// to players. The randomness is determined by the internal randomizer.</remarks>
    public void Shuffle()
    {
        // Create a new shuffle value for every card in the deck.

        foreach (Card card in Cards)
        {
            card.ShuffleValue = _randomizer.Next(MAX_RANDOM_VALUE);
        }

        // Sort the cards in the deck based on the random value and create the stack used for dealing the
        //  cards to the players.

        _cardStack = new Stack<Card>(Cards.OrderBy(c => c.ShuffleValue));
    }

    /// <summary>
    /// Deals the top card from the deck.
    /// </summary>
    /// <returns>The top card from the deck.</returns>
    /// <exception cref="EmptyDeckException" />
    public Card Deal()
    {
        if (_cardStack.Count == 0) throw new EmptyDeckException();
        
        return _cardStack.Pop();
    }
}
