using Euchre.Logic.Exceptions;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using static Euchre.Logic.Helpers.Constants;

namespace Euchre.Logic.Components;

/// <summary>
/// The base class for all players in the game of Euchre.
/// </summary>
public abstract class Player : IPlayer
{
    /// <summary>
    /// Creates a new player with the specified name and whether they are human or not.
    /// </summary>
    /// <param name="name">The name of the player.</param>
    /// <param name="teamIndex"> The index of the team this player is associated with, starting from 0.</param>
    /// <param name="isHuman">True if the player is a human and false for automated players.</param>
    /// <param name="avatarNumber">The number indicating which avatar the user is represented by in the UI.</param>
    public Player(string name, int teamIndex, int playerIndex, bool isHuman, int avatarNumber)
    {
        Name = name;
        Hand = [];
        TeamIndex = teamIndex;
        PlayerIndex = playerIndex;
        IsHuman = isHuman;
        AvatarNumber = avatarNumber;
    }

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// The number representing the avatar to display on the UI for the player.
    /// </summary>
    public int AvatarNumber { get; protected set; }

    /// <summary>
    /// The cards in the player's hand.
    /// </summary>
    public List<Card> Hand { get; protected set; }

    /// <summary>
    /// True if the player is a human player, false if the player is an automated player.
    /// </summary>
    public bool IsHuman { get; protected set; }

    /// <summary>
    /// Gets the index of the team associated with this player.
    /// </summary>
    public int TeamIndex { get; protected set; }

    /// <summary>
    /// Gets/sets the index number of the player in the list of players.
    /// </summary>
    public int PlayerIndex { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the individual is going alone.
    /// </summary>
    public abstract bool IsGoingAlone { get; set; }

    /// <summary>
    /// Adds a card to the player's hand.
    /// </summary>
    /// <param name="card">The card to add to the player's hand. Must not already exist in the hand.</param>
    /// <exception cref="TooManyCardsException" />
    /// <exception cref="CardAlreadyExistsException" />
    public void AddCard(Card card)
    {
        if (Hand.Count >= CARDS_PER_PLAYER)
        {
            throw new TooManyCardsException();
        }

        if (Hand.Contains(card))
        {
            throw new CardAlreadyExistsException();
        }

        Hand.Add(card);
    }

    /// <summary>
    /// Adds a list of cards to the player's hand.
    /// </summary>
    /// <param name="cardList">A list of playing cards.</param>
    /// <returns>True to indicate all of the cards were added to the player's hand.</returns>
    /// <exception cref="DuplicateCardException"></exception>
    public void ReceiveSeveralCards(List<ICard> cardList)
    {
        foreach (ICard card in cardList)
        {
            AddCard((Card)card);
        }
    }

    /// <summary>
    /// Clears the player's hand, removing all cards.
    /// </summary>
    public void ClearHand()
    {
        Hand.Clear();
    }

    /// <summary>
    /// Resorts the player's hand based on a specified trump suit.
    /// </summary>
    /// <param name="trump">The suit set as trump.  Null indicates no trump set.</param>
    public void SortPlayerCards(Suit? trump)
    {
        Hand = Hand.Sort(trump);
    }

    /// <summary>
    /// Removes a card from the player's hand at the specified index to simulate playing a card.
    /// </summary>
    /// <param name="index">The index of the card to play.</param>
    /// <returns>The card played.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Card PlayCard(int index)
    {
        if (index < 0 || index >= Hand.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var card = Hand[index];
        Hand.RemoveAt(index);
        return card;
    }

    /// <summary>
    /// Prompts the player to call trump for the game.
    /// </summary>
    /// <param name="kitty">The card from the kitty indicating which suit cannot be called trump.</param>
    /// <returns>The trump suit selected by the user, or <see langword="null"/> if no trump suit is 
    /// selected which indicates the automated player will pass.</returns>
    public abstract Suit? CallTrump(Card kitty, bool isDealer);

    /// <summary>
    /// Determines which card in the player's hand will be replaced by the kitty's upturned card.
    /// </summary>
    /// <param name="kitty">The upturned card from the kitty.</param>
    public abstract void DiscardForKitty(Card kitty);

    /// <summary>
    /// Determines whether to order up the given card during the bidding phase.
    /// </summary>
    /// <param name="kitty">The card being considered for ordering up.</param>
    /// <param name="isDealer">A boolean value indicating whether the current player is the dealer.</param>
    /// <param name="goUnder">A boolean value indicating whether the player chooses to go under.</param>
    /// <returns><see langword="true"/> if the player decides to order up the card; otherwise, 
    /// <see langword="false"/>.</returns>
    public abstract bool OrderUp(Card kitty, bool isDealer, out bool goUnder);

    /// <summary>
    /// Selects the card to play for the current trick based on the provided game state.
    /// </summary>
    /// <param name="trick">The current trick containing the cards played so far.</param>
    /// <param name="trump">The trump suit for the game.</param>
    /// <param name="leadSuit">The suit that was led for the trick, or <see langword="null"/> if no suit has 
    /// been led yet.</param>
    /// <returns>The card selected to play in the current trick.</returns>
    public abstract Card SelectCardToPlay(Trick trick, Suit trump, Suit? leadSuit);

    /// <summary>
    /// Copies the values of this object.
    /// </summary>
    /// <returns>An IPlayer containing the same properties as this object.</returns>
    public abstract IPlayer Clone();

    /// <summary>
    /// Checks the player's hand for a lack of Aces, cards above 10, and trump.
    /// </summary>
    /// <param name="trump">The current trump suit.</param>
    /// <returns>Returns true to indicate there are no Aces, cards above 10, or trump in the player's hand.</returns>
    public virtual bool HasNoAceNoFaceNoTrump(Suit trump)
    {
        if (GameSettingsManager.Instance.NoAceNoFaceNoTrumpRule)
        {
            var numAces = CardFinder.CountCardsOfRank(Hand, Rank.Ace);
            var numFaces = CardFinder.CountCardsOfRank(Hand, Rank.King) +
                           CardFinder.CountCardsOfRank(Hand, Rank.Queen) +
                           CardFinder.CountCardsOfRank(Hand, Rank.Jack);
            var numTrump = CardFinder.CountTrump(Hand, trump);

            return numAces + numFaces + numTrump == 0;
        }

        return false;
    }

    /// <summary>
    /// Retrieves a list of cards from the player's hand that are eligible to be placed under the kitty when
    /// going under.
    /// </summary>
    /// <param name="kittyCards">The kitty cards that will be added to the player's hand.</param>
    /// <returns>A list of cards from the player's hand to be placed into the kitty.</returns>
    public abstract List<Card> GetGoUnderCards(List<Card> kittyCards);
}