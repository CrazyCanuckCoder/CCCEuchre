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

    public int ShuffleValue { get; internal set; }

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

    public int GetTrickValue(Suit trump, Suit leadSuit)
    {
        if (IsRightBower(trump)) return 1000;
        if (IsLeftBower(trump)) return 999;

        if (Suit == trump) return 500 + (int)Rank;
        if (Suit == leadSuit) return (int)Rank;
        
        return 0; // Can't win if not trump or lead suit
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}
