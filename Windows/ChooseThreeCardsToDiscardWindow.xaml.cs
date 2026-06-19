using Euchre.Logic.Components;
using Euchre.Logic.EventArgs;
using Euchre.UILogic.Classes;
using System.Windows;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for ChooseThreeCardsToDiscardWindow.xaml
/// </summary>
public partial class ChooseThreeCardsToDiscardWindow : Window
{
    public ChooseThreeCardsToDiscardWindow(List<Card> cards)
    {
        InitializeComponent();

        // Initialize the lists of cards.

        KeepCards = [.. cards];
        DiscardCards = [];

        // Populate UI controls with the cards.

        KeepCardDisplay.SetupCards(KeepCards, null, false, true);
        DiscardCardDisplay.SetupCards(DiscardCards, null, false, true);

        // Wire up events for when the user chooses a card to move between the keep and discard piles.

        KeepCardDisplay.UserChoseCard += KeepCardDisplay_UserChoseCard;
        DiscardCardDisplay.UserChoseCard += DiscardCardDisplay_UserChoseCard;

        Owner = Application.Current.MainWindow;
        DataContext = this;
    }

    /// <summary>
    /// The list of cards remaining to keep.
    /// </summary>
    private List<Card> KeepCards { get; set; }

    /// <summary>
    /// The list of cards chosen for discard.
    /// </summary>
    private List<Card> DiscardCards { get; set; }

    /// <summary>
    /// Public property exposing the discarded cards chosen by the user.
    /// </summary>
    public List<Card>? DiscardedCards { get; private set; }

    /// <summary>
    /// Refreshes the card displays to reflect the current state of the keep and discard piles.
    /// </summary>
    private void RefreshDisplays()
    {
        // Re-render both displays; allow selection on both.

        KeepCardDisplay.SetupCards(KeepCards, null, false, true);
        DiscardCardDisplay.SetupCards(DiscardCards, null, false, true);
    }

    private void KeepCardDisplay_UserChoseCard(object? sender, UserChoseCardEventArgs e)
    {
        if (e.ChosenCard is Card chosenCard)
        {
            if (DiscardCards.Count >= 3)
            {
                DialogBoxes.ErrorDialog("You may only discard three cards.", this);
                return;
            }
            else if (chosenCard.Rank is not (Rank.Ten or Rank.Nine))
            {
                DialogBoxes.ErrorDialog("You may only discard 9s and 10s.", this);
                return;
            }

            // Move the chosen card from the keep pile to the discard pile.

            if (KeepCards.Remove(chosenCard))
            {
                DiscardCards.Add(chosenCard);
                RefreshDisplays();
            }
        }
    }

    private void DiscardCardDisplay_UserChoseCard(object? sender, UserChoseCardEventArgs e)
    {
        if (e.ChosenCard is Card chosenCard)
        {
            // Move the chosen card from the discard pile back to the keep pile.

            if (DiscardCards.Remove(chosenCard))
            {
                KeepCards.Add(chosenCard);
                RefreshDisplays();
            }
        }
    }

    private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (DiscardCards.Count != 3)
        {
            DialogBoxes.ErrorDialog("You must choose exactly 3 cards to discard.", this);
            return;
        }

        DiscardedCards = [.. DiscardCards];
        DialogResult = true;
        Close();
    }
}
