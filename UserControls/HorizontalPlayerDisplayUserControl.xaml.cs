using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using Euchre.UILogic.Interfaces;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for HorizontalPlayerDisplayUserControl.xaml
/// </summary>
public partial class HorizontalPlayerDisplayUserControl : UserControl, IBasePlayerDisplay
{
    public HorizontalPlayerDisplayUserControl()
    {
        InitializeComponent();
        TricksColour = new SolidColorBrush(UIConstants.Team1Colour);
        SuitImage = new Image();
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
    /// Using a DependencyProperty as the backing store for SuitImage.
    /// </summary>
    public static readonly DependencyProperty SuitImageProperty =
        DependencyProperty.Register(nameof(SuitImage), typeof(Image),
            typeof(HorizontalPlayerDisplayUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for SuitIconVisibility.
    /// </summary>
    public static readonly DependencyProperty SuitIconVisibilityProperty =
        DependencyProperty.Register(nameof(SuitIconVisibility), typeof(Visibility),
            typeof(HorizontalPlayerDisplayUserControl), new PropertyMetadata(Visibility.Hidden));


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
    /// Sets the visibility of the icon displaying the suit that the player called trump.
    /// </summary>
    public Visibility SuitIconVisibility
    {
        get => (Visibility)GetValue(SuitIconVisibilityProperty);
        set => SetValue(SuitIconVisibilityProperty, value);
    }

    /// <summary>
    /// Contains the image of the suit that the player called trump.
    /// </summary>
    public Image SuitImage
    {
        get => (Image)GetValue(SuitImageProperty);
        set => SetValue(SuitImageProperty, value);
    }

    /// <summary>
    /// A list of visibility values corresponding to the number of tricks won by player.
    /// </summary>
    public ObservableCollection<Visibility> TrickIconsVisibility { get; set; } =
    [
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
    ];

    /// <summary>
    /// The colour to use for the number of trick won by the player.
    /// </summary>
    public Brush TricksColour { get; }

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


    public void SetTrumpSuit(Suit trump)
    {
        SuitImage = new Image()
        {
            Source = UIHelpers.GenerateSuitImageSource(trump)
        };
        SuitIconVisibility = Visibility.Visible;
    }

    public void ClearTrumpSuit()
    {
        SuitImage.Source = null;
        SuitIconVisibility = Visibility.Hidden;
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
            SetBiddingTeamTrickNumbers(ActivePlayer.TotalTricks);
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
        for (int index = 0; index < TrickIconsVisibility.Count; index++)
        {
            TrickIconsVisibility[index] = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Sets the number of icons of tricks won when the player is the bidder or their partner.
    /// </summary>
    /// <param name="numberOfTricks">The number of icons to show.</param>
    private void SetBiddingTeamTrickNumbers(int numberOfTricks)
    {
        if (numberOfTricks <= Constants.MAX_NUMBER_OF_TRICKS)
        {
            for (int count = 1; count <= numberOfTricks; count++)
            {
                TrickIconsVisibility[count - 1] = Visibility.Visible;
            }
        }
    }
}
