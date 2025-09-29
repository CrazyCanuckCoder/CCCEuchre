using Euchre.Logic.Components;

namespace Euchre.Logic.Interfaces;

public interface IPlayer
{
    List<Card> Hand { get; }
    bool IsHuman { get; }
    string Name { get; }
    bool IsGoingAlone { get; set; }
    int TeamIndex { get; }


    void AddCard(Card card);
    Suit? CallTrump(Card kitty);
    void ClearHand();
    void DiscardForKitty(Card kitty);
    bool OrderUp(Card kitty, bool isDealer);
    Card PlayCard(int index);
    Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit);
}