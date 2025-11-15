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

    private readonly Thickness _availableCardMargin = new(5, 0, 0, 0);
    private readonly Thickness _playerCardMargin = new(5, 8, 0, 0);

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

    public ObservableCollection<CardDisplay> KittyCardImage { get; private set; } = [];

    public ICard KittyCard { get; private set; }

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
                CardMargin = _availableCardMargin,
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

    private void AddCardToKitty(ICard draggedCard)
    {
        // TODO: Ensure only one card can be added to the kitty.

        GetDisplayCardReferences(draggedCard,
            out ObservableCollection<CardDisplay>? availableCardsCollectionRef,
            out CardDisplay? foundCard);

        MoveCardFromAvailableCardToKitty(draggedCard, availableCardsCollectionRef, foundCard);
    }

    private void MoveCardFromAvailableCardToKitty(ICard cardRef, 
        ObservableCollection<CardDisplay>? availableCardsCollectionRef, CardDisplay? cardDisplayRef)
    {
        if (availableCardsCollectionRef != null && cardDisplayRef != null)
        {
            cardDisplayRef.CardMargin = _playerCardMargin;
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

    private void MoveCardFromAvailableCardToPlayer(
        ObservableCollection<CardDisplay> playerImagesCollection, List<ICard> playerCardList,
        ICard cardRef, ObservableCollection<CardDisplay>? availableCardsCollectionRef,
        CardDisplay? cardDisplayRef)
    {
        if (availableCardsCollectionRef != null && cardDisplayRef != null)
        {
            cardDisplayRef.CardMargin = _playerCardMargin;
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

    private void DealCardCollectionToPlayers(
        ObservableCollection<CardDisplay> availableCardsCollectionRef)
    {
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player1Cards, Player1CardImages);
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player2Cards, Player2CardImages);
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player3Cards, Player3CardImages);
        DealCardsToSpecificPlayer(availableCardsCollectionRef, Player4Cards, Player4CardImages);
    }

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

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (VerifyCardSelection())
        {
            DialogResult = true;
            Close();
        }
    }

    private void Image_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Image clickedImage)
        {
            var data = new DataObject();
            data.SetData("Card", clickedImage.Tag);
            DragDrop.DoDragDrop(clickedImage, data, DragDropEffects.Move);
        }
    }

    private void Player1ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData("Card") is ICard draggedCard)
        {
            AddCardToPlayersHand(Player1CardImages, Player1Cards, draggedCard);
        }
    }

    private void Player2ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData("Card") is ICard draggedCard)
        {
            AddCardToPlayersHand(Player2CardImages, Player2Cards, draggedCard);
        }
    }

    private void Player3ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData("Card") is ICard draggedCard)
        {
            AddCardToPlayersHand(Player3CardImages, Player3Cards, draggedCard);
        }
    }

    private void Player4ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData("Card") is ICard draggedCard)
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
        if (e.Data.GetData("Card") is ICard draggedCard)
        {
            AddCardToKitty(draggedCard);
        }
    }
}

