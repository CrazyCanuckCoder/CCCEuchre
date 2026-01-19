using Euchre.Logic.Components;

namespace Euchre.Logic.Interfaces;

public interface IDeck
{
    List<Card> Cards { get; }

    Card Deal();
    void Shuffle();
}