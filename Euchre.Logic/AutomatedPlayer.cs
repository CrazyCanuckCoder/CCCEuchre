using Euchre.Logic.Interfaces;

namespace Euchre.Logic;

public class AutomatedPlayer : Player
{
    public AutomatedPlayer(string name) : base(name, false) 
    {
        _bidder = new Bidder(Hand);
        _playManager = new PlayManager(Hand);
    }

    private readonly IBidder _bidder;

    private readonly IPlayManager _playManager;


    public override List<Card> GetValidCards(Suit leadSuit, Suit trump)
    {
        if (Hand.Count == 0) return [];

        var leadCards = Hand.Where(c => c.EffectiveSuit(trump) == leadSuit).ToList();
        return leadCards.Count > 0 ? leadCards : [.. Hand];
    }

    public override bool OrderUp(Card kitty, bool isDealer)
    {
        return _bidder.DetermineWhetherToOrderUp(kitty, isDealer);
    }

    public override Suit? CallTrump(Card kitty)
    {
        return _bidder.DetermineTrump(kitty);
    }

    public override void DiscardForKitty(Card kitty, Suit trump)
    {
        _bidder.DiscardForKitty(kitty, trump);
    }

    public override Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit)
    {
        return _playManager.DetermineCardToPlay(trick, trump, leadSuit);
    }
}
