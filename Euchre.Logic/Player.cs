using Euchre.Logic.Exceptions;
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

    /// <summary>
    /// Adds a card to the player's hand.
    /// </summary>
    /// <param name="card">The card to add to the player's hand. Must not already exist in the hand.</param>
    /// <exception cref="TooManyCardsException" />
    /// <exception cref="CardAlreadyExistsException" />
    public void AddCard(Card card)
    {
        if (Hand.Count >= CARDS_PER_PLAYER)
        {
            throw new TooManyCardsException();
        }

        if (Hand.Contains(card))
        {
            throw new CardAlreadyExistsException();
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

    public abstract bool OrderUp(Card kitty, bool isDealer);

    public abstract Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit);
}
