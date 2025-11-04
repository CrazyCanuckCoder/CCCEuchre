using Euchre.Logic.Components;
using System.Windows;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for PickTrumpSuitWindow.xaml
/// </summary>
public partial class PickTrumpSuitWindow : Window
{
    public PickTrumpSuitWindow(List<Suit> allowedSuits)
    {
        InitializeComponent();
        SetTrumpButtonsVisibility(allowedSuits);
        DataContext = this;
    }

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



    private void SetTrumpButtonsVisibility(List<Suit> allowedSuits)
    {
        foreach (Suit suit in allowedSuits)
        {
            switch (suit)
            {
                case Suit.Hearts:
                    HeartsVisibility = Visibility.Visible; 
                    break;

                case Suit.Diamonds:
                    DiamondsVisibility = Visibility.Visible;
                    break;

                case Suit.Clubs: 
                    ClubsVisibility = Visibility.Visible;
                    break;

                case Suit.Spades:
                    SpadesVisibility = Visibility.Visible;
                    break;
            }
        }
    }

    private void HeartsButton_Click(object? sender, RoutedEventArgs e)
    {
        SelectedSuit = Suit.Hearts;
        DialogResult = true;
        Close();
    }

    private void DiamondsButton_Click(object? sender, RoutedEventArgs e)
    {
        SelectedSuit = Suit.Diamonds;
        DialogResult = true;
        Close();
    }

    private void ClubsButton_Click(object? sender, RoutedEventArgs e)
    {
        SelectedSuit = Suit.Clubs;
        DialogResult = true;
        Close();
    }

    private void SpadesButton_Click(object? sender, RoutedEventArgs e)
    {
        SelectedSuit = Suit.Spades;
        DialogResult = true;
        Close();
    }

    private void PassButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
