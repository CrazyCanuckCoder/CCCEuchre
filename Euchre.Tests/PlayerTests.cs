using Euchre.Logic;
using Euchre.Logic.Exceptions;

namespace Euchre.Tests;

[TestFixture]
public class PlayerTests
{
    private AutomatedPlayer? _player;

    [SetUp]
    public void Setup()
    {
        _player = new AutomatedPlayer("TestPlayer");
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
        for (int rank = (int)Rank.Nine; rank < (int)Rank.Ace; rank++)
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

    // Behaviour: Getting valid cards for a lead suit returns cards that match the lead suit when the player
    //            has cards in hand of the lead suit.

    [Test]
    public void GetValidCards_ReturnsCardsMatchingLeadSuit()
    {
        var leadSuit = Suit.Hearts;
        _player?.AddCard(new Card(Suit.Hearts, Rank.Ace));
        _player?.AddCard(new Card(Suit.Diamonds, Rank.King));
        
        var validCards = _player?.GetValidCards(leadSuit, Suit.Spades);
        
        Assert.That(validCards?.Count, Is.EqualTo(1), "Should return one card matching the lead suit.");
        Assert.That(validCards?[0].Suit, Is.EqualTo(leadSuit), "The valid card should match the lead suit.");
    }

    // Behaviour: Getting valid cards returns all cards when the player has no cards matching the lead suit.

    [Test]
    public void GetValidCards_ReturnsAllCards_WhenNoMatchingLeadSuit()
    {
        _player?.AddCard(new Card(Suit.Diamonds, Rank.Ace));
        _player?.AddCard(new Card(Suit.Clubs, Rank.King));
        _player?.AddCard(new Card(Suit.Clubs, Rank.Queen));
        _player?.AddCard(new Card(Suit.Diamonds, Rank.Queen));

        var validCards = _player?.GetValidCards(Suit.Hearts, Suit.Spades);
        
        Assert.That(validCards?.Count, Is.EqualTo(_player?.Hand.Count), 
            "Should return all cards when no matching lead suit.");
    }

    // Behaviour: Getting valid cards should return an empty list when the hand is empty.

    [Test]
    public void GetValidCards_ReturnsEmpty_WhenHandIsEmpty()
    {
        var validCards = _player?.GetValidCards(Suit.Hearts, Suit.Spades);
        
        Assert.That(validCards?.Count, Is.EqualTo(0), "Should return an empty list when hand is empty.");
    }
}
