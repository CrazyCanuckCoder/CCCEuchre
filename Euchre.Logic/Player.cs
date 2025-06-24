using Euchre.Logic.Interfaces;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

public abstract class Player : IPlayer
{
    public Player(string name, bool isHuman)
    {
        Name = name;
        Hand = [];
        IsHuman = isHuman;
    }

    public List<Card> Hand { get; protected set; }

    public bool IsHuman { get; protected set; }

    public string Name { get; protected set; }

    public void AddCard(Card card)
    {
        if (Hand.Count >= CARDS_PER_PLAYER)
        {
            throw new InvalidOperationException($"Cannot add more than {CARDS_PER_PLAYER} cards to hand.");
        }

        if (Hand.Contains(card))
        {
            throw new ArgumentException("Card already exists in hand.");
        }

        Hand.Add(card);
    }

    public void ClearHand()
    {
        Hand.Clear();
    }

    public Card PlayCard(int index)
    {
        if (index < 0 || index >= Hand.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var card = Hand[index];
        Hand.RemoveAt(index);
        return card;
    }

    public abstract Suit? CallTrump(Card kitty);

    public abstract void DiscardForKitty(Card kitty, Suit trump);

    public abstract List<Card> GetValidCards(Suit leadSuit, Suit trump);

    public abstract bool OrderUp(Card kitty, bool isDealer);

    public abstract Card SelectCardToPlay(List<Card> trick, Suit trump, Suit? leadSuit);
}
