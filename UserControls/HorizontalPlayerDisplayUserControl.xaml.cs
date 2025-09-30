using Euchre.Logic.Enums;
using Euchre.Logic.Interfaces;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for HorizontalPlayerDisplayUserControl.xaml
/// </summary>
public partial class HorizontalPlayerDisplayUserControl : UserControl
{
    public HorizontalPlayerDisplayUserControl()
    {
        InitializeComponent();
        DataContext = this;
    }


    /// <summary>
    /// Using a DependencyProperty as the backing store for ActivePlayer.
    /// </summary>
    public static readonly DependencyProperty ActivePlayerProperty =
        DependencyProperty.Register(nameof(ActivePlayer), typeof(PlayerDisplay),
            typeof(HorizontalPlayerDisplayUserControl),
            new PropertyMetadata(new PlayerDisplay()));

    /// <summary>
    /// Using a DependencyProperty as the backing store for DealerIconVisibility.
    /// </summary>
    public static readonly DependencyProperty DealerIconVisibilityProperty =
        DependencyProperty.Register(nameof(DealerIconVisibility), typeof(Visibility),
            typeof(HorizontalPlayerDisplayUserControl), new PropertyMetadata(Visibility.Hidden));

    /// <summary>
    /// Using a DependencyProperty as the backing store for BidderPartnerVisibility.
    /// </summary>
    public static readonly DependencyProperty BidderPartnerIconVisibilityProperty =
        DependencyProperty.Register(nameof(BidderPartnerIconVisibility), typeof(Visibility),
            typeof(HorizontalPlayerDisplayUserControl), new PropertyMetadata(Visibility.Collapsed));

    /// <summary>
    /// Using a DependencyProperty as the backing store for OppositionIconVisibility.
    /// </summary>
    public static readonly DependencyProperty OppositionIconVisibilityProperty =
        DependencyProperty.Register(nameof(OppositionIconVisibility), typeof(Visibility),
            typeof(HorizontalPlayerDisplayUserControl), new PropertyMetadata(Visibility.Collapsed));


    /// <summary>
    /// The information about the user associated with this control.
    /// </summary>
    public PlayerDisplay ActivePlayer
    {
        get => (PlayerDisplay)GetValue(ActivePlayerProperty);
        set => SetValue(ActivePlayerProperty, value);
    }

    /// <summary>
    /// Displays/hides the dealer icon on the control.
    /// </summary>
    public Visibility DealerIconVisibility
    {
        get => (Visibility)GetValue(DealerIconVisibilityProperty);
        set => SetValue(DealerIconVisibilityProperty, value);
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
        ActivePlayer = new PlayerDisplay()
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
        switch (PlayerBidStatus)
        {
            case PlayerRoundState.None:
                BidderPartnerIconVisibility = Visibility.Collapsed;
                OppositionIconVisibility = Visibility.Collapsed;
                break;

            case PlayerRoundState.BiddingTeam:
                BidderPartnerIconVisibility = Visibility.Visible;
                OppositionIconVisibility = Visibility.Collapsed;
                break;

            case PlayerRoundState.OppositionTeam:
                BidderPartnerIconVisibility = Visibility.Collapsed;
                OppositionIconVisibility = Visibility.Visible;
                break;
        }

        // Reset the display of the number of tricks icons.

        UpdateNumberOfTricks(ActivePlayer.TotalTricks);
    }

    /// <summary>
    /// Updates the display of the number of tricks for the player to a specified number.
    /// </summary>
    /// <param name="newNumberOfTricks">The number of tricks for the player.</param>
    public void UpdateNumberOfTricks(int newNumberOfTricks)
    {
        ActivePlayer.TotalTricks = newNumberOfTricks;

        if (newNumberOfTricks == 0)
        {
            ResetIconVisibility();
        }
        else
        {
            // Update the icons that display the number of tricks won by the player.

            switch (PlayerBidStatus)
            {
                case PlayerRoundState.BiddingTeam:
                    SetBiddingTeamTrickNumbers(ActivePlayer.TotalTricks);
                    break;

                case PlayerRoundState.OppositionTeam:
                    SetOppositionTrickNumbers(ActivePlayer.TotalTricks);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets the icon that indicates the user associated with this control is the dealer.
    /// </summary>
    /// <param name="isVisible">True to indicate the player is the dealer.</param>
    public void SetDealerIconVisibility(bool isVisible)
    {
        DealerIconVisibility = isVisible ? Visibility.Visible : Visibility.Hidden;
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
    private void SetBiddingTeamTrickNumbers(int numberOfTricks)
    {
        if (numberOfTricks < 9)
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
