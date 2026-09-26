using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Components;

/// <summary>
/// Represents a standard deck of playing cards, providing functionality for shuffling and dealing cards.
/// </summary>
public class Deck : IDeck
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

        foreach (var card in Cards)
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
        return _cardStack.Count == 0 ? throw new EmptyDeckException() : _cardStack.Pop();
    }

    /// <summary>
    /// Creates a copy of the Deck.
    /// </summary>
    /// <returns>A Deck with the same properties as this Deck.</returns>
    public Deck Clone()
    {
        return new Deck()
        {
            Cards = [.. Cards],
        };
    }

    /// <summary>
    /// Retrieves the three cards that are currently remaining in the "kitty".
    /// </summary>
    /// <returns>A list of the three cards in the kitty.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<Card> GetKittyCards()
    {
        if (_cardStack.Count != 3)
        {
            throw new InvalidOperationException("Invalid number of cards in the deck to retrieve the kitty.");
        }

        var kittyCards = new List<Card>();

        for (var numCards = 0; numCards < 3; numCards++)
        {
            kittyCards.Add(_cardStack.Pop());
        }

        return kittyCards;
    }

    /// <summary>
    /// Sets the three cards in the "kitty" to the provided list of cards.
    /// </summary>
    /// <param name="kittyCards">The list of three cards to set as the kitty.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SetKittyCards(List<Card> kittyCards)
    {
        if (kittyCards.Count != 3)
        {
            throw new InvalidOperationException("Invalid number of cards provided to set the kitty.");
        }
        if (_cardStack.Count != 0)
        {
            throw new InvalidOperationException("Deck is not empty for accepting kitty cards.");
        }

        _cardStack = new Stack<Card>(kittyCards);
    }
}
