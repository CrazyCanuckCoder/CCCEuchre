using Euchre.Logic.Enums;
using Euchre.Logic.Interfaces;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Euchre.UserControls;
/// <summary>
/// Interaction logic for VerticalPlayerDisplayUserControl.xaml
/// </summary>
public partial class VerticalPlayerDisplayUserControl : UserControl
{
    public VerticalPlayerDisplayUserControl()
    {
        InitializeComponent();
        DataContext = this;
    }


    /// <summary>
    /// Using a DependencyProperty as the backing store for ActivePlayer.
    /// </summary>
    public static readonly DependencyProperty VerticalActivePlayerProperty =
        DependencyProperty.Register(nameof(VerticalActivePlayer), typeof(PlayerDisplay),
            typeof(VerticalPlayerDisplayUserControl), new PropertyMetadata(new PlayerDisplay()));

    /// <summary>
    /// Using a DependencyProperty as the backing store for DealerIconVisibility.
    /// </summary>
    public static readonly DependencyProperty VerticalDealerIconVisibilityProperty =
        DependencyProperty.Register(nameof(VerticalDealerIconVisibility), typeof(Visibility),
            typeof(VerticalPlayerDisplayUserControl), new PropertyMetadata(Visibility.Hidden));

    /// <summary>
    /// Using a DependencyProperty as the backing store for BidderPartnerVisibility.
    /// </summary>
    public static readonly DependencyProperty BidderPartnerIconVisibilityProperty =
        DependencyProperty.Register(nameof(BidderPartnerIconVisibility), typeof(Visibility),
            typeof(VerticalPlayerDisplayUserControl), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for OppositionIconVisibility.
    /// </summary>
    public static readonly DependencyProperty OppositionIconVisibilityProperty =
        DependencyProperty.Register(nameof(OppositionIconVisibility), typeof(Visibility),
            typeof(VerticalPlayerDisplayUserControl), new PropertyMetadata(Visibility.Collapsed));


    /// <summary>
    /// The information about the user associated with this control.
    /// </summary>
    public PlayerDisplay VerticalActivePlayer
    {
        get => (PlayerDisplay)GetValue(VerticalActivePlayerProperty);
        set => SetValue(VerticalActivePlayerProperty, value);
    }

    /// <summary>
    /// Displays/hides the dealer icon on the control.
    /// </summary>
    public Visibility VerticalDealerIconVisibility
    {
        get => (Visibility)GetValue(VerticalDealerIconVisibilityProperty);
        set => SetValue(VerticalDealerIconVisibilityProperty, value);
    }

    /// <summary>
    /// The visibility setting of the icons when the player is either the bidder or their partner.
    /// </summary>
    public Visibility BidderPartnerIconVisibility
    {
        get => (Visibility)GetValue(BidderPartnerIconVisibilityProperty);
        set => SetValue(BidderPartnerIconVisibilityProperty, value);
    }

    /// <summary>
    /// The visibility setting of the icons when the player is a member of the opposition.
    /// </summary>
    public Visibility OppositionIconVisibility
    {
        get => (Visibility)GetValue(OppositionIconVisibilityProperty);
        set => SetValue(OppositionIconVisibilityProperty, value);
    }

    /// <summary>
    /// A list of visibility values corresponding to the number of tricks won by player when they are the 
    /// bidder or their partner, if they have one.
    /// </summary>
    public ObservableCollection<Visibility> BidderTrickIconsVisibility { get; set; } = new()
    {
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
    };

    /// <summary>
    /// A list of visibility values corresponding to the number of tricks won by the player when they are a 
    /// member of the opposition.
    /// </summary>
    public ObservableCollection<Visibility> OppositionTrickIconsVisibility { get; set; } = new()
    {
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
    };

    /// <summary>
    /// Indicates whether the player is the bidder, their partner, a member of the opposition or if the 
    /// player's status is currently unknown.
    /// </summary>
    public PlayerRoundState PlayerBidStatus { get; set; } = PlayerRoundState.None;




    /// <summary>
    /// Sets the player information to display on the control.
    /// </summary>
    /// <param name="activePlayer">The player to display.</param>
    /// <param name="avatarNumber">The avatar associated with the player.</param>
    public void SetActivePlayer(IPlayer activePlayer, int avatarNumber)
    {
        VerticalActivePlayer = new PlayerDisplay()
        {
            Player = activePlayer,
            AvatarNumber = avatarNumber,
            TotalTricks = 0,
        };
    }

    /// <summary>
    /// Changes the bidding status of the player and changes which icons are displayed for the number of
    /// tricks won by the player.
    /// </summary>
    /// <param name="newPlayerStatus">The new bid status for the player.</param>
    public void SetPlayersBidStatus(PlayerRoundState newPlayerStatus)
    {
        PlayerBidStatus = newPlayerStatus;

        // Set the visibility of the number of tricks icons.

        BidderPartnerIconVisibility = PlayerBidStatus == PlayerRoundState.BiddingTeam ?
            Visibility.Visible : Visibility.Collapsed;
        OppositionIconVisibility = PlayerBidStatus == PlayerRoundState.OppositionTeam ?
            Visibility.Visible : Visibility.Collapsed;

        // Reset the display of the number of tricks icons.

        UpdateNumberOfTricks(VerticalActivePlayer.TotalTricks);
    }

    /// <summary>
    /// Updates the display of the number of tricks for the player to a specified number.
    /// </summary>
    /// <param name="newNumberOfTricks">The number of tricks for the player.</param>
    public void UpdateNumberOfTricks(int newNumberOfTricks)
    {
        VerticalActivePlayer.TotalTricks = newNumberOfTricks;

        if (newNumberOfTricks == 0)
        {
            ResetIconVisibility();
        }
        else
        {
            // Update the icons that display the number of tricks won by the player.

            if (PlayerBidStatus == PlayerRoundState.BiddingTeam)
            {
                SetBidderAndPartnerTrickNumbers(VerticalActivePlayer.TotalTricks);
            }
            else
            { 
                SetOppositionTrickNumbers(VerticalActivePlayer.TotalTricks);
            }
        }
    }

    /// <summary>
    /// Sets the icon that indicates the user associated with this control is the dealer.
    /// </summary>
    /// <param name="isVisible">True to indicate the player is the dealer.</param>
    public void SetDealerIconVisibility(bool isVisible)
    {
        VerticalDealerIconVisibility = isVisible ? Visibility.Visible : Visibility.Hidden;
    }

    /// <summary>
    /// Resets the visibility status of each of the bidding status icons.
    /// </summary>
    private void ResetIconVisibility()
    {
        for (int index = 0; index < BidderTrickIconsVisibility.Count; index++)
        {
            BidderTrickIconsVisibility[index] = Visibility.Hidden;
        }
        for (int index = 0; index < OppositionTrickIconsVisibility.Count; index++)
        {
            OppositionTrickIconsVisibility[index] = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Sets the number of icons of tricks won when the player is the bidder or their partner.
    /// </summary>
    /// <param name="numberOfTricks">The number of icons to show.</param>
    private void SetBidderAndPartnerTrickNumbers(int numberOfTricks)
    {
        if (numberOfTricks < 6)
        {
            for (int count = 1; count <= numberOfTricks; count++)
            {
                BidderTrickIconsVisibility[count - 1] = Visibility.Visible;
            }
        }
    }

    /// <summary>
    /// Sets the number of icons of tricks won by the player when they are a member of the opposition.
    /// </summary>
    /// <param name="numberOfTricks">The number of icons to show.</param>
    private void SetOppositionTrickNumbers(int numberOfTricks)
    {
        if (numberOfTricks < 5)
        {
            for (int count = 1; count <= numberOfTricks; count++)
            {
                OppositionTrickIconsVisibility[count - 1] = Visibility.Visible;
            }
        }
    }
}
