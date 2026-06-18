using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Euchre.Windows;
/// <summary>
/// Interaction logic for ChooseCardsForPlayersWindow.xaml
/// </summary>
public partial class ChooseCardsForPlayersWindow : Window
{
    public ChooseCardsForPlayersWindow()
    {
        InitializeComponent();
        SetupAvailableCards();
        DataContext = this;
    }

    /// <summary>
    /// Specifies the clipboard data format name used for card objects.
    /// </summary>
    private const string CardClipBoardDataFormat = "Card";

    /// <summary>
    /// The margin to apply to each card.
    /// </summary>
    private readonly Thickness _cardMargin = new(5, 8, 0, 0);

    /// <summary>
    /// Contains the images for the cards in the Clubs suit.
    /// </summary>
    public ObservableCollection<CardDisplay> Clubs { get; private set; } = [];

    /// <summary>
    /// Contains the images for the cards in the Hearts suit.
    /// </summary>
    public ObservableCollection<CardDisplay> Hearts { get; private set; } = [];

    /// <summary>
    /// Contains the images for the cards in the Spades suit.
    /// </summary>
    public ObservableCollection<CardDisplay> Spades { get; private set; } = [];

    /// <summary>
    /// Contains the images for the cards in the Diamonds suit.
    /// </summary>
    public ObservableCollection<CardDisplay> Diamonds { get; private set; } = [];

    /// <summary>
    /// Contains the images for the cards selected for Player 1.
    /// </summary>
    public ObservableCollection<CardDisplay> Player1CardImages { get; private set; } = [];

    /// <summary>
    /// Contains the images for the cards selected for Player 2.
    /// </summary>
    public ObservableCollection<CardDisplay> Player2CardImages { get; private set; } = [];

    /// <summary>
    /// Contains the images for the cards selected for Player 3.
    /// </summary>
    public ObservableCollection<CardDisplay> Player3CardImages { get; private set; } = [];

    /// <summary>
    /// Contains the images for the cards selected for Player 4.
    /// </summary>
    public ObservableCollection<CardDisplay> Player4CardImages { get; private set; } = [];

    /// <summary>
    /// Contains the cards selected for Player 1.
    /// </summary>
    public List<ICard> Player1Cards { get; private set; } = [];

    /// <summary>
    /// Contains the cards selected for Player 2.
    /// </summary>
    public List<ICard> Player2Cards { get; private set; } = [];

    /// <summary>
    /// Contains the cards selected for Player 3.
    /// </summary>
    public List<ICard> Player3Cards { get; private set; } = [];

    /// <summary>
    /// Contains the cards selected for Player 4.
    /// </summary>
    public List<ICard> Player4Cards { get; private set; } = [];

    /// <summary>
    /// Contains the image of the card selected for the kitty.
    /// </summary>
    public ObservableCollection<CardDisplay> KittyCardImage { get; private set; } = [];

    /// <summary>
    /// Gets the card designated as the kitty card.
    /// </summary>
    public ICard KittyCard { get; private set; }

    /// <summary>
    /// Contains the cards left over that were not assigned to a player or as the kitty card.
    /// </summary>
    public List<Card> RemainingCards { get; private set; } = [];

    /// <summary>
    /// Initializes the collections of available cards for each suit using a new deck.
    /// </summary>
    private void SetupAvailableCards()
    {
        Deck deck = new();

        // Grab all the cards for each suit from the new deck.

        AddSuitCardsToCollection(deck, Suit.Clubs,    Clubs);
        AddSuitCardsToCollection(deck, Suit.Hearts,   Hearts);
        AddSuitCardsToCollection(deck, Suit.Spades,   Spades);
        AddSuitCardsToCollection(deck, Suit.Diamonds, Diamonds);
    }

