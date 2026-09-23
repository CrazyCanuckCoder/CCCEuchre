using Euchre.Logic.Components;
using Euchre.Logic.Helpers;

namespace Euchre.Tests;

[TestFixture]
public class RoundHelperTests
{
    private static GameStateManager CreateGameStateWithPlayers(out AutomatedPlayer leadingPlayer,
        out AutomatedPlayer p1, out AutomatedPlayer p2, out AutomatedPlayer p3)
    {
        var gsm = new GameStateManager();

        leadingPlayer = new AutomatedPlayer("Lead", 0, 0, gsm, 1);
        p1 = new AutomatedPlayer("P1", 1, 1, gsm, 2);
        p2 = new AutomatedPlayer("P2", 0, 2, gsm, 3);
        p3 = new AutomatedPlayer("P3", 1, 3, gsm, 4);

        gsm.Players = [leadingPlayer, p1, p2, p3];

        return gsm;
    }


    // Behaviour: For the PlayerCanWinRemainingTricks method, when the leading player has 3 remaining cards
    //            that are the right bower, left bower, and ace of trump, and the other players have a 
    //            combination of cards that cannot beat any of the leading player's cards, the method should
    //            return true.

    [Test]
    public void LeadingPlayerHasRightLeftAceOfTrump_OthersCantBeat_ReturnsTrue()
    {
        var gsm = CreateGameStateWithPlayers(out var lead, out var p1, out var p2, out var p3);

        gsm.Trump = Suit.Hearts;

        // Leading player: Right bower (J Hearts), Left bower (J Diamonds), Ace Hearts
        lead.AddCard(new Card(Suit.Hearts, Rank.Jack));
        lead.AddCard(new Card(Suit.Diamonds, Rank.Jack));
        lead.AddCard(new Card(Suit.Hearts, Rank.Ace));

        // Other players: No trump, low off-suit cards
        p1.AddCard(new Card(Suit.Clubs, Rank.Nine));
        p1.AddCard(new Card(Suit.Clubs, Rank.Ten));
        p1.AddCard(new Card(Suit.Spades, Rank.Nine));

        p2.AddCard(new Card(Suit.Clubs, Rank.King));
        p2.AddCard(new Card(Suit.Clubs, Rank.Queen));
        p2.AddCard(new Card(Suit.Spades, Rank.King));

        p3.AddCard(new Card(Suit.Diamonds, Rank.Nine));
        p3.AddCard(new Card(Suit.Diamonds, Rank.Ten));
        p3.AddCard(new Card(Suit.Diamonds, Rank.Queen));

        var result = RoundHelper.PlayerCanWinRemainingTricks(lead, gsm);

        Assert.IsTrue(result);
    }

    // Behaviour: For the PlayerCanWinRemainingTricks method, when the leading player has 3 remaining cards
    //            that are the left bower, ace of trump, and king of trump, and the other players have a 
    //            combination of cards with one of the players having the right bower, the method should
    //            return false.

    [Test]
    public void LeadingPlayerHasLeftAceKing_OtherHasRightBower_ReturnsFalse()
    {
        var gsm = CreateGameStateWithPlayers(out var lead, out var p1, out var p2, out var p3);

        gsm.Trump = Suit.Spades;

        // Leading player: Left bower (J Clubs for spades), Ace Spades, King Spades
        lead.AddCard(new Card(Suit.Clubs, Rank.Jack));
        lead.AddCard(new Card(Suit.Spades, Rank.Ace));
        lead.AddCard(new Card(Suit.Spades, Rank.King));

        // One of the other players has the right bower (J Spades)
        p1.AddCard(new Card(Suit.Spades, Rank.Jack));
        p1.AddCard(new Card(Suit.Clubs, Rank.Ace));
        p1.AddCard(new Card(Suit.Diamonds, Rank.Ace));

        p2.AddCard(new Card(Suit.Hearts, Rank.Ace));
        p2.AddCard(new Card(Suit.Hearts, Rank.King));
        p2.AddCard(new Card(Suit.Clubs, Rank.Ten));

        p3.AddCard(new Card(Suit.Hearts, Rank.Queen));
        p3.AddCard(new Card(Suit.Hearts, Rank.Ten));
        p3.AddCard(new Card(Suit.Diamonds, Rank.Queen));

        var result = RoundHelper.PlayerCanWinRemainingTricks(lead, gsm);

        Assert.IsFalse(result);
    }

    // Behaviour: For the PlayerCanWinRemainingTricks method, when the leading player has 4 remaining cards
    //            that are high cards in two suits, and the other players have a combination of cards that
    //            do not contain trump and cannot beat any of the leading player's cards, the method should
    //            return true.

    [Test]
    public void LeadingPlayerHasHighCardsInTwoSuits_OthersNoTrump_ReturnsTrue()
    {
        var gsm = CreateGameStateWithPlayers(out var lead, out var p1, out var p2, out var p3);

        gsm.Trump = Suit.Clubs;

        // Leading player: high cards in Hearts and Diamonds
        lead.AddCard(new Card(Suit.Hearts, Rank.Ace));
        lead.AddCard(new Card(Suit.Hearts, Rank.King));
        lead.AddCard(new Card(Suit.Diamonds, Rank.Ace));
        lead.AddCard(new Card(Suit.Diamonds, Rank.King));

        // Other players: no trump (clubs) and cannot beat the high hearts/diamonds
        p1.AddCard(new Card(Suit.Spades, Rank.Nine));
        p1.AddCard(new Card(Suit.Spades, Rank.Ten));
        p1.AddCard(new Card(Suit.Spades, Rank.Queen));
        p1.AddCard(new Card(Suit.Spades, Rank.King));

        p2.AddCard(new Card(Suit.Hearts, Rank.Ten));
        p2.AddCard(new Card(Suit.Hearts, Rank.Queen));
        p2.AddCard(new Card(Suit.Diamonds, Rank.Ten));
        p2.AddCard(new Card(Suit.Diamonds, Rank.Queen));

        p3.AddCard(new Card(Suit.Hearts, Rank.Nine));
        p3.AddCard(new Card(Suit.Diamonds, Rank.Nine));
        p3.AddCard(new Card(Suit.Spades, Rank.Queen));
        p3.AddCard(new Card(Suit.Spades, Rank.King));

        var result = RoundHelper.PlayerCanWinRemainingTricks(lead, gsm);

        Assert.IsTrue(result);
    }

    // Behaviour: For the PlayerCanWinRemainingTricks method, when the leading player has 4 remaining cards
    //            that are cards in two suits, and the other players have a combination of cards that contain
    //            trump and can beat the leading player's cards, the method should return false.

    [Test]
    public void LeadingPlayerHasCards_OthersHaveTrump_ReturnsFalse()
    {
        var gsm = CreateGameStateWithPlayers(out var lead, out var p1, out var p2, out var p3);

        gsm.Trump = Suit.Diamonds;

        // Leading player: high cards in Hearts and Clubs
        lead.AddCard(new Card(Suit.Hearts, Rank.Ace));
        lead.AddCard(new Card(Suit.Hearts, Rank.King));
        lead.AddCard(new Card(Suit.Clubs, Rank.Ace));
        lead.AddCard(new Card(Suit.Clubs, Rank.King));

        // Other players: contain trump cards which could beat non-trump leads
        p1.AddCard(new Card(Suit.Diamonds, Rank.Nine));
        p3.AddCard(new Card(Suit.Clubs, Rank.King));
        p1.AddCard(new Card(Suit.Spades, Rank.Nine));
        p1.AddCard(new Card(Suit.Spades, Rank.Ten));

        p2.AddCard(new Card(Suit.Diamonds, Rank.Ace));
        p2.AddCard(new Card(Suit.Hearts, Rank.Ten));
        p2.AddCard(new Card(Suit.Hearts, Rank.Nine));
        p2.AddCard(new Card(Suit.Clubs, Rank.Ten));

        p3.AddCard(new Card(Suit.Spades, Rank.King));
        p3.AddCard(new Card(Suit.Spades, Rank.Queen));
        p3.AddCard(new Card(Suit.Spades, Rank.Ten));
        p3.AddCard(new Card(Suit.Clubs, Rank.Queen));

        var result = RoundHelper.PlayerCanWinRemainingTricks(lead, gsm);

        Assert.IsFalse(result);
    }

    [Test]
    public void GoingAlone_RemovesPartnerFromConsideration_ReturnsTrue()
    {
        var gsm = CreateGameStateWithPlayers(out var p0, out var p1, out var p2, out var p3);

        gsm.Trump = Suit.Hearts;

        // We'll test with p1 as the leading player. p2 is going alone so their partner (p0) should be removed
        // from consideration. p0 would have a trump that could beat p1's cards if not removed.
        p1.AddCard(new Card(Suit.Hearts, Rank.Ace));
        p1.AddCard(new Card(Suit.Hearts, Rank.King));

        // p0 (partner of p2) has the right bower which would beat p1's Ace if considered
        p0.AddCard(new Card(Suit.Hearts, Rank.Jack));

        // p2 is going alone and does not have trump that can beat p1
        p2.AddCard(new Card(Suit.Spades, Rank.Nine));

        gsm.GoingAlone = true;
        gsm.AlonePlayer = p2;

        var result = RoundHelper.PlayerCanWinRemainingTricks(p1, gsm);

        Assert.IsTrue(result);
    }

    [Test]
    public void LeadingPlayerWithEmptyHand_ThrowsException()
    {
        var gsm = CreateGameStateWithPlayers(out var lead, out var p1, out var p2, out var p3);
        gsm.Trump = Suit.Clubs;

        // Leading player's hand is empty by default
        Assert.Throws<ArgumentException>(() => RoundHelper.PlayerCanWinRemainingTricks(lead, gsm));
    }
}
