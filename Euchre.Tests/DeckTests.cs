using Euchre.Logic.Components;
using Euchre.Logic.Exceptions;

namespace Euchre.Tests.Components;

[TestFixture]
public class DeckTests
{
    private Deck? _deck;

    [SetUp]
    public void Setup()
    {
        _deck = new Deck();
    }

    [TearDown]
    public void TearDown()
    {
        _deck = null;
    }

    // Behaviour: The Deck constructor should initialize a deck of cards with all suits and ranks.

    [Test]
    public void DeckConstructor_ShouldInitializeDeckWithAllCards()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_deck!.Cards, Has.Count.EqualTo(24)); // 6 ranks * 4 suits
            Assert.That(_deck.Cards.Select(c => c.Suit).Distinct().Count(), Is.EqualTo(4));
            Assert.That(_deck.Cards.Select(c => c.Rank).Distinct().Count(), Is.EqualTo(6));
        }
    }

    // Behaviour: If the Deal method is called before Shuffle, it should throw an InvalidOperationException
    //            indicating that the deck is empty.

    [Test]
    public void Deal_ShouldThrowInvalidOperationException_WhenCalledBeforeShuffle()
    {
        Assert.Throws<EmptyDeckException>(() => _deck!.Deal(), "Cannot deal from empty deck.");
    }

    // Behaviour: After initialization, all of the cards in the deck should have a ShuffleValue of 0.

    [Test]
    public void Cards_ShouldHaveShuffleValueOfZero_AfterInitialization()
    {
        Assert.That(_deck!.Cards.All(c => c.ShuffleValue == 0), Is.True,
            "All cards should have a ShuffleValue of 0 after initialization.");
    }

    // Behaviour: After Shuffle is called, all cards should have a non-zero ShuffleValue.

    [Test]
    public void Shuffle_ShouldAssignNonZeroShuffleValue_ToAllCards()
    {
        _deck?.Shuffle();

        Assert.That(_deck!.Cards.All(c => c.ShuffleValue > 0), Is.True,
            "All cards should have a non-zero ShuffleValue after shuffling.");
    }

    // Behaviour: After Shuffle is called, no two cards should have the same ShuffleValue.

    [Test]
    public void Shuffle_ShouldAssignUniqueShuffleValues_ToAllCards()
    {
        _deck!.Shuffle();

        var shuffleValues = _deck.Cards.Select(c => c.ShuffleValue).Distinct().ToList();
        Assert.That(shuffleValues?.Count, Is.EqualTo(_deck.Cards.Count),
            "More than one card has the same ShuffleValue after shuffling the deck.");
    }

    // Behaviour: After Shuffle is called, the Deal method should return cards in a random order.

    [Test]
    public void Deal_ShouldReturnCardsInRandomOrder_AfterShuffle()
    {
        _deck!.Shuffle();

        var dealtCards = new List<Card>();
        for (var i = 0; i < 24; i++)
        {
            dealtCards.Add(_deck.Deal() ?? throw new EmptyDeckException());
        }

        using (Assert.EnterMultipleScope())
        {
            Assert.That(dealtCards, Has.Count.EqualTo(24), "All cards should be dealt after shuffling.");
            Assert.That(dealtCards.Distinct().Count(), Is.EqualTo(24), "Dealt cards should be unique.");
        }
    }

    // New tests for Clone, GetKittyCards and SetKittyCards

    [Test]
    public void Clone_WhenCalled_ReturnsIndependentCopyOfCardsList()
    {
        // Arrange
        var original = _deck!;

        // Act
        var clone = original.Clone();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(clone, Is.Not.SameAs(original), "Clone should return a different Deck instance.");
            Assert.That(clone.Cards, Is.Not.SameAs(original.Cards), "Cards list should be a different instance.");
            Assert.That(clone.Cards, Has.Count.EqualTo(original.Cards.Count), "Cloned deck should have same number of cards.");

            // Mutate original and ensure clone is unaffected
            var removed = original.Cards[0];
            original.Cards.RemoveAt(0);

            Assert.That(original.Cards.Count + 1, Is.EqualTo(clone.Cards.Count), "Removing from original should not change clone count.");
            Assert.That(clone.Cards.Any(c => c.Equals(removed)), Is.True, "Clone should still contain the removed card (objects are not deep-copied).");
        }
    }

    [Test]
    public void GetKittyCards_Throws_WhenCardStackCountIsNotThree()
    {
        // Arrange
        var deck = _deck!; // newly created deck has an empty _cardStack

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => deck.GetKittyCards());
        Assert.That(ex?.Message, Is.EqualTo("Invalid number of cards in the deck to retrieve the kitty."));

        // Ensure no side-effect occurred: dealing should still indicate empty deck
        Assert.Throws<EmptyDeckException>(() => deck.Deal());
    }

    [Test]
    public void SetKittyCards_WhenEmpty_SetsThreeCardsAndGetKittyReturnsThemInLifoOrder()
    {
        // Arrange
        var deck = _deck!;
        var a = new Card(Suit.Hearts, Rank.Nine);
        var b = new Card(Suit.Diamonds, Rank.Ten);
        var c = new Card(Suit.Clubs, Rank.Jack);
        var kitty = new List<Card> { a, b, c };

        // Act
        deck.SetKittyCards(kitty);
        var returned = deck.GetKittyCards();

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returned, Has.Count.EqualTo(3));
            // Stack is LIFO: first returned should be the last item of the provided list
            Assert.That(returned[0].Equals(c));
            Assert.That(returned[1].Equals(b));
            Assert.That(returned[2].Equals(a));

            // After popping the kitty the deck should be empty (dealing will throw)
            Assert.Throws<EmptyDeckException>(() => deck.Deal());
        }
    }

    [Test]
    public void SetKittyCards_Throws_WhenKittyCountIsIncorrect_AndDoesNotModifyDeck()
    {
        // Arrange
        var deck = _deck!;
        var badKitty = new List<Card> { new Card(Suit.Hearts, Rank.Nine), new Card(Suit.Hearts, Rank.Ten) };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => deck.SetKittyCards(badKitty));
        Assert.That(ex?.Message, Is.EqualTo("Invalid number of cards provided to set the kitty."));

        // Ensure no side-effect: deck should remain empty
        Assert.Throws<EmptyDeckException>(() => deck.Deal());
    }

    [Test]
    public void SetKittyCards_Throws_WhenDeckNotEmpty_AndDoesNotModifyDeck()
    {
        // Arrange
        var deck = _deck!;
        deck.Shuffle(); // populate internal stack
        var kitty = new List<Card> { new Card(Suit.Hearts, Rank.Nine), new Card(Suit.Diamonds, Rank.Ten), new Card(Suit.Clubs, Rank.Jack) };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => deck.SetKittyCards(kitty));
        Assert.That(ex?.Message, Is.EqualTo("Deck is not empty for accepting kitty cards."));

        // Ensure the deck is still usable (i.e. not emptied by the failed call)
        Assert.DoesNotThrow(() => { var c = deck.Deal(); });
    }

}
