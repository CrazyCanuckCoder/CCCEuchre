using Euchre.Logic.Components;
using Euchre.Logic.Exceptions;

namespace Euchre.Tests;

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
        Assert.Multiple(() =>
        {
            Assert.That(_deck!.Cards, Has.Count.EqualTo(24)); // 6 ranks * 4 suits
            Assert.That(_deck.Cards.Select(c => c.Suit).Distinct().Count(), Is.EqualTo(4));
            Assert.That(_deck.Cards.Select(c => c.Rank).Distinct().Count(), Is.EqualTo(6));
        });
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
        for (int i = 0; i < 24; i++)
        {
            dealtCards.Add(_deck.Deal() ?? throw new EmptyDeckException());
        }
        
        Assert.Multiple(() =>
        {
            Assert.That(dealtCards, Has.Count.EqualTo(24), "All cards should be dealt after shuffling.");
            Assert.That(dealtCards.Distinct().Count(), Is.EqualTo(24), "Dealt cards should be unique.");
        });
    }
}