    /// <summary>
    /// Gets a list of the cards from the deck for a specified suit and adds it to the specified suit
    /// collection.
    /// </summary>
    /// <param name="deck">The deck of cards containing the cards for the specified suit.</param>
    /// <param name="suit">The suit to find cards for.</param>
    /// <param name="suitCollection">The collection of images to add the found cards to.</param>
    private void AddSuitCardsToCollection(Deck deck, Suit suit, 
        ObservableCollection<CardDisplay> suitCollection)
    {
        var suitCards =   from card in deck.Cards
                         where card.Suit == suit
                        select card;
        foreach (var eachCard in suitCards)
        {
            // Add the image to the current suit collection.

            suitCollection.Add(new CardDisplay()
            {
                ImageData = new Image()
                {
                    Source = UIHelpers.GenerateCardImageSource(eachCard, false, false)
                },
                CardVisibility = Visibility.Visible,
                ImageVisibility = Visibility.Visible,
                Card = eachCard,
                CardMargin = _cardMargin,
                IsEnabled = true
            });
        }
    }

    /// <summary>
    /// Checks the specified collection for a specified card and returns the CardDisplay for the card, if
    /// it found.  If the card is not found, null is returned.
    /// </summary>
    /// <param name="availableCollection">The collection of cards to check.</param>
    /// <param name="cardToFind">The card to find in the collection.</param>
    /// <returns>Null if the card is not found; otherwise, the CardDisplay information for the found
    /// card.</returns>
    private CardDisplay? GetCardFromAvailableCollection(IEnumerable<CardDisplay> availableCollection,
        ICard cardToFind)
    {
        CardDisplay? foundCard = null;
        var foundCards =   from cardDisplay in availableCollection
                          where cardDisplay.Card == cardToFind
                         select cardDisplay;
        if (foundCards.Any())
        {
            foundCard = foundCards.First();
        }

        return foundCard;
    }

    /// <summary>
    /// Adds the specified card to the kitty, replacing any existing card if present.
    /// </summary>
    /// <param name="draggedCard">The card to add to the kitty.</param>
    private void AddCardToKitty(ICard draggedCard)
    {
        // Ensure only one card can be added to the kitty.

        if (KittyCardImage.Count > 0)
        {
            ReturnKittyCardToItsCollection();
            KittyCardImage.Clear();
        }

        GetDisplayCardReferences(draggedCard,
            out ObservableCollection<CardDisplay>? availableCardsCollectionRef,
            out CardDisplay? foundCard);

        MoveCardFromAvailableCardToKitty(draggedCard, availableCardsCollectionRef, foundCard);
    }

    /// <summary>
    /// Returns the existing kitty card to its original suit collection.
    /// </summary>
    private void ReturnKittyCardToItsCollection()
    {
        switch (KittyCard.Suit)
        {
            case Suit.Clubs:
                Clubs.Add(KittyCardImage.First());
                break;

            case Suit.Hearts:
                Hearts.Add(KittyCardImage.First());
                break;

            case Suit.Spades:
                Spades.Add(KittyCardImage.First());
                break;

            case Suit.Diamonds:
                Diamonds.Add(KittyCardImage.First());
                break;
        }
    }

    /// <summary>
    /// Moves the specified card from the available cards collection to the kitty.
    /// </summary>
    /// <param name="cardRef">A reference to the card to be moved to the kitty.</param>
    /// <param name="availableCardsCollectionRef">The collection of available cards from which the card will
    /// be removed. If null, the operation is not performed.</param>
    /// <param name="cardDisplayRef">The display representation of the card to move. If null, the operation 
    /// is not performed.</param>
    private void MoveCardFromAvailableCardToKitty(ICard cardRef, 
        ObservableCollection<CardDisplay>? availableCardsCollectionRef, CardDisplay? cardDisplayRef)
    {
        if (availableCardsCollectionRef != null && cardDisplayRef != null)
        {
            KittyCardImage.Add(cardDisplayRef);
            KittyCard = cardRef;

            // Remove the card from its original collection.

            availableCardsCollectionRef.Remove(cardDisplayRef);
        }
    }

    /// <summary>
    /// Adds a specified card to a specified player's collection of card images and their list of cards.
    /// </summary>
    /// <param name="playerImagesCollection">The player's collection of card images.</param>
    /// <param name="playerCardList">The list of the player's cards.</param>
    /// <param name="draggedCard">The card to add to the player's hand.</param>
    private void AddCardToPlayersHand(ObservableCollection<CardDisplay> playerImagesCollection,
        List<ICard> playerCardList, ICard draggedCard)
    {
        if (playerImagesCollection.Count < Constants.CARDS_PER_PLAYER)
        {
            GetDisplayCardReferences(draggedCard,
                out ObservableCollection<CardDisplay>? availableCardsCollectionRef,
                out CardDisplay? foundCard);

            // Copy the card information to the collections for the referenced player.

            MoveCardFromAvailableCardToPlayer(playerImagesCollection, playerCardList, draggedCard,
                availableCardsCollectionRef, foundCard);
        }
    }

    /// <summary>
    /// Moves a card from the available cards collection to the specified player's collections.
    /// </summary>
    /// <param name="playerImagesCollection">The collection of card display objects representing the player's 
    /// visible cards. The specified card display will be added to this collection.</param>
    /// <param name="playerCardList">The list of card data objects representing the player's hand. The 
    /// specified card will be added to this list.</param>
    /// <param name="cardRef">The card to move from the available cards to the player's hand.</param>
    /// <param name="availableCardsCollectionRef">The collection of available card display objects. If not
    /// null, the specified card display will be removed from this collection.</param>
    /// <param name="cardDisplayRef">The card display object to move. If not null, this object will be added
    /// to the player's collection and removed from the available cards collection.</param>
    private void MoveCardFromAvailableCardToPlayer(
        ObservableCollection<CardDisplay> playerImagesCollection, List<ICard> playerCardList,
        ICard cardRef, ObservableCollection<CardDisplay>? availableCardsCollectionRef,
        CardDisplay? cardDisplayRef)
    {
        if (availableCardsCollectionRef != null && cardDisplayRef != null)
        {
            playerImagesCollection.Add(cardDisplayRef);
            playerCardList.Add(cardRef);

            // Remove the card from its original collection.

            availableCardsCollectionRef.Remove(cardDisplayRef);
        }
    }

