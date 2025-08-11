using Euchre.Logic.Exceptions;
using Euchre.Logic.Interfaces;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Components;

public class Trick
{
    public Trick(Suit trump)
    {
        Cards = [];
        Trump = trump;
    }

    public List<(IPlayer Player, Card Card)> Cards { get; }

    public Suit LeadSuit { get; private set; }

    public Suit Trump { get; }

    public bool IsComplete => Cards.Count == MAX_NUMBER_OF_TRICK_CARDS;

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
        if (!IsComplete) throw new TrickIncompleteException();

        return GetHighestCardInTrick().Player;
    }

    public Card GetCurrentLeadingCard()
    {
        if (Cards.Count == 0) throw new EmptyTrickException();

        return GetHighestCardInTrick().Card;
    }

    private (IPlayer Player, Card Card) GetHighestCardInTrick()
    {
        return Cards.OrderByDescending(c => c.Card.GetTrickValue(Trump, LeadSuit))
                    .First();
    }
}
