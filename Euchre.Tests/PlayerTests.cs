using Euchre.Logic.Components;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;

namespace Euchre.Tests;

[TestFixture]
public class PlayerTests
{
    private AutomatedPlayer? _player;

    [SetUp]
    public void Setup()
    {
        _player = new AutomatedPlayer("TestPlayer", 0, 0, new GameStateManager(), 0);
    }

    [TearDown]
    public void TearDown()
    {
        _player = null;
    }

    // Behaviour: Adding a card to the player's hand increases the card count by 1.

    [Test]
    public void AddCard_IncreasesCardCount()
    {
        var card = new Card(Suit.Hearts, Rank.Ace);
        _player?.AddCard(card);
        Assert.That(_player?.Hand.Count, Is.EqualTo(1));
    }

    // Behaviour: Adding a card that already exists in the hand should throw an exception.

    [Test]
    public void AddCard_ShouldThrowException_WhenCardAlreadyExists()
    {
        var card = new Card(Suit.Hearts, Rank.Ace);
        _player?.AddCard(card);

        Assert.Throws<CardAlreadyExistsException>(() => _player?.AddCard(card), "Card already exists in hand.");
    }

    // Behaviour: Adding a card that exceeds the maximum hand size should throw an exception.

    [Test]
    public void AddCard_ShouldThrowException_WhenExceedingMaxHandSize()
    {
        for (var rank = (int)Rank.Nine; rank < (int)Rank.Ace; rank++)
        {
            _player?.AddCard(new Card(Suit.Hearts, (Rank)rank));
        }
        var newCard = new Card(Suit.Diamonds, Rank.Ace);
        Assert.Throws<TooManyCardsException>(() => _player?.AddCard(newCard), 
            "Cannot add more than 5 cards to hand.");
    }

    // Behaviour: Clearing the hand should remove all cards.
    
    [Test]
    public void ClearHand_RemovesAllCards()
    {
        _player?.AddCard(new Card(Suit.Hearts, Rank.Ace));
        _player?.ClearHand();
        
        Assert.That(_player?.Hand.Count, Is.EqualTo(0), "Hand should be empty after clearing.");
    }

    // Behaviour: Playing a card by index should remove the card from the hand and return it.
    
    [Test]
    public void PlayCard_RemovesCardFromHand()
    {
        var card = new Card(Suit.Hearts, Rank.Ace);
        _player?.AddCard(card);
        
        var playedCard = _player?.PlayCard(0);
        
        Assert.Multiple(() =>
        {
            Assert.That(playedCard, Is.EqualTo(card), "Played card should match the one added.");
            Assert.That(_player?.Hand.Count, Is.EqualTo(0), "Hand should be empty after playing the card.");
        });
    }
    
    // Behaviour: Playing a card with an invalid index should throw an exception.
    
    [Test]
    public void PlayCard_ShouldThrowException_WhenIndexIsInvalid()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _player?.PlayCard(-1), 
                "Index cannot be negative.");
            Assert.Throws<ArgumentOutOfRangeException>(() => _player?.PlayCard(0), 
                "Index is out of range for the hand.");
        });
    }

    // Behaviour: Receiving several cards adds all cards to the hand.
    [Test]
    public void ReceiveSeveralCards_AddsAllCards()
    {
        var cards = new List<ICard>
        {
            new Card(Suit.Clubs, Rank.Nine),
            new Card(Suit.Diamonds, Rank.Ten),
            new Card(Suit.Spades, Rank.Nine)
        };

        _player?.ReceiveSeveralCards(cards);

        Assert.That(_player?.Hand.Count, Is.EqualTo(3));
        Assert.That(_player?.Hand.Contains(new Card(Suit.Clubs, Rank.Nine)), Is.True);
    }

    // Behaviour: Receiving several cards where duplicates exist throws an exception.
    [Test]
    public void ReceiveSeveralCards_Throws_WhenDuplicateInList()
    {
        var duplicate = new Card(Suit.Hearts, Rank.Ten);
        var cards = new List<ICard> { duplicate, duplicate };

        Assert.Throws<CardAlreadyExistsException>(() => _player?.ReceiveSeveralCards(cards));
    }

    // Behaviour: Sorting player cards orders trump cards first.
    [Test]
    public void SortPlayerCards_PutsTrumpFirst()
    {
        _player?.AddCard(new Card(Suit.Hearts, Rank.Ace));
        _player?.AddCard(new Card(Suit.Clubs, Rank.Nine));

        _player?.SortPlayerCards(Suit.Clubs);

        Assert.That(_player?.Hand.Count, Is.EqualTo(2));
        Assert.That(_player?.Hand[0].EffectiveSuit(Suit.Clubs), Is.EqualTo(Suit.Clubs));
    }

    // Behaviour: HasNoAceNoFaceNoTrump returns true when rule enabled and hand contains no ace, face or trump.
    [Test]
    public void HasNoAceNoFaceNoTrump_ReturnsTrue_WhenNoAcesFacesOrTrump()
    {
        GameSettingsManager.Instance.NoAceNoFaceNoTrumpRule = true;

        _player?.ClearHand();
        _player?.AddCard(new Card(Suit.Clubs, Rank.Nine));
        _player?.AddCard(new Card(Suit.Diamonds, Rank.Ten));

        var result = _player?.HasNoAceNoFaceNoTrump(Suit.Hearts);

        Assert.That(result, Is.True);
    }

    // Behaviour: HasNoAceNoFaceNoTrump returns false when the rule is disabled regardless of hand.
    [Test]
    public void HasNoAceNoFaceNoTrump_ReturnsFalse_WhenRuleDisabled()
    {
        GameSettingsManager.Instance.NoAceNoFaceNoTrumpRule = false;

        _player?.ClearHand();
        _player?.AddCard(new Card(Suit.Clubs, Rank.Nine));

        var result = _player?.HasNoAceNoFaceNoTrump(Suit.Spades);

        Assert.That(result, Is.False);
    }

    // Behaviour: HasNoAceNoFaceNoTrump returns false when hand contains an ace or face or trump.
    [Test]
    public void HasNoAceNoFaceNoTrump_ReturnsFalse_WhenHasAceFaceOrTrump()
    {
        GameSettingsManager.Instance.NoAceNoFaceNoTrumpRule = true;

        _player?.ClearHand();
        _player?.AddCard(new Card(Suit.Clubs, Rank.Ace));

        var result = _player?.HasNoAceNoFaceNoTrump(Suit.Hearts);

        Assert.That(result, Is.False);
    }
}
