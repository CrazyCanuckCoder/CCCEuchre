using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for CurrentRoundInfoUserControl.xaml
/// </summary>
public partial class CurrentRoundInfoUserControl : UserControl
{
    public CurrentRoundInfoUserControl()
    {
        InitializeComponent();
        Team1Colour = new SolidColorBrush(UIConstants.Team1Colour);
        Team2Colour = new SolidColorBrush(UIConstants.Team2Colour);
        DataContext = this;
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for BidInfo.
    /// </summary>
    public static readonly DependencyProperty BidInfoProperty =
        DependencyProperty.Register(nameof(BidInfo), typeof(string), typeof(CurrentRoundInfoUserControl),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Contains information about the current bid.
    /// </summary>
    public string BidInfo
    {
        get => (string)GetValue(BidInfoProperty); 
        set => SetValue(BidInfoProperty, value);
    }

    /// <summary>
    /// The colour of the icons for the first team.
    /// </summary>
    public Brush Team1Colour { get; }

    /// <summary>
    /// The colour of the icons for the second team.
    /// </summary>
    public Brush Team2Colour { get; }

    /// <summary>
    /// The list of the players for the first team.
    /// </summary>
    public ObservableCollection<IPlayer> Team1List { get; set; } = [];

    /// <summary>
    /// The list of the players for the second team.
    /// </summary>
    public ObservableCollection<IPlayer> Team2List { get; set; } = [];

    /// <summary>
    /// Sets the text for the bidding information based on the player, trump and went alone status.
    /// </summary>
    /// <param name="player">The player that called trump.</param>
    /// <param name="trump">The trump suit.</param>
    /// <param name="isAlone">True if the player went alone.</param>
    public void SetBidInformation(IPlayer player, Suit trump, bool isAlone)
    {
        ArgumentNullException.ThrowIfNull(player, nameof(player));

        BidInfo = $"{player.Name} called {trump} {(isAlone ? "Alone" : string.Empty)}";
    }

    /// <summary>
    /// Clears the text for the bidding information.
    /// </summary>
    public void ResetBidInformation()
    {
        BidInfo = string.Empty;
    }
}
