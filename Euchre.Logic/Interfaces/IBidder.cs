using Euchre.Logic.Components;

namespace Euchre.Logic.Interfaces;

public interface IBidder
{
    Suit? DetermineTrump(Card kitty, bool isDealer, out bool goAlone);
    bool DetermineWhetherToOrderUp(Card kitty, bool isDealer, out bool goAlone);
    void DiscardForKitty(Card kitty);
}