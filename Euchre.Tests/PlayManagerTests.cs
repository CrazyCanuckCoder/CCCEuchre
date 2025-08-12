using Euchre.Logic.Components;
using Euchre.Logic.Helpers;

namespace Euchre.Tests;

[TestFixture]
public class PlayManagerTests
{
    // Behaviour: Getting valid cards for a lead suit returns cards that match the lead suit when the player
    //            has cards in hand of the lead suit.

    [Test]
    public void GetValidCards_ReturnsCardsMatchingLeadSuit()
    {
        var leadSuit = Suit.Hearts;
        var playerHand = new List<Card>
        {
            new(Suit.Hearts, Rank.Ace),
            new(Suit.Diamonds, Rank.King)
        };

        var gameDataManager = new GameDataManager();

        var validCards = new PlayManager(playerHand, gameDataManager, 
            new AutomatedPlayer("TestPlayer", 0, gameDataManager)).GetValidCards(leadSuit, Suit.Spades);

        Assert.Multiple(() =>
            {
                Assert.That(validCards?.Count, Is.EqualTo(1), "Should return one card matching the lead suit.");
                Assert.That(validCards?[0].Suit, Is.EqualTo(leadSuit), "The valid card should match the lead suit.");
            }
        );
    }

    // Behaviour: Getting valid cards returns all cards when the player has no cards matching the lead suit.

    [Test]
    public void GetValidCards_ReturnsAllCards_WhenNoMatchingLeadSuit()
    {
        var playerHand = new List<Card>
        { 
            new(Suit.Diamonds, Rank.Ace),
            new(Suit.Clubs, Rank.King),
            new(Suit.Clubs, Rank.Queen),
            new(Suit.Diamonds, Rank.Queen)
        };

        var gameDataManager = new GameDataManager();

        var validCards = new PlayManager(playerHand, gameDataManager, 
            new AutomatedPlayer("TestPlayer", 0, gameDataManager)).GetValidCards(Suit.Hearts, Suit.Spades);

        Assert.That(validCards?.Count, Is.EqualTo(playerHand.Count),
            "Should return all cards when no matching lead suit.");
    }

    // Behaviour: Getting valid cards should return an empty list when the hand is empty.

    [Test]
    public void GetValidCards_ReturnsEmpty_WhenHandIsEmpty()
    {
        var gameDataManager = new GameDataManager();

        var playerManager = new PlayManager([], gameDataManager, 
            new AutomatedPlayer("TestPlayer", 0, gameDataManager));
        var validCards = playerManager.GetValidCards(Suit.Hearts, Suit.Spades);

        Assert.That(validCards?.Count, Is.EqualTo(0), "Should return an empty list when hand is empty.");
    }
}
