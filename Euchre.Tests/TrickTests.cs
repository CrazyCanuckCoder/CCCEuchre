using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;
using Euchre.Logic.Exceptions;
using Moq;

namespace Euchre.Tests;

[TestFixture]
public class TrickTests
{
    // Tests will use mocked IPlayer instances (Moq) instead of a concrete TestPlayer implementation.

    [Test]
    public void IsComplete_ReturnsFalse_WhenLessThanMaxCards()
    {
        // Arrange
        var trick = new Trick(Suit.Hearts);
        var p1Mock = new Mock<IPlayer>();
        p1Mock.SetupGet(p => p.Name).Returns("P1");
        var p2Mock = new Mock<IPlayer>();
        p2Mock.SetupGet(p => p.Name).Returns("P2");
        var p3Mock = new Mock<IPlayer>();
        p3Mock.SetupGet(p => p.Name).Returns("P3");

        trick.AddCard(p1Mock.Object, new Card(Suit.Clubs, Rank.Ten));
        trick.AddCard(p2Mock.Object, new Card(Suit.Diamonds, Rank.Nine));
        trick.AddCard(p3Mock.Object, new Card(Suit.Spades, Rank.Jack));

        // Act
        var result = trick.IsComplete;

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void IsComplete_ReturnsTrue_WhenMaxCardsAdded()
    {
        // Arrange
        var trick = new Trick(Suit.Spades);
        var players = new[] { new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>(), new Mock<IPlayer>() };
        players[0].SetupGet(p => p.Name).Returns("P1");
        players[1].SetupGet(p => p.Name).Returns("P2");
        players[2].SetupGet(p => p.Name).Returns("P3");
        players[3].SetupGet(p => p.Name).Returns("P4");

        trick.AddCard(players[0].Object, new Card(Suit.Clubs, Rank.Nine));
        trick.AddCard(players[1].Object, new Card(Suit.Diamonds, Rank.Ten));
        trick.AddCard(players[2].Object, new Card(Suit.Hearts, Rank.Queen));
        trick.AddCard(players[3].Object, new Card(Suit.Spades, Rank.King));

        // Act
        var result = trick.IsComplete;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void GetWinner_ReturnsPlayerWithHighestCard()
    {
        // Arrange
        var trick = new Trick(Suit.Hearts);
        var p1 = new Mock<IPlayer>(); p1.SetupGet(p => p.Name).Returns("Leader");
        var p2 = new Mock<IPlayer>(); p2.SetupGet(p => p.Name).Returns("Winner");
        var p3 = new Mock<IPlayer>(); p3.SetupGet(p => p.Name).Returns("Other");

        // First card sets lead suit (Clubs)
        trick.AddCard(p1.Object, new Card(Suit.Clubs, Rank.Queen));
        // A right bower (Jack of Hearts) should beat non-trump cards
        trick.AddCard(p2.Object, new Card(Suit.Hearts, Rank.Jack));
        // Non-winning card
        trick.AddCard(p3.Object, new Card(Suit.Clubs, Rank.King));

        // Act
        var winner = trick.GetWinner();

        // Assert
        Assert.That(winner, Is.SameAs(p2.Object));
    }

    [Test]
    public void GetCurrentLeadingCard_ReturnsCard_WhenCardsPresent()
    {
        // Arrange
        var trick = new Trick(Suit.Spades);
        var p1 = new Mock<IPlayer>(); p1.SetupGet(p => p.Name).Returns("P1");
        var p2 = new Mock<IPlayer>(); p2.SetupGet(p => p.Name).Returns("P2");

        // Lead with Hearts Ace (lead suit Hearts)
        trick.AddCard(p1.Object, new Card(Suit.Hearts, Rank.Ace));
        // Play a Spades Jack (right bower when trump is Spades) which should win
        trick.AddCard(p2.Object, new Card(Suit.Spades, Rank.Jack));

        // Act
        var leading = trick.GetCurrentLeadingCard();

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(leading.Suit, Is.EqualTo(Suit.Spades));
            Assert.That(leading.Rank, Is.EqualTo(Rank.Jack));
        }
    }

    [Test]
    public void GetCurrentLeadingCard_ThrowsEmptyTrickException_WhenNoCards()
    {
        // Arrange
        var trick = new Trick(Suit.Diamonds);

        // Act & Assert
        Assert.That(() => trick.GetCurrentLeadingCard(), Throws.TypeOf<EmptyTrickException>());
    }
}
