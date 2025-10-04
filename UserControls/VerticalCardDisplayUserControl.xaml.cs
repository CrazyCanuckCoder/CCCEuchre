using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for VerticalCardDisplayUserControl.xaml
/// </summary>
public partial class VerticalCardDisplayUserControl : UserControl
{
    public VerticalCardDisplayUserControl()
    {
        InitializeComponent();
        VerticalCardImages = [];
        DataContext = this;
    }

    /// <summary>
    /// Tracks the number of cards when displaying only the card backs.
    /// </summary>
    private int _numberOfCardBacks = 0;

    /// <summary>
    /// The collection of images to display for the cards.
    /// </summary>
    public ObservableCollection<CardDisplay> VerticalCardImages { get; set; }

    /// <summary>
    /// A list of the cards to display on the control.
    /// </summary>
    public List<Card> Cards { get; private set; } = new();

    /// <summary>
    /// Used for compatibility with the horizontal card display control.
    /// </summary>
    /// <param name="cards">The list of IPlayerCards to display on the control.</param>
    /// <param name="trumpSuit">The current suit that is trump. Not used.</param>
    public void SetupCards(List<Card> cards, Suit trumpSuit)
    {
        SetupCards(cards);
    }

    /// <summary>
    /// Displays a provided list of cards on the control.
    /// </summary>
    /// <param name="cards">The list of IPlayerCards to display on the control.</param>
    public void SetupCards(List<Card> cards)
    {
        ClearCards();

        // Get the cards that have not been played.  Also, don't include the card that is used to call the
        //  unknown Ace.

        Cards = (  from currentCard in cards
                  where !currentCard.IsPlayed
                 select currentCard)
                .ToList();

        if (Cards.Count > 0)
        {
            // Load each card in order.

            for (int cardIndex = 0; cardIndex < Cards.Count; cardIndex++)
            {
                // Load the rotated image for the card.

                string cardFilename = Cards[cardIndex].ToString()?.ToLower() + " rotated.png";
                BitmapImage source = new();
                source.BeginInit();
                source.UriSource =
                    new Uri($"pack://application:,,,/Euchre;component/images/cards/{cardFilename}");
                source.EndInit();

                // Add the image to the collection of images.

                VerticalCardImages.Add(new CardDisplay()
                {
                    ImageData = new Image()
                    {
                        Source = source
                    },
                    Card = Cards[cardIndex],
                    CardVisibility = Visibility.Collapsed,
                    ImageVisibility = Visibility.Visible,
                    CardMargin = cardIndex > 0 ? new(0, -70, 0, 0) : new(0),
                });
            }
        }
    }

    /// <summary>
    /// Removes all the cards from the control.
    /// </summary>
    public void ClearCards()
    {
        Cards.Clear();
        VerticalCardImages.Clear();
        _numberOfCardBacks = 0;
    }

    /// <summary>
    /// Removes a card from the list of images of cards.
    /// </summary>
    public void RemoveCard()
    {
        VerticalCardImages.Remove(VerticalCardImages.Last());
        _numberOfCardBacks--;
    }

    /// <summary>
    /// Adds a number of card backs to display to the control and to the internal list of the number of cards
    /// that are displayed on the control.
    /// </summary>
    /// <param name="numberOfCards">The number of card back images to add to the control.</param>
    public void AddCards(int numberOfCards)
    {
        DisplayCards(numberOfCards);
        _numberOfCardBacks += numberOfCards;
    }

    /// <summary>
    /// Shows a number of cards on the control but does not add them to the internal list of number of cards
    /// that are displayed on the control.
    /// </summary>
    /// <param name="numberOfCards">The number of card back images to show on the control.</param>
    public void DisplayCards(int numberOfCards)
    {
        string cardFilename = GameSettingsManager.Instance.SelectedCardBack;
        cardFilename = cardFilename.Insert(cardFilename.LastIndexOf('.'), " rotated");
        Uri uriSource = new($"pack://application:,,,/Euchre;component/images/cardbacks/{cardFilename}");

        // Load each card back for the specified number cards.

        for (int cardIndex = 0; cardIndex < numberOfCards; cardIndex++)
        {
            // Load the image for the card.

            BitmapImage source = new();
            source.BeginInit();
            source.UriSource = uriSource;
            source.EndInit();

            // Add the image to the collection of images and set it visible.

            VerticalCardImages.Add(new CardDisplay()
            {
                ImageData = new Image()
                {
                    Source = source
                },
                CardVisibility = Visibility.Collapsed,
                ImageVisibility = Visibility.Visible,
                CardMargin = _numberOfCardBacks + cardIndex > 0 ? new(0, -70, 0, 0) : new(0)
            });
        }
    }
}
