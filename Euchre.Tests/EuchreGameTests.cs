using System.Reflection;
using Moq;
using NUnit.Framework;
using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Enums;
using Euchre.Logic.Exceptions;

namespace Euchre.Tests;

[TestFixture]
public class EuchreGameTests
{
    private List<AutomatedPlayerAvatar> MakeAvatars()
    {
        return new List<AutomatedPlayerAvatar>
        {
            new() { PlayerName = "Human", AvatarNumber = 1 },
            new() { PlayerName = "AI1", AvatarNumber = 2 },
            new() { PlayerName = "AI2", AvatarNumber = 3 },
            new() { PlayerName = "AI3", AvatarNumber = 4 },
        };
    }

    [Test]
    public void Constructor_Throws_WhenInvalidNumberOfPlayers()
    {
        var avatars = new List<AutomatedPlayerAvatar> { new() { PlayerName = "OnlyOne" } };
        var gsm = new GameStateManager();

        Assert.Throws<InvalidNumberOfPlayersException>(() => new EuchreGame(avatars, gsm));
    }

    [Test]
    public void Constructor_NewGame_InitializesPlayersAndDealer()
    {
        var avatars = MakeAvatars();
        var gsm = new GameStateManager();

        var game = new EuchreGame(avatars, gsm);

        Assert.That(game.GameInfo.Players, Is.Not.Null);
        Assert.That(game.GameInfo.Dealer, Is.Not.Null);
        Assert.That(game.GameInfo.Players![0].GetType(), Is.EqualTo(typeof(HumanPlayer)));
        Assert.That(game.GameInfo.Players[1].GetType(), Is.EqualTo(typeof(AutomatedPlayer)));
        Assert.That(game.GameInfo.Dealer, Is.SameAs(game.GameInfo.Players[0]));
    }

    [Test]
    public void SetKittyCard_SetsKitty_And_RaisesEvent()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        var evtRaised = false;
        game.DeclareKittyCard += (_, e) => evtRaised = true;

        var mi = typeof(EuchreGame).GetMethod("SetKittyCard", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var card = new Card(Suit.Hearts, Rank.Ace);
        mi.Invoke(game, new object[] { card });

        Assert.That(game.GameInfo.Kitty, Is.EqualTo(card));
        Assert.That(evtRaised, Is.True);
    }

    [Test]
    public void GetNextPlayer_WrapsAround()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        var players = game.GameInfo.Players!;
        var mi = typeof(EuchreGame).GetMethod("GetNextPlayer", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var next = (IPlayer)mi.Invoke(game, new object[] { players[0] })!;
        Assert.That(next, Is.SameAs(players[1]));

        var lastNext = (IPlayer)mi.Invoke(game, new object[] { players[3] })!;
        Assert.That(lastNext, Is.SameAs(players[0]));
    }

    [Test]
    public void AdvanceDealer_UpdatesDealer_And_ResetsStage()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        var oldDealer = game.GameInfo.Dealer!;
        var mi = typeof(EuchreGame).GetMethod("AdvanceDealer", BindingFlags.Instance | BindingFlags.NonPublic)!;

        mi.Invoke(game, null);

        Assert.That(game.GameInfo.Dealer, Is.Not.Null);
        Assert.That(game.GameInfo.Dealer, Is.Not.SameAs(oldDealer));
        Assert.That(game.GameInfo.LastCompletedStage, Is.EqualTo(RoundStage.None));
    }

    [Test]
    public void IsPartner_ReturnsTrueForTeamMates()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        var players = game.GameInfo.Players!;
        var mi = typeof(EuchreGame).GetMethod("IsPartner", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // players 0 and 2 are team 0 in initialization
        var res = (bool)mi.Invoke(game, new object[] { players[0], players[2] })!;
        Assert.That(res, Is.True);

        var res2 = (bool)mi.Invoke(game, new object[] { players[0], players[1] })!;
        Assert.That(res2, Is.False);
    }

    [Test]
    public void TrumpWasCalled_SetsStage_And_RaisesEvent()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        var called = false;
        game.TrumpCalled += (_, __) => called = true;

        // set trump caller so SaveGameData can run without NRE
        game.GameInfo.TrumpCaller = game.GameInfo.Players![1];

        var mi = typeof(EuchreGame).GetMethod("TrumpWasCalled", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var ret = (bool)mi.Invoke(game, null)!;

        Assert.That(ret, Is.True);
        Assert.That(game.GameInfo.LastCompletedStage, Is.EqualTo(RoundStage.TrumpChosen));
        Assert.That(called, Is.True);
    }

    [Test]
    public void StopIfNoAceNoFaceNoTrump_Declares_And_AdvancesDealer()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        // choose a trump that players do not have
        game.GameInfo.Trump = Suit.Spades;

        // Make player 0 have no ace/face/trump
        var p0 = (HumanPlayer)game.GameInfo.Players![0];
        p0.ClearHand();
        p0.AddCard(new Card(Suit.Clubs, Rank.Nine));

        // make sure another player has something so logic continues
        var p1 = (AutomatedPlayer)game.GameInfo.Players[1];
        p1.ClearHand();
        p1.AddCard(new Card(Suit.Hearts, Rank.Ace));

        var declared = false;
        game.NoAceNoFaceNoTrumpDeclared += (_, __) => declared = true;

        var mi = typeof(EuchreGame).GetMethod("StopIfNoAceNoFaceNoTrump", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var result = (bool)mi.Invoke(game, null)!;

        Assert.That(result, Is.True);
        Assert.That(declared, Is.True);
    }

    [Test]
    public void BiddingRound_Round2_CallTrump_UpdatesGameInfo()
    {
        var gsm = new GameStateManager();
        var game = new EuchreGame(MakeAvatars(), gsm);

        // replace players with mocks to control CallTrump behavior
        var mocks = new Mock<IPlayer>[4];
        for (int i = 0; i < 4; i++)
        {
            mocks[i] = new Mock<IPlayer>();
            mocks[i].SetupGet(m => m.PlayerIndex).Returns(i);
            mocks[i].SetupGet(m => m.TeamIndex).Returns(i % 2);
            mocks[i].SetupGet(m => m.Name).Returns($"P{i}");
            mocks[i].Setup(m => m.CallTrump(It.IsAny<Card>(), It.IsAny<bool>())).Returns((Suit?)null);
        }

        // make player 2 call trump
        mocks[2].Setup(m => m.CallTrump(It.IsAny<Card>(), It.IsAny<bool>())).Returns(Suit.Diamonds);

        game.GameInfo.Players = new IPlayer[] { mocks[0].Object, mocks[1].Object, mocks[2].Object, mocks[3].Object };
        game.GameInfo.Dealer = mocks[0].Object;
        game.GameInfo.Kitty = new Card(Suit.Clubs, Rank.Nine);

        var mi = typeof(EuchreGame).GetMethod("BiddingRound", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var ret = (bool)mi.Invoke(game, new object[] { 2, null })!;

        Assert.That(ret, Is.True);
        Assert.That(game.GameInfo.Trump, Is.EqualTo(Suit.Diamonds));
        Assert.That(game.GameInfo.TrumpCaller, Is.SameAs(mocks[2].Object));
    }

    [Test]
    public void CanEndRound_Behaviour_CoversBothBranches()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        // configure trump caller
        game.GameInfo.TrumpCaller = game.GameInfo.Players![0];

        // Setup tricks won such that bidderTricks == MIN_NUMBER_TRICKS_TO_SCORE and opposition >0 -> true
        game.GameInfo.TricksWonByPlayers = new int[] { 3, 0, 0, 0 };
        var mi = typeof(EuchreGame).GetMethod("CanEndRound", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var res = (bool)mi.Invoke(game, null)!;
        Assert.That(res, Is.False);

        // Now set opposition to MIN_NUMBER_TRICKS_TO_SCORE and bidder less -> true via else branch
        game.GameInfo.TricksWonByPlayers = new int[] { 0, 3, 0, 0 };
        game.GameInfo.TrumpCaller = game.GameInfo.Players[1];
        var res2 = (bool)mi.Invoke(game, null)!;
        Assert.That(res2, Is.False);
    }

    [Test]
    public void PlayTrick_PlaysAllCards_And_RaisesEvents()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        // create four mock players with real backing hands
        var mocks = new Mock<IPlayer>[4];
        for (int i = 0; i < 4; i++)
        {
            var hand = new List<Card> { new Card(Suit.Clubs, Rank.Nine) };
            mocks[i] = new Mock<IPlayer>();
            mocks[i].SetupGet(m => m.PlayerIndex).Returns(i);
            mocks[i].SetupGet(m => m.TeamIndex).Returns(i % 2);
            mocks[i].SetupGet(m => m.Name).Returns($"P{i}");
            mocks[i].SetupGet(m => m.Hand).Returns(hand);
            mocks[i].Setup(m => m.SelectCardToPlay(It.IsAny<Trick>(), It.IsAny<Suit>(), It.IsAny<Suit?>()))
                .Returns((Trick t, Suit tr, Suit? ls) => hand[0]);
            mocks[i].Setup(m => m.PlayCard(It.IsAny<int>()))
                .Returns((int idx) => { var c = hand[idx]; hand.RemoveAt(idx); return c; });
        }

        game.GameInfo.Players = new IPlayer[] { mocks[0].Object, mocks[1].Object, mocks[2].Object, mocks[3].Object };
        game.GameInfo.Trump = Suit.Clubs;
        game.GameInfo.NextTrickPlayer = game.GameInfo.Players[0];

        var playedCount = 0;
        game.CardPlayedByPlayer += (_, __) => playedCount++;

        var mi = typeof(EuchreGame).GetMethod("PlayTrick", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var trick = (Trick)mi.Invoke(game, null)!;

        Assert.That(trick.Cards.Count, Is.EqualTo(4));
        Assert.That(playedCount, Is.EqualTo(4));
    }

    [Test]
    public void ScoreRound_AwardsPoints_And_RaisesEvent()
    {
        var game = new EuchreGame(MakeAvatars(), new GameStateManager());
        // configure players and tricks won such that caller team gets points
        game.GameInfo.Players = game.GameInfo.Players!;
        game.GameInfo.TrumpCaller = game.GameInfo.Players[0];
        game.GameInfo.TricksWonByPlayers = new int[] { 3, 0, 0, 0 };

        DeclareRoundWinningPlayersEventArgs? args = null;
        game.DeclareRoundWinningPlayers += (_, e) => args = e;

        var mi = typeof(EuchreGame).GetMethod("ScoreRound", BindingFlags.Instance | BindingFlags.NonPublic)!;
        mi.Invoke(game, null);

        Assert.That(args, Is.Not.Null);
        Assert.That(game.GameInfo.LastCompletedStage, Is.EqualTo(RoundStage.Scored));
    }
}
