using Euchre.Logic;

namespace Euchre.Tests;

[TestFixture]
public class CardTests
{
    [SetUp]
    public void Setup()
    {
    }

    // Behaviour: The IsRightBower method should return true or false depending on whether the card is the
    //            right bower for the given trump suit.

    [TestCase(Suit.Hearts, Rank.Jack,  true,  TestName = "Right Bower Hearts")]
    [TestCase(Suit.Clubs,  Rank.Jack,  true,  TestName = "Right Bower Clubs")]
    [TestCase(Suit.Hearts, Rank.Queen, false, TestName = "Not Right Bower Hearts")]
    [TestCase(Suit.Clubs,  Rank.Ten,   false, TestName = "Not Right Bower Clubs")]
    public void IsRightBower_ShouldReturnTrue_WhenCardIsRightBower(Suit trump, Rank rank, bool expected)
    {
        var card = new Card(trump, rank);
        Assert.That(card.IsRightBower(trump), Is.EqualTo(expected));
    }

    // Behaviour: The IsLeftBower method should return true or false depending on whether the card is the
    //            left bower for the given trump suit.

    [TestCase(Suit.Hearts,   Suit.Diamonds, Rank.Jack, true,  TestName = "Left Bower Hearts")]
    [TestCase(Suit.Spades,   Suit.Clubs,    Rank.Jack, true,  TestName = "Left Bower Spades")]
    [TestCase(Suit.Hearts,   Suit.Clubs,    Rank.Jack, false, TestName = "Not Left Bower Hearts")]
    [TestCase(Suit.Clubs,    Suit.Hearts,   Rank.Jack, false, TestName = "Not Left Bower Clubs")]
    [TestCase(Suit.Diamonds, Suit.Hearts,   Rank.Ten,  false, TestName = "Not Left Bower Diamonds")]
    [TestCase(Suit.Spades,   Suit.Clubs,    Rank.King, false, TestName = "Not Left Bower Spades")]
    public void IsLeftBower_ShouldReturnTrue_WhenCardIsLeftBower(Suit trump, Suit cardSuit, Rank rank, 
        bool expected)
    {
        var card = new Card(cardSuit, rank);
        Assert.That(card.IsLeftBower(trump), Is.EqualTo(expected));
    }

    // Behaviour: The IsBower method should return true if the card is either a right or left bower for the
    //            given trump suit.

    [TestCase(Suit.Hearts, Suit.Hearts,   Rank.Jack,  true,  TestName = "Bower Hearts, Right")]
    [TestCase(Suit.Clubs,  Suit.Clubs,    Rank.Jack,  true,  TestName = "Bower Clubs, Right")]
    [TestCase(Suit.Hearts, Suit.Diamonds, Rank.Jack,  true,  TestName = "Bower Hearts, Left")]
    [TestCase(Suit.Clubs,  Suit.Spades,   Rank.Jack,  true,  TestName = "Bower Clubs, Left")]
    [TestCase(Suit.Hearts, Suit.Hearts,   Rank.Queen, false, TestName = "Not Bower Hearts, Right")]
    [TestCase(Suit.Clubs,  Suit.Clubs,    Rank.Ten,   false, TestName = "Not Bower Clubs, Right")]
    [TestCase(Suit.Hearts, Suit.Diamonds, Rank.Queen, false, TestName = "Not Bower Hearts, Left")]
    [TestCase(Suit.Clubs,  Suit.Spades,   Rank.Ten,   false, TestName = "Not Bower Clubs, Left")]
    public void IsBower_ShouldReturnTrue_WhenCardIsEitherBower(Suit trump, Suit cardSuit, Rank rank, 
        bool expected)
    {
        var card = new Card(cardSuit, rank);
        Assert.That(card.IsBower(trump), Is.EqualTo(expected));
    }

    // Behaviour: The EffectiveSuit method should return the correct suit for a card based on the card's rank
    //            and the current trump suit.

    [TestCase(Suit.Hearts, Suit.Hearts,   Rank.Jack,  Suit.Hearts,   TestName = "Effective Suit Right Bower Hearts")]
    [TestCase(Suit.Clubs,  Suit.Clubs,    Rank.Jack,  Suit.Clubs,    TestName = "Effective Suit Right Bower Clubs")]
    [TestCase(Suit.Hearts, Suit.Diamonds, Rank.Jack,  Suit.Hearts,   TestName = "Effective Suit Left Bower Hearts")]
    [TestCase(Suit.Clubs,  Suit.Spades,   Rank.Jack,  Suit.Clubs,    TestName = "Effective Suit Left Bower Clubs")]
    [TestCase(Suit.Hearts, Suit.Hearts,   Rank.Queen, Suit.Hearts,   TestName = "Effective Suit Not Bower Hearts")]
    [TestCase(Suit.Clubs,  Suit.Clubs,    Rank.Ten,   Suit.Clubs,    TestName = "Effective Suit Not Bower Clubs")]
    [TestCase(Suit.Hearts, Suit.Diamonds, Rank.Queen, Suit.Diamonds, TestName = "Effective Suit Not Bower Hearts")]
    [TestCase(Suit.Clubs,  Suit.Spades,   Rank.Ten,   Suit.Spades,   TestName = "Effective Suit Not Bower Clubs")]
    public void EffectiveSuit_ShouldReturnCorrectSuit(Suit trump, Suit cardSuit, Rank rank, Suit expected)
    {
        var card = new Card(cardSuit, rank);
        Assert.That(card.EffectiveSuit(trump), Is.EqualTo(expected));
    }
}