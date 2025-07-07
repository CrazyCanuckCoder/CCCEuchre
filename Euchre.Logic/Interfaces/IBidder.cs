namespace Euchre.Logic.Interfaces;

public interface IBidder
{
    Suit? DetermineTrump(Card kitty);
    bool DetermineWhetherToOrderUp(Card kitty, bool isDealer);
    void DiscardForKitty(Card kitty, Suit trump);
}