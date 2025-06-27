using Euchre.Logic.Exceptions;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

public class Deck
{
    public Deck()
    {
        _randomizer = new Random();
        InitializeDeck();
    }

    private readonly Random _randomizer;
    private Stack<Card> _cardStack = new();

    public List<Card> Cards { get; private set; } = [];

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
