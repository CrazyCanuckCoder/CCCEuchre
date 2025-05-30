namespace Euchre.Logic;

public class Trick
{
    public List<(Player Player, Card Card)> Cards { get; }
    public Suit LeadSuit { get; private set; }
    public Suit Trump { get; }

    public Trick(Suit trump)
    {
        Cards = [];
        Trump = trump;
    }

    public void AddCard(Player player, Card card)
    {
        if (Cards.Count == 0)
        {
            LeadSuit = card.EffectiveSuit(Trump);
        }
        Cards.Add((player, card));
    }

    public Player GetWinner()
    {
        if (Cards.Count == 0) return null;
        
        return Cards.OrderByDescending(c => c.Card.GetTrickValue(Trump, LeadSuit))
                    .First()
                    .Player;
    }

    public bool IsComplete => Cards.Count == 4;
}
