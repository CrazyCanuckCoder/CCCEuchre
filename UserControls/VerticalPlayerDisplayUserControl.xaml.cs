using Euchre.Logic.Helpers;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Euchre.UserControls;
/// <summary>
/// Interaction logic for VerticalPlayerDisplayUserControl.xaml
/// </summary>
public partial class VerticalPlayerDisplayUserControl : UserControl
{
    public VerticalPlayerDisplayUserControl()
    {
        InitializeComponent();
        TricksColour = new SolidColorBrush(UIConstants.Team2Colour);
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
    /// A list of visibility values corresponding to the number of tricks won by the player when they are a 
    /// member of the opposition.
    /// </summary>
    public ObservableCollection<Visibility> TrickIconsVisibility { get; set; } = new()
    {
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
        Visibility.Hidden,
    };

    /// <summary>
    /// The colour to use for the number of trick won by the player.
    /// </summary>
    public SolidColorBrush TricksColour { get; }


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
            SetTrickNumbers(VerticalActivePlayer.TotalTricks);
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
        for (int index = 0; index < TrickIconsVisibility.Count; index++)
        {
            TrickIconsVisibility[index] = Visibility.Hidden;
        }
    }

    /// <summary>
    /// Sets the number of icons of tricks won by the player.
    /// </summary>
    /// <param name="numberOfTricks">The number of icons to show.</param>
    private void SetTrickNumbers(int numberOfTricks)
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
