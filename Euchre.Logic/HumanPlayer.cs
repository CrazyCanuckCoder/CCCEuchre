namespace Euchre.Logic;

public class HumanPlayer : Player
{
    public HumanPlayer(string name) : base(name, true) 
    { 
    }

    public override List<Card> GetValidCards(Suit leadSuit, Suit trump)
    {
        // Human players will need UI to select cards.
        return Hand;
    }
    public override bool OrderUp(Card kitty, bool isDealer)
    {
        // Human players will need UI to decide whether to order up.
        return false;
    }
    public override Suit? CallTrump(Card kitty)
    {
        // Human players will need UI to select trump suit.
        return null;
    }
    public override Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit)
    {
        // Human players will need UI to select card to play.
        return null;
    }

    public override void DiscardForKitty(Card kitty, Suit trump)
    {
    }
}
