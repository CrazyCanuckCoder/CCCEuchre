using Euchre.Logic.Components;

namespace Euchre.Logic.Interfaces;

public interface IPlayManager
{
    Card DetermineCardToPlay(Trick trick, Suit trump, Suit? leadSuit);
}