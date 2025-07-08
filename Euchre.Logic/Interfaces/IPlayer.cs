namespace Euchre.Logic.Interfaces;

public interface IPlayer
{
    List<Card> Hand { get; }
    bool IsHuman { get; }
    string Name { get; }

    void AddCard(Card card);
    Suit? CallTrump(Card kitty);
    void ClearHand();
    void DiscardForKitty(Card kitty, Suit trump);
    bool OrderUp(Card kitty, bool isDealer);
    Card PlayCard(int index);
    Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit);
}