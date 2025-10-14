using Euchre.Logic.Components;
using Euchre.Logic.EventArgs;
using Euchre.Logic.Helpers;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using Euchre.UILogic.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Euchre.UserControls;
/// <summary>
/// Interaction logic for CardDisplayUserControl.xaml
/// </summary>
public partial class CardDisplayUserControl : UserControl, IBaseCardDisplay
{
    public CardDisplayUserControl()
    {
        InitializeComponent();
        CardImages = [];
        DataContext = this;
    }

    /// <summary>
    /// Tracks the number of cards when displaying only the card backs.
    /// </summary>
    private int _numberOfCardBacks = 0;

    /// <summary>
    /// The collection of images to display for the cards.
    /// </summary>
    public ObservableCollection<CardDisplay> CardImages { get; set; }

    /// <summary>
    /// Gets the card chosen by the user.
    /// </summary>
    public Card? ChosenCard { get; private set; }

    /// <summary>
    /// A list of the cards to display on the control.
    /// </summary>
    public List<Card> Cards { get; private set; } = [];

    /// <summary>
    /// Fired when the user chooses a card.
    /// </summary>
    public event EventHandler<UserChoseCardEventArgs>? UserChoseCard;


    /// <summary>
    /// Displays a provided list of cards on the control.
    /// </summary>
    /// <param name="cards">The list of Cards to display on the control.</param>
    public void SetupCards(List<Card> cards)
    {
        ClearCards();

        // Get the cards that have not been played.

        Cards = (  from currentCard in cards
                 select currentCard)
                .ToList();

        // Create a different Margin value for a card that separates the group.

        Thickness inGroupMargin = new(-25, 0, 0, 0);

        if (Cards.Count > 0)
        {
            // Group the cards by suit. Setup up a previous suit to see if the current card belongs in the
            //  current group of cards. 

            Suit previousSuit = Cards.First().Suit;

            // Load each card in order.

            for (int cardIndex = 0; cardIndex < Cards.Count; cardIndex++)
            {
                string cardFilename = Cards[cardIndex].ToString()?.ToLower() + ".png";
                BitmapImage source = UIHelpers.GenerateImageSource(
                    $"pack://application:,,,/Euchre;component/images/cards/{cardFilename}");

                // Determine the margin to use for the current card.

                Thickness cardMargin = new(0);
                if (cardIndex > 0)
                {
                    if (Cards[cardIndex].Suit == previousSuit)
                    {
                        cardMargin = inGroupMargin;
                    }
                }

                // Add the image to the collection of images and set its visibility.

                CardImages.Add(new CardDisplay()
                {
                    ImageData = new Image()
                    {
                        Source = source
                    },
                    CardVisibility = Visibility.Collapsed,
                    ImageVisibility = Visibility.Visible,
                    Card = Cards[cardIndex],
                    CardMargin = cardMargin,
                    IsEnabled = true
                });

                // Reset the suit and trump tracker variables.

                previousSuit = Cards[cardIndex].Suit;
            }
        }
    }

    /// <summary>
    /// Displays a provided list of cards on the control.
    /// </summary>
    /// <param name="cards">The list of Cards to display on the control.</param>
    /// <param name="trumpSuit">The current suit for trump.</param>
    public void SetupCards(List<Card> cards, Suit trumpSuit)
    {
        SetupCards(cards, null, false, trumpSuit);
    }

    /// <summary>
    /// Displays a provided list of cards on the control.
    /// </summary>
    /// <param name="cards">The list of Cards to display on the control.</param>
    /// <param name="disabledSuits">A list of the suits that should be disabled on the control.</param>
    /// <param name="allowSelection">True to indicate the user can select a card; false by default.</param>
    public void SetupCards(List<Card> cards, Suit? trickSuit, bool trickSuitIsTrump,
        bool allowSelection = false)
    {
        SetupCards(cards, trickSuit, trickSuitIsTrump, Suit.Clubs, allowSelection);
    }

    /// <summary>
    /// Displays a provided list of cards on the control.
    /// </summary>
    /// <param name="cards">The list of Cards to display on the control.</param>
    /// <param name="disabledSuits">A list of the suits that should be disabled on the control.</param>
    /// <param name="allowSelection">True to indicate the user can select a card; false by default.</param>
    public void SetupCards(List<Card> cards, Suit? trickSuit, bool trickSuitIsTrump,
        Suit trumpSuit, bool allowSelection = false)
    {
        ClearCards();

        // Get the cards that have not been played.

        Cards = (  from currentCard in cards
                 select currentCard)
                .ToList();

        // Determine if any of the suits in the hand need to be disabled.

        List<Suit> disabledSuits = [];
        if (allowSelection && !trickSuitIsTrump && trickSuit != null &&
            Cards.HasAnyOfSuit(trickSuit.Value, trumpSuit))
        {
            disabledSuits = (  from Suit suitType in Enum.GetValues(typeof(Suit))
                              where suitType != trickSuit
                             select suitType)
                            .ToList();
        }

        // Create a different Margin value for a card that separates the group.

        Thickness inGroupMargin = new(-25, 0, 0, 0);

        if (Cards.Count > 0)
        {

            // Group the cards by suit. Setup up a previous suit to see if the current card belongs in the
            //  current group of cards. 

            Suit previousSuit = Cards.First().Suit;
            bool isTrump = Cards.First().IsTrump(trumpSuit);

            // Load each card in order.

            for (int cardIndex = 0; cardIndex < Cards.Count; cardIndex++)
            {
                // Load the image for the card, checking for whether or not it should be disabled.

                bool disableCard;
                if (trickSuitIsTrump)
                {
                    disableCard = Cards.HasAnyTrump(trumpSuit) && !Cards[cardIndex].IsTrump(trumpSuit);
                }
                else if (Cards[cardIndex].IsTrump(trumpSuit))
                {
                    disableCard = disabledSuits.Count != 0;
                }
                else
                {
                    disableCard = disabledSuits.Contains(Cards[cardIndex].EffectiveSuit(trumpSuit));
                }
                string cardFilename = Cards[cardIndex].ToString()?.ToLower() +
                    (disableCard ? " disabled" : "") + ".png";
                BitmapImage source = UIHelpers.GenerateImageSource(
                    $"pack://application:,,,/Euchre;component/images/cards/{cardFilename}");

                // Determine the margin to use for the current card.

                Thickness cardMargin = new(0);
                if (cardIndex > 0)
                {
                    if (isTrump && Cards[cardIndex].IsTrump(trumpSuit))
                    {
                        cardMargin = inGroupMargin;
                    }
                    else if (Cards[cardIndex].EffectiveSuit(trumpSuit) == previousSuit)
                    {
                        cardMargin = inGroupMargin;
                    }
                }

                // Add the image to the collection of images and set its visibility.

                CardImages.Add(new CardDisplay()
                {
                    ImageData = new Image()
                    {
                        Source = source
                    },
                    CardVisibility = allowSelection ?
                        (disableCard ? Visibility.Collapsed : Visibility.Visible) : Visibility.Collapsed,
                    ImageVisibility = allowSelection ?
                        (disableCard ? Visibility.Visible : Visibility.Collapsed) : Visibility.Visible,
                    Card = Cards[cardIndex],
                    CardMargin = cardMargin,
                    IsEnabled = !disableCard
                });

                // Reset the suit and trump tracker variables.

                isTrump = Cards[cardIndex].IsTrump(trumpSuit);
                previousSuit = isTrump ? trumpSuit : Cards[cardIndex].Suit;
            }
        }
    }

    /// <summary>
    /// Removes all the cards from the control.
    /// </summary>
    public void ClearCards()
    {
        Cards.Clear();
        CardImages.Clear();
        _numberOfCardBacks = 0;
    }

    /// <summary>
    /// Waits for the user to select a card.
    /// </summary>
    /// <param name="disabledSuits">A list of the suits that the user cannot select a card from.</param>
    public void GetCardFromUser(List<Card> cards, Suit? trickSuit, bool trickSuitIsTrump,
        Suit trumpSuit)
    {
        // Send a copy of the current cards to the method override so the cards aren't erased.

        SetupCards(cards.ToArray().ToList(), trickSuit, trickSuitIsTrump, trumpSuit, true);
        ChosenCard = null;

        if (GameSettingsManager.Instance.PlayLastCardInHand && cards.Count == 1)
        {
            ChosenCard = cards.First();
        }
        else
        {
            // TODO: Add a time limit to this loop.  Create a mechanism to prompt the user to continue.

            while (ChosenCard == null)
            {
                UIHelpers.AllowUIToUpdate();
            }
        }
    }

    /// <summary>
    /// Sets up the cards to allow the user to pick one card to discard.
    /// </summary>
    /// <param name="cards">The list of cards to pick from.</param>
    public void GetDiscard(List<Card> cards)
    {
        // Get the cards that have not been played.

        Cards = (  from currentCard in cards
                 select currentCard)
                .ToList();

        if (Cards.Count > 0)
        {
            // Group the cards by suit. Setup up a previous suit to see if the current card belongs in the
            //  current group of cards. Create two different Margin values for a card in the group and the
            //  card that separates the group.

            Suit previousSuit = Cards.First().Suit;
            Thickness inGroupMargin = new(-25, 0, 0, 0);

            // Load each card in order.

            for (int cardIndex = 0; cardIndex < Cards.Count; cardIndex++)
            {
                // Load the image for the card.

                string cardFilename = Cards[cardIndex].ToString()?.ToLower() + ".png";
                string imageName = $"pack://application:,,,/Euchre;component/images/cards/{cardFilename}";

                // Determine the margin to use for the current card.

                Thickness cardMargin = new(0);
                if (cardIndex > 0)
                {
                    if (Cards[cardIndex].Suit == previousSuit)
                    {
                        cardMargin = inGroupMargin;
                    }
                }

                // Add the image to the collection of images and set its visibility.

                CardImages.Add(new CardDisplay()
                {
                    ImageData = new Image()
                    {
                        Source = UIHelpers.GenerateImageSource(imageName)
                    },
                    CardVisibility = Visibility.Visible,
                    ImageVisibility = Visibility.Collapsed,
                    Card = Cards[cardIndex],
                    CardMargin = cardMargin,
                    IsEnabled = true,
                });

                // Reset the suit tracker variable.

                previousSuit = Cards[cardIndex].Suit;
            }
        }
    }

    /// <summary>
    /// Removes a card from the list of images of cards.
    /// </summary>
    public void RemoveCard()
    {
        CardImages.Remove(CardImages.First());
        _numberOfCardBacks--;
    }

    /// <summary>
    /// Adds a number of card backs to display to the control and to the internal list of the number of cards
    /// that are displayed on the control.
    /// </summary>
    /// <param name="numberOfCards">The number of card back images to add to the control.</param>
    public void AddCards(int numberOfCards)
    {
        _numberOfCardBacks += numberOfCards;
        DisplayCards(numberOfCards);
    }

    /// <summary>
    /// Shows a number of cards on the control but does not add them to the internal list of number of cards
    /// that are displayed on the control.
    /// </summary>
    /// <param name="numberOfCards">The number of card back images to show on the control.</param>
    public void DisplayCards(int numberOfCards)
    {
        // Load each card back for the specified number of cards.

        for (int cardIndex = 0; cardIndex < numberOfCards; cardIndex++)
        {
            // Add the image to the collection of images and set it visible.

            CardImages.Add(new CardDisplay()
            {
                ImageData = new Image()
                {
                    Source = UIHelpers.GenerateImageSource(
                        "pack://application:,,,/Euchre;component/images/cardbacks/" + 
                        GameSettingsManager.Instance.SelectedCardBack)
                },
                CardVisibility = Visibility.Collapsed,
                ImageVisibility = Visibility.Visible,
                CardMargin = _numberOfCardBacks + cardIndex > 0 ? new(-25, 0, 0, 0) : new(0)
            });
        }
    }

    /// <summary>
    /// Sets the value for the ChosenCard property based on which toggle button is checked.
    /// </summary>
    /// <param name="toggleButton">The toggle button that was checked by the user.</param>
    private void SetUpChosenCard(ToggleButton toggleButton)
    {
        if (toggleButton.Tag is Card chosenCard)
        {
            ChosenCard = chosenCard;
            OnCardChosen(chosenCard);
        }
    }

    /// <summary>
    /// Fires the UserChoseCard event if there are any listeners.
    /// </summary>
    /// <param name="chosenCard">The card that the user chose.</param>
    private void OnCardChosen(Card chosenCard)
    {
        UserChoseCard?.Invoke(this, new UserChoseCardEventArgs(chosenCard));
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        // Uncheck the toggle buttons on the control except for the one just checked by the user.

        if (sender is ToggleButton currentButton)
        {
            this.UncheckOtherToggleButtons(currentButton);

            // Set up the selected card property.

            SetUpChosenCard(currentButton);
        }
    }
}
