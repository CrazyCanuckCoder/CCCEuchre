using Euchre.Logic.Components;

namespace Euchre.UILogic.Interfaces;

public interface IBaseCardDisplay
{
    void ClearCards();
    void RemoveCard();
    void SetupCards(List<Card> cards);
    void SetupCards(List<Card> cards, Suit trumpSuit);
    void AddCards(int numberOfCards);
    void DisplayCards(int numberOfCards);
    void DisableCards(int numberOfCards);
    void DisableCards(List<Card> cards, Suit trumpSuit);
}
