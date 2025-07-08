namespace Euchre.Logic.Interfaces;

public interface IPlayManager
{
    Card DetermineCardToPlay(Trick trick, Suit trump, Suit? leadSuit);
    List<Card> GetValidCards(Suit leadSuit, Suit trump);
}