using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for PickTrumpSuitWindow.xaml
/// </summary>
public partial class PickTrumpSuitWindow : Window
{
    public PickTrumpSuitWindow(List<Suit> allowedSuits, IPlayer currentPlayer, IPlayer dealer, 
        bool isKittyRound)
    {
        InitializeComponent();
        ConfigWindow(allowedSuits, currentPlayer, dealer, isKittyRound);
        Owner = Application.Current.MainWindow;
        DataContext = this;
    }

    /// <summary>
    /// Reference to the cards in the player's hand.
    /// </summary>
    private List<Card> _playerHand= [];

    /// <summary>
    /// True to indicate that the user cannot pass when they are the dealer and the second bidding round has
    /// not produced a trump suit.
    /// </summary>
    private bool _stickTheDealer;

    #region Dependency Properties

    /// <summary>
    /// Using a DependencyProperty as the backing store for IsSubmitButtonEnabled.
    /// </summary>
    public static readonly DependencyProperty IsSubmitButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsSubmitButtonEnabled), typeof(bool), typeof(PickTrumpSuitWindow),
            new PropertyMetadata(true));

    /// <summary>
    /// Using a DependencyProperty as the backing store for HeartsVisibility.
    /// </summary>
    public static readonly DependencyProperty HeartsVisibilityProperty =
        DependencyProperty.Register(nameof(HeartsVisibility), typeof(Visibility),
            typeof(PickTrumpSuitWindow), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for DiamondsVisibility.
    /// </summary>
    public static readonly DependencyProperty DiamondsVisibilityProperty =
        DependencyProperty.Register(nameof(DiamondsVisibility), typeof(Visibility),
            typeof(PickTrumpSuitWindow), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for ClubsVisibility.
    /// </summary>
    public static readonly DependencyProperty ClubsVisibilityProperty =
        DependencyProperty.Register(nameof(ClubsVisibility), typeof(Visibility),
            typeof(PickTrumpSuitWindow), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for SpadesVisibility.
    /// </summary>
    public static readonly DependencyProperty SpadesVisibilityProperty =
        DependencyProperty.Register(nameof(SpadesVisibility), typeof(Visibility),
            typeof(PickTrumpSuitWindow), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for SelectedSuit.
    /// </summary>
    public static readonly DependencyProperty SelectedSuitProperty =
        DependencyProperty.Register(nameof(SelectedSuit), typeof(Suit?), typeof(PickTrumpSuitWindow),
            new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for GoAlone.
    /// </summary>
    public static readonly DependencyProperty GoAloneProperty =
        DependencyProperty.Register(nameof(GoAlone), typeof(bool), typeof(PickTrumpSuitWindow),
            new PropertyMetadata(false));

    /// <summary>
    /// Using a DependencyProperty as the backing store for IsGoAloneCheckBoxEnabled.
    /// </summary>
    public static readonly DependencyProperty IsGoAloneCheckBoxEnabledProperty =
        DependencyProperty.Register(nameof(IsGoAloneCheckBoxEnabled), typeof(bool),
            typeof(PickTrumpSuitWindow), new PropertyMetadata(true));

    /// <summary>
    /// Using a DependencyProperty as the backing store for IsPassButtonEnabled.
    /// </summary>
    public static readonly DependencyProperty IsPassButtonEnabledProperty =
        DependencyProperty.Register(nameof(IsPassButtonEnabled), typeof(bool), typeof(PickTrumpSuitWindow),
            new PropertyMetadata(true));

    /// <summary>
    /// Using a DependencyProperty as the backing store for SubmitButtonText.
    /// </summary>
    public static readonly DependencyProperty SubmitButtonTextProperty =
        DependencyProperty.Register(nameof(SubmitButtonText), typeof(string), typeof(PickTrumpSuitWindow),
            new PropertyMetadata(string.Empty));
        
    #endregion Dependency Properties


    /// <summary>
    /// Shows/hides the button to select hearts as trump.
    /// </summary>
    public Visibility HeartsVisibility
    {
        get => (Visibility)GetValue(HeartsVisibilityProperty);
        set => SetValue(HeartsVisibilityProperty, value);
    }

    /// <summary>
    /// Shows/hides the button to select diamonds as trump.
    /// </summary>
    public Visibility DiamondsVisibility
    {
        get => (Visibility)GetValue(DiamondsVisibilityProperty);
        set => SetValue(DiamondsVisibilityProperty, value);
    }

    /// <summary>
    /// Shows/hides the button to select clubs as trump.
    /// </summary>
    public Visibility ClubsVisibility
    {
        get => (Visibility)GetValue(ClubsVisibilityProperty);
        set => SetValue(ClubsVisibilityProperty, value);
    }

    /// <summary>
    /// Shows/hides the button to select spades as trump.
    /// </summary>
    public Visibility SpadesVisibility
    {
        get => (Visibility)GetValue(SpadesVisibilityProperty);
        set => SetValue(SpadesVisibilityProperty, value);
    }

    /// <summary>
    /// The suit selected by the user to be trump.  Null indicates the user wants to pass.
    /// </summary>
    public Suit? SelectedSuit
    {
        get => (Suit?)GetValue(SelectedSuitProperty); 
        set => SetValue(SelectedSuitProperty, value); 
    }

    /// <summary>
    /// True to indicate the user wants to go alone on the specified trump.
    /// </summary>
    public bool GoAlone
    {
        get => (bool)GetValue(GoAloneProperty); 
        set => SetValue(GoAloneProperty, value); 
    }

    /// <summary>
    /// True to indicate the Go Alone checkbox should be enabled.
    /// </summary>
    public bool IsGoAloneCheckBoxEnabled
    {
        get => (bool)GetValue(IsGoAloneCheckBoxEnabledProperty); 
        set => SetValue(IsGoAloneCheckBoxEnabledProperty, value);
    }

    /// <summary>
    /// True to indicate the Pass button can be enabled.
    /// </summary>
    public bool IsPassButtonEnabled
    {
        get => (bool)GetValue(IsPassButtonEnabledProperty); 
        set => SetValue(IsPassButtonEnabledProperty, value);
    }

    /// <summary>
    /// True to indicate the Submit button can be enabled.
    /// </summary>
    public bool IsSubmitButtonEnabled
    {
        get => (bool)GetValue(IsSubmitButtonEnabledProperty);
        set => SetValue(IsSubmitButtonEnabledProperty, value);
    }

    /// <summary>
    /// The caption to appear on the Submit button.
    /// </summary>
    public string SubmitButtonText
    {
        get => (string)GetValue(SubmitButtonTextProperty);
        set => SetValue(SubmitButtonTextProperty, value);
    }


    /// <summary>
    /// Sets up the properties and some of the controls on the form.
    /// </summary>
    /// <param name="allowedSuits">A list of the suits that the user is allowed to pick from.</param>
    /// <param name="currentPlayer">A reference to the user.</param>
    /// <param name="dealer">A reference to the player that is the dealer.</param>
    /// <param name="isKittyRound">True to indicate the current bidding round is the first one.</param>
    private void ConfigWindow(List<Suit> allowedSuits, IPlayer currentPlayer, IPlayer dealer, 
        bool isKittyRound)
    {
        _playerHand = currentPlayer.Hand;
        SetTrumpButtonsVisibility(allowedSuits);
        if (isKittyRound)
        {
            Title = "Choose Kitty Suit";
            CheckForCanadianLoner(currentPlayer, dealer);
        }
        else
        {
            CheckForStickTheDealer(currentPlayer, dealer);
        }
        SetSubmitButtonsCaption(isKittyRound, currentPlayer, dealer);
    }

    /// <summary>
    /// Sets the caption on the submit button based on whether it is the kitty round or not.
    /// </summary>
    /// <param name="isKittyRound">True to indicate it is the kitty round.</param>
    /// <param name="currentPlayer">A reference to the current player.</param>
    /// <param name="dealer">A reference to the dealer.</param>
    private void SetSubmitButtonsCaption(bool isKittyRound, IPlayer currentPlayer, IPlayer dealer)
    {
        if (isKittyRound)
        {
            SubmitButtonText = currentPlayer == dealer ? "Pick It Up" : "Order Up";
        }
        else
        {
            SubmitButtonText = "Call Trump";
        }
    }

    /// <summary>
    /// Sets the visibility of each suit if it exists in a list of suits.
    /// </summary>
    /// <param name="allowedSuits">The suits to set as visible.</param>
    private void SetTrumpButtonsVisibility(List<Suit> allowedSuits)
    {
        // Filter the suits if the user must have a natural in their hand.

        if (GameSettingsManager.Instance.MustHaveSuitToCall)
        {
            allowedSuits = CheckForSuitExistence(allowedSuits);
        }

        // Set the Submit and Go Alone buttons enabled when there are some suits to select for trump.

        IsSubmitButtonEnabled = allowedSuits.Count != 0;
        IsGoAloneCheckBoxEnabled = allowedSuits.Count != 0;
        bool selectTrumpButton = allowedSuits.Count == 1;
        
        // Set the available suits' buttons visible.

        foreach (Suit suit in allowedSuits)
        {
            switch (suit)
            {
                case Suit.Hearts:
                    HeartsVisibility = Visibility.Visible; 
                    HeartsButton.IsChecked = selectTrumpButton;
                    break;

                case Suit.Diamonds:
                    DiamondsVisibility = Visibility.Visible;
                    DiamondsButton.IsChecked = selectTrumpButton;
                    break;

                case Suit.Clubs: 
                    ClubsVisibility = Visibility.Visible;
                    ClubsButton.IsChecked = selectTrumpButton;
                    break;

                case Suit.Spades:
                    SpadesVisibility = Visibility.Visible;
                    HeartsButton.IsChecked = selectTrumpButton;
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the properties on the form when the Canadian Loner rule is set.
    /// </summary>
    /// <param name="currentPlayer">The player that is prompted to order the kitty card up.</param>
    /// <param name="dealer">The player that is the dealer for the current round.</param>
    private void CheckForCanadianLoner(IPlayer currentPlayer, IPlayer dealer)
    {
        if (GameSettingsManager.Instance.CanadianLonerRule)
        {
            if (currentPlayer.TeamIndex == dealer.TeamIndex && currentPlayer != dealer)
            {
                GoAlone = true;
                IsGoAloneCheckBoxEnabled = false;
            }
        }
    }

    /// <summary>
    /// Checks the player's hand for cards that match the suit in a list of suits.
    /// </summary>
    /// <param name="allowedSuits">A list of all possible suits that could be called trump.</param>
    /// <returns>A list of suits derived from the allowed suits indicating which suits that the player has
    /// cards for.</returns>
    private List<Suit> CheckForSuitExistence(List<Suit> allowedSuits)
    {
        List<Suit> newAllowed = [];

        foreach (Suit suit in allowedSuits)
        {
            if (_playerHand.Where(c => c.Suit == suit).Any())
            {
                newAllowed.Add(suit);
            }
        }

        return newAllowed;
    }

    /// <summary>
    /// Checks the conditions for sticking the dealer.  Assumes that it is not being called when it is the
    /// kitty round.
    /// </summary>
    /// <param name="player">A reference to the user.</param>
    /// <param name="dealer">A reference to the player that is the dealer.</param>
    private void CheckForStickTheDealer(IPlayer player, IPlayer dealer)
    {
        _stickTheDealer = GameSettingsManager.Instance.StickTheDealer && player == dealer;
        IsPassButtonEnabled = !_stickTheDealer;
    }

    /// <summary>
    /// Sets the value for the ChosenSuit property based on which toggle button is checked.
    /// </summary>
    /// <param name="toggleButton">The toggle button that was checked by the user.</param>
    private void SetUpChosenSuit(ToggleButton toggleButton)
    {
        if (toggleButton.Tag is string chosenSuit)
        {
            SelectedSuit = Enum.Parse<Suit>(chosenSuit);
        }
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        // Uncheck the toggle buttons on the control except for the one just checked by the user.

        if (sender is ToggleButton currentButton)
        {
            this.UncheckOtherToggleButtons(currentButton);

            // Set up the selected suit property.

            SetUpChosenSuit(currentButton);
        }
    }

    private void PassButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedSuit != null)
        {
            DialogResult = true;
            Close();
        }
        else
        {
            DialogBoxes.ErrorDialog("You must select a suit.", this);
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_stickTheDealer)
        {
            if (SelectedSuit == null)
            {
                DialogBoxes.InformationDialog(
                    "This dialog box cannot close until a trump suit has been chosen.", 
                    this, 
                    "Stick the Dealer");
                e.Cancel = true;
            }
        }
    }
}
