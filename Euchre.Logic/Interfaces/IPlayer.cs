using Euchre.Logic.Components;

namespace Euchre.Logic.Interfaces;

public interface IPlayer
{
    string Name { get; }
    int AvatarNumber { get; }
    List<Card> Hand { get; }
    bool IsHuman { get; }
    bool IsGoingAlone { get; set; }
    int TeamIndex { get; }
    int PlayerIndex { get; }

    void AddCard(Card card);
    Suit? CallTrump(Card kitty);
    void SortPlayerCards(Suit? trump);
    void ClearHand();
    void DiscardForKitty(Card kitty);
    bool OrderUp(Card kitty, bool isDealer);
    Card PlayCard(int index);
    Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit);
}