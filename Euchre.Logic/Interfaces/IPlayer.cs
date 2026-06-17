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
    Suit? CallTrump(Card kitty, bool isDealer);
    void SortPlayerCards(Suit? trump);
    void ClearHand();
    void DiscardForKitty(Card kitty);
    bool OrderUp(Card kitty, bool isDealer, out bool goUnder);
    Card PlayCard(int index);
    Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit);
    IPlayer Clone();
    void ReceiveSeveralCards(List<ICard> cards);
    bool HasNoAceNoFaceNoTrump(Suit trump);
    List<Card> GetGoUnderCards(List<Card> kittyCards);
}