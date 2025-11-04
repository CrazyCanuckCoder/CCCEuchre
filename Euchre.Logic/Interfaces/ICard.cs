using Euchre.Logic.Components;

namespace Euchre.Logic.Interfaces;

public interface ICard
{
    bool IsJack { get; }
    Rank Rank { get; }
    int ShuffleValue { get; }
    Suit Suit { get; }

    ICard Clone();
    Suit EffectiveSuit(Suit trump);
    int GetTrickValue(Suit trump, Suit leadSuit);
    bool IsBower(Suit trump);
    bool IsLeftBower(Suit trump);
    bool IsRightBower(Suit trump);
}