using Euchre.Logic.Components;
using Euchre.Logic.Exceptions;
using Euchre.Logic.Helpers;

namespace Euchre.Tests;

[TestFixture]
public class PlayManagerTests
{
    private GameStateManager _gameState;
    private AutomatedPlayer[] _players = new AutomatedPlayer[4];

    [SetUp]
    public void Setup()
    {
        _gameState = new GameStateManager();

        // Create four automated players. Team 0: players 0 and 2. Team 1: players 1 and 3.
        _players[0] = new AutomatedPlayer("P0", 0, 0, _gameState, 1);
        _players[1] = new AutomatedPlayer("P1", 1, 1, _gameState, 1);
        _players[2] = new AutomatedPlayer("P2", 0, 2, _gameState, 1);
        _players[3] = new AutomatedPlayer("P3", 1, 3, _gameState, 1);

        _gameState.Players = [.. _players.Cast<Logic.Interfaces.IPlayer>()];

        // Ensure hands are clear before each test.
        foreach (var p in _players) p.ClearHand();
    }

    [Test]
    public void DetermineCardToPlay_Throws_WhenHandEmpty()
    {
        var pm = new PlayManager(_gameState, _players[0]);

        var trick = new Trick(Suit.Hearts);

        Assert.Throws<InvalidGameConditionException>(() => pm.DetermineCardToPlay(trick, Suit.Hearts, null));
    }

    [Test]
    public void DetermineCardToPlay_ReturnsOnlyCard_WhenSingleCardInHand()
    {
        var player = _players[0];
        player.AddCard(new Card(Suit.Clubs, Rank.Ace));

        var pm = new PlayManager(_gameState, player);
        var trick = new Trick(Suit.Hearts);

        var result = pm.DetermineCardToPlay(trick, Suit.Hearts, null);

        Assert.That(result, Is.EqualTo(new Card(Suit.Clubs, Rank.Ace)));
    }

    [Test]
    public void DetermineCardToPlay_Leading_NonCaller_LeadsHighestOffSuit()
    {
        // Setup trump is Hearts. Trump caller is player 1 so player 0 is not caller and not partner.
        _gameState.Trump = Suit.Hearts;
        _gameState.TrumpCaller = _players[1];

        var player = _players[0];
        // Give player an off-suit Ace of Clubs and a lower trump.
        player.AddCard(new Card(Suit.Clubs, Rank.Ace));
        player.AddCard(new Card(Suit.Hearts, Rank.Nine));

        var pm = new PlayManager(_gameState, player);
        var trick = new Trick(Suit.Hearts);

        var result = pm.DetermineCardToPlay(trick, Suit.Hearts, null);

        Assert.That(result, Is.EqualTo(new Card(Suit.Clubs, Rank.Ace)));
    }

    [Test]
    public void DetermineCardToPlay_Leading_CallerGoingAlone_LeadsHighestTrump()
    {
        _gameState.Trump = Suit.Spades;
        SetTrumpCallerAndGoingAlone(_players[2]);

        var player = _players[2];
        player.AddCard(new Card(Suit.Spades, Rank.King));
        player.AddCard(new Card(Suit.Diamonds, Rank.Ace));

        var pm = new PlayManager(_gameState, player);
        var trick = new Trick(Suit.Spades);

        var result = pm.DetermineCardToPlay(trick, Suit.Spades, null);

        Assert.That(result, Is.EqualTo(new Card(Suit.Spades, Rank.King)));
    }

    private void SetTrumpCallerAndGoingAlone(AutomatedPlayer caller)
    {
        _gameState.TrumpCaller = caller;
        _gameState.GoingAlone = true;
    }

    [Test]
    public void DetermineCardToPlay_Following_PlayHigherOfLeadSuit_WhenAvailable()
    {
        // Setup players and trick: lead suit is Clubs and trump is Hearts.
        _gameState.Trump = Suit.Hearts;

        var leadPlayer = _players[0];
        var follower = _players[1];

        // Lead player plays Ten of Clubs.
        var trick = new Trick(Suit.Hearts);
        trick.AddCard(leadPlayer, new Card(Suit.Clubs, Rank.Ten));

        // Follower has King of Clubs and Nine of Hearts.
        follower.AddCard(new Card(Suit.Clubs, Rank.King));
        follower.AddCard(new Card(Suit.Hearts, Rank.Nine));

        var pm = new PlayManager(_gameState, follower);

        var result = pm.DetermineCardToPlay(trick, Suit.Hearts, trick.LeadSuit);

        Assert.That(result, Is.EqualTo(new Card(Suit.Clubs, Rank.King)));
    }
}
