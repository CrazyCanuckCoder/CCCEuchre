using Euchre.Logic.Components;
using Euchre.Logic.Helpers;

namespace Euchre.Tests;

[TestFixture]
public class BidderTests
{
    // The test data set to use with the DetermineWhetherToOrderUp method.

    private static IEnumerable<TestCaseData> GetOrderUpInformation()
    {
        // Behaviour: Player has none of the suit matching the kitty card so DetermineWhetherToOrderUp should
        //            return false.

        yield return new TestCaseData(
             
            // Player's hand.

            new List<Card>
            {
                new(Suit.Hearts, Rank.Ace),
                new(Suit.Hearts, Rank.King),
                new(Suit.Hearts, Rank.Queen),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Diamonds, Rank.Queen)
            },

            // Kitty card.
            
            new Card(Suit.Clubs, Rank.Jack),

            // Not the dealer.
            
            false,

            // Expected go alone status.

            false,

            // Expected result.

            false
        ).SetName("OrderUpTest01");

        // Behaviour: Player has two cards of the suit matching the kitty card without a bower, they are not
        //            the dealer, so DetermineWhetherToOrderUp should return false.

        yield return new TestCaseData(

            // Player's hand.

            new List<Card>
            {
                new(Suit.Clubs, Rank.Ace),
                new(Suit.Clubs, Rank.King),
                new(Suit.Hearts, Rank.Queen),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Diamonds, Rank.Queen)
            },

            // Kitty card.

            new Card(Suit.Clubs, Rank.Jack),

            // Not the dealer.

            false,

            // Expected go alone status.

            false,

            // Expected result.

            false
        ).SetName("OrderUpTest02");

        // Behaviour: Player has two cards of the suit matching the kitty card without a bower, they are the
        //            dealer, which includes the kitty card when evaluating whether to order up, so
        //            DetermineWhetherToOrderUp should return true.

        yield return new TestCaseData(

            // Player's hand.

            new List<Card>
            {
                new(Suit.Clubs, Rank.Ace),
                new(Suit.Clubs, Rank.King),
                new(Suit.Hearts, Rank.Queen),
                new(Suit.Diamonds, Rank.King),
                new(Suit.Diamonds, Rank.Queen)
            },

            // Kitty card.

            new Card(Suit.Clubs, Rank.Jack),

            // Player is the dealer.

            true,

            // Expected go alone status.

            false,

            // Expected result.

            true
        ).SetName("OrderUpTest03");
    }

    [Test, TestCaseSource(nameof(GetOrderUpInformation))]
    public void GivenPlayingConditions_DetermineWhetherToOrderUp_ShouldReturnCorrectResult(
        List<Card> playerHand, Card kitty, bool isDealer, bool expectedGoAlone, bool expectedResult)
    {
        var bidder = new Bidder(playerHand);
        bool result = bidder.DetermineWhetherToOrderUp(kitty, isDealer, out bool goAlone);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(goAlone, Is.EqualTo(expectedGoAlone));
        });
    }

}