    /// <summary>
    /// Determines which available cards collection has the dragged card.
    /// </summary>
    /// <param name="draggedCard">The card dragged by the user.</param>
    /// <param name="availableCardsCollectionRef">Returns a reference to the collection containing the 
    /// card dragged by the user.</param>
    /// <param name="foundCard">Returns the CardDisplay information for dragged card.</param>
    private void GetDisplayCardReferences(ICard draggedCard,
        out ObservableCollection<CardDisplay>? availableCardsCollectionRef,
        out CardDisplay? foundCard)
    {
        // Find which collection has the dragged card.

        availableCardsCollectionRef = null;
        foundCard = GetCardFromAvailableCollection(Clubs, draggedCard);
        if (foundCard != null)
        {
            availableCardsCollectionRef = Clubs;
        }
        else
        {
            foundCard = GetCardFromAvailableCollection(Hearts, draggedCard);
            if (foundCard != null)
            {
                availableCardsCollectionRef = Hearts;
            }
            else
            {
                foundCard = GetCardFromAvailableCollection(Spades, draggedCard);
                if (foundCard != null)
                {
                    availableCardsCollectionRef = Spades;
                }
                else
                {
                    foundCard = GetCardFromAvailableCollection(Diamonds, draggedCard);
                    if (foundCard != null)
                    {
                        availableCardsCollectionRef = Diamonds;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Distributes any remaining cards from each suit to the players.
    /// </summary>
    private void DealRemainingCardsToPlayers()
    {
        if (Clubs.Count > 0)
        {
            DealCardCollectionToPlayers(Clubs);
        }
        if (Hearts.Count > 0)
        {
            DealCardCollectionToPlayers(Hearts);
        }
        if (Spades.Count > 0)
        {
            DealCardCollectionToPlayers(Spades);
        }
        if (Diamonds.Count > 0)
        {
            DealCardCollectionToPlayers(Diamonds);
        }
    }

    /// <summary>
    /// Distributes a collection of available cards among all players in the game.
    /// </summary>
    /// <param name="availableCardsCollectionRef">A reference to the collection of cards available to be 
    /// dealt. The collection is modified as cards are distributed to each player.</param>
    private void DealCardCollectionToPlayers(
        ObservableCollection<CardDisplay> availableCardsCollectionRef)
    {
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player1Cards, Player1CardImages);
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player2Cards, Player2CardImages);
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player3Cards, Player3CardImages);
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player4Cards, Player4CardImages);
    }

    /// <summary>
    /// Transfers cards from the available cards collection to a specific player's hand until the player has 
    /// the maximum allowed number of cards or there are no more available cards.
    /// </summary>
    /// <param name="cardsCollectionRef">The collection of available card displays to be dealt to the player. 
    /// Cards are removed from this collection as they are dealt.</param>
    /// <param name="playerCards">The list representing the player's current hand. Cards are added to this 
    /// list until it reaches the maximum allowed per player.</param>
    /// <param name="playerCardImages">The collection that holds the visual representations of the player's 
    /// cards. Card displays are added here as cards are dealt.</param>
    private void DealCardsToSpecificPlayer(ObservableCollection<CardDisplay> cardsCollectionRef,
        List<ICard> playerCards, ObservableCollection<CardDisplay> playerCardImages)
    {
        while (cardsCollectionRef.Count > 0 &&
               playerCards.Count < Constants.CARDS_PER_PLAYER)
        {
            var cardDisplay = cardsCollectionRef.First();
            MoveCardFromAvailableCardToPlayer(playerCardImages, playerCards, cardDisplay.Card,
                cardsCollectionRef, cardDisplay);
        }
    }

    /// <summary>
    /// Returns true to indicate every players' hand has the required number of cards and a kitty card has
    /// been selected.
    /// </summary>
    /// <returns>True if all cards have been assigned correctly.</returns>
    private bool VerifyCardSelection()
    {
        bool allCardsAssigned = Player1Cards.Count == Constants.CARDS_PER_PLAYER &&
                                Player2Cards.Count == Constants.CARDS_PER_PLAYER &&
                                Player3Cards.Count == Constants.CARDS_PER_PLAYER &&
                                Player4Cards.Count == Constants.CARDS_PER_PLAYER;

        if (!allCardsAssigned)
        {
            DialogBoxes.ErrorDialog("One or more player's hands do not contain the correct number " +
                "of cards.", this);
        }
        else if (!KittyCardImage.Any())
        {
            allCardsAssigned = false;
            DialogBoxes.ErrorDialog("A kitty card has not been selected.", this);
        }

        return allCardsAssigned;
    }

    /// <summary>
    /// Populates the RemainingCards collection with any cards that were not assigned to a player or as the
    /// kitty card.
    /// </summary>
    private void SetRemainingCards()
    {
        if (Hearts.Any())
        {
            RemainingCards.AddRange(Hearts.Select(x => x.Card));
        }
        if (Spades.Any())
        {
            RemainingCards.AddRange(Spades.Select(x => x.Card));
        }
        if (Clubs.Any())
        {
            RemainingCards.AddRange(Clubs.Select(x => x.Card));
        }
        if (Diamonds.Any())
        {
            RemainingCards.AddRange(Diamonds.Select(x => x.Card));
        }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (VerifyCardSelection())
        {
            SetRemainingCards();
            DialogResult = true;
            Close();
        }
    }

    private void Image_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Image clickedImage)
        {
            var data = new DataObject();
            data.SetData(CardClipBoardDataFormat, clickedImage.Tag);
            DragDrop.DoDragDrop(clickedImage, data, DragDropEffects.Move);
        }
    }

    private void Player1ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(CardClipBoardDataFormat) is ICard draggedCard)
        {
            AddCardToPlayersHand(Player1CardImages, Player1Cards, draggedCard);
        }
    }

    private void Player2ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(CardClipBoardDataFormat) is ICard draggedCard)
        {
            AddCardToPlayersHand(Player2CardImages, Player2Cards, draggedCard);
        }
    }

    private void Player3ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(CardClipBoardDataFormat) is ICard draggedCard)
        {
            AddCardToPlayersHand(Player3CardImages, Player3Cards, draggedCard);
        }
    }

    private void Player4ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(CardClipBoardDataFormat) is ICard draggedCard)
        {
            AddCardToPlayersHand(Player4CardImages, Player4Cards, draggedCard);
        }
    }

    private void RandomlyDealButton_Click(object sender, RoutedEventArgs e)
    {
        DealRemainingCardsToPlayers();
    }

    private void KittyItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(CardClipBoardDataFormat) is ICard draggedCard)
        {
            AddCardToKitty(draggedCard);
        }
    }
}