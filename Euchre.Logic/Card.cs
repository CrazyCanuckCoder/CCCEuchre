namespace Euchre.Logic;

public class Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public bool IsJack => Rank == Rank.Jack;

    public bool IsRightBower(Suit trump) => IsJack && Suit == trump;

    public bool IsLeftBower(Suit trump)
    {
        if (!IsJack) return false;
        return trump switch
        {
            Suit.Hearts => Suit == Suit.Diamonds,
            Suit.Diamonds => Suit == Suit.Hearts,
            Suit.Clubs => Suit == Suit.Spades,
            Suit.Spades => Suit == Suit.Clubs,
            _ => false
        };
    }

    public bool IsBower(Suit trump) => IsRightBower(trump) || IsLeftBower(trump);

    public Suit EffectiveSuit(Suit trump)
    {
        if (IsLeftBower(trump)) return trump;
        return Suit;
    }

    // TODO: Write a unit test for this method that validates or refutes the need for the call to EffectiveSuit.
    //       It looks like the call to EffectiveSuit is not needed in the GetTrickValue method.
    public int GetTrickValue(Suit trump, Suit leadSuit)
    {
        if (IsRightBower(trump)) return 1000;
        if (IsLeftBower(trump)) return 999;
        
        var effectiveSuit = EffectiveSuit(trump);
        
        if (effectiveSuit == trump && trump != leadSuit) return 500 + (int)Rank;
        if (effectiveSuit == leadSuit) return (int)Rank;
        
        return 0; // Can't win if not trump or lead suit
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
