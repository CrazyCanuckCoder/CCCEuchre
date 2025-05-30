namespace Euchre.Logic;

public class Deck
{
    private List<Card> cards;
    private readonly Random random;

    public Deck()
    {
        random = new Random();
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        cards = [];
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            foreach (Rank rank in Enum.GetValues<Rank>())
            {
                cards.Add(new Card(suit, rank));
            }
        }
    }

    // TODO: How efficient is this shuffle? Is it sufficient for a card game?
    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    // TODO: Consider adding a method to reset the deck to its initial state.
    // TODO: Instead of removing cards from the deck, use a flag to mark cards as dealt.
    public Card Deal()
    {
        if (cards.Count == 0) throw new InvalidOperationException("Cannot deal from empty deck");
        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public int Count => cards.Count;
}
