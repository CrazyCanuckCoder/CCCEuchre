using Euchre.Logic.Interfaces;
using static Euchre.Logic.Constants;

namespace Euchre.Logic;

public class Trick
{
    public List<(IPlayer Player, Card Card)> Cards { get; }
    public Suit LeadSuit { get; private set; }
    public Suit Trump { get; }

    public Trick(Suit trump)
    {
        Cards = [];
        Trump = trump;
    }

    public void AddCard(IPlayer player, Card card)
    {
        if (Cards.Count == 0)
        {
            LeadSuit = card.EffectiveSuit(Trump);
        }
        Cards.Add((player, card));
    }

    public IPlayer GetWinner()
    {
        // TODO: Change this to throw an exception when there are no cards.
        if (!IsComplete) return null;
        
        return Cards.OrderByDescending(c => c.Card.GetTrickValue(Trump, LeadSuit))
                    .First()
                    .Player;
    }

    public bool IsComplete => Cards.Count == MAX_NUMBER_OF_TRICK_CARDS;
}
