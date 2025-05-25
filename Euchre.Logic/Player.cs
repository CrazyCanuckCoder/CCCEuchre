namespace Euchre.Logic;

public class Player
{
    public string Name { get; }
    public List<Card> Hand { get; }
    public bool IsHuman { get; }

    public Player(string name, bool isHuman = false)
    {
        Name = name;
        Hand = [];
        IsHuman = isHuman;
    }

    public void AddCard(Card card)
    {
        Hand.Add(card);
    }

    public void ClearHand()
    {
        Hand.Clear();
    }

    public Card PlayCard(int index)
    {
        if (index < 0 || index >= Hand.Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        
        var card = Hand[index];
        Hand.RemoveAt(index);
        return card;
    }

    public List<Card> GetValidCards(Suit leadSuit, Suit trump)
    {
        if (Hand.Count == 0) return [];

        var leadCards = Hand.Where(c => c.EffectiveSuit(trump) == leadSuit).ToList();
        return leadCards.Count > 0 ? leadCards : [.. Hand];
    }

    public bool OrderUp(Card kitty, Suit trump, int round, bool isDealer)
    {
        if (IsHuman) return false; // For AI, implement simple strategy
        
        var trumpCards = Hand.Count(c => c.EffectiveSuit(trump) == trump || c.IsBower(trump));
        var bowers = Hand.Count(c => c.IsBower(trump));
        var aces = Hand.Count(c => c.Rank == Rank.Ace);
        
        if (round == 1) // First round - consider kitty suit
        {
            return trumpCards >= 3 || (trumpCards >= 2 && bowers >= 1) || (isDealer && trumpCards >= 2);
        }
        else // Second round - different suit
        {
            return trumpCards >= 3 || bowers >= 1;
        }
    }

    public Suit? CallTrump(Card kitty)
    {
        if (IsHuman) return null; // For AI, implement simple strategy
        
        var suitCounts = new Dictionary<Suit, int>();
        foreach (Suit suit in Enum.GetValues<Suit>())
        {
            if (suit == kitty.Suit) continue; // Can't call kitty suit in round 2
            
            var count = Hand.Count(c => c.EffectiveSuit(suit) == suit || c.IsBower(suit));
            suitCounts[suit] = count;
        }
        
        var bestSuit = suitCounts.OrderByDescending(kvp => kvp.Value).First();
        return bestSuit.Value >= 3 ? bestSuit.Key : null;
    }

    public Card SelectCardToPlay(List<Card> trick, Suit trump, Suit? leadSuit)
    {
        if (IsHuman) return null; // Human players need UI
        
        var validCards = GetValidCards(leadSuit ?? trump, trump);
        if (validCards.Count == 0) return Hand[0];
        
        if (trick.Count == 0) // Leading
        {
            // Lead with highest trump if available, otherwise highest card
            var trumps = validCards.Where(c => c.EffectiveSuit(trump) == trump).ToList();
            if (trumps.Count > 0)
                return trumps.OrderByDescending(c => c.GetTrickValue(trump, trump)).First();
            
            return validCards.OrderByDescending(c => (int)c.Rank).First();
        }
        else // Following
        {
            var currentWinner = GetTrickWinner(trick, trump, leadSuit.Value);
            var winningValue = currentWinner.GetTrickValue(trump, leadSuit.Value);
            
            // Try to win the trick
            var canWin = validCards.Where(c => c.GetTrickValue(trump, leadSuit.Value) > winningValue).ToList();
            if (canWin.Count > 0)
                return canWin.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();
            
            // Can't win, play lowest card
            return validCards.OrderBy(c => c.GetTrickValue(trump, leadSuit.Value)).First();
        }
    }

    private Card GetTrickWinner(List<Card> trick, Suit trump, Suit leadSuit)
    {
        return trick.OrderByDescending(c => c.GetTrickValue(trump, leadSuit)).First();
    }

    public void DiscardForKitty(Card kitty, Suit trump)
    {
        if (IsHuman) return; // Human players need UI
        
        // Discard lowest non-trump card
        var nonTrumps = Hand.Where(c => c.EffectiveSuit(trump) != trump && !c.IsBower(trump)).ToList();
        if (nonTrumps.Count > 0)
        {
            var discard = nonTrumps.OrderBy(c => (int)c.Rank).First();
            Hand.Remove(discard);
        }
        else
        {
            // If all trumps, discard lowest
            var discard = Hand.OrderBy(c => c.GetTrickValue(trump, trump)).First();
            Hand.Remove(discard);
        }
        
        Hand.Add(kitty);
    }
}
