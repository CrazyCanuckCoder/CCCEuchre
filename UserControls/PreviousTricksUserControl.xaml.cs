using Euchre.Logic.Components;
using Euchre.UILogic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for PreviousTricksUserControl.xaml
/// </summary>
public partial class PreviousTricksUserControl : UserControl
{
    public PreviousTricksUserControl()
    {
        InitializeComponent();
        InitializeIndicatorImages();
        DataContext = this;
    }

    /// <summary>
    /// Tracks the index from the collection of played tricks for the displayed trick.
    /// </summary>
    private int _currentTrickIndex = -1;

    #region Backing Properties

    /// <summary>
    /// Using a DependencyProperty as the backing store for PreviousTrickButtonEnabled.
    /// </summary>
    public static readonly DependencyProperty PreviousTrickButtonEnabledProperty =
        DependencyProperty.Register(nameof(PreviousTrickButtonEnabled), typeof(bool),
            typeof(PreviousTricksUserControl), new PropertyMetadata(false));

    /// <summary>
    /// Using a DependencyProperty as the backing store for NextTrickButtonEnabled.
    /// </summary>
    public static readonly DependencyProperty NextTrickButtonEnabledProperty =
        DependencyProperty.Register(nameof(NextTrickButtonEnabled), typeof(bool),
            typeof(PreviousTricksUserControl), new PropertyMetadata(false));

    /// <summary>
    /// Using a DependencyProperty as the backing store for FirstPlayerImage.
    /// </summary>
    public static readonly DependencyProperty FirstPlayerImageProperty =
        DependencyProperty.Register(nameof(FirstPlayerImage), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for SecondPlayerImage.
    /// </summary>
    public static readonly DependencyProperty SecondPlayerImageProperty =
        DependencyProperty.Register(nameof(SecondPlayerImage), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for ThirdPlayerImage.
    /// </summary>
    public static readonly DependencyProperty ThirdPlayerImageProperty =
        DependencyProperty.Register(nameof(ThirdPlayerImage), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for FourthPlayerImage.
    /// </summary>
    public static readonly DependencyProperty FourthPlayerImageProperty =
        DependencyProperty.Register(nameof(FourthPlayerImage), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for FirstPlayerTrickWonIndicator.
    /// </summary>
    public static readonly DependencyProperty FirstPlayerTrickWonIndicatorProperty =
        DependencyProperty.Register(nameof(FirstPlayerTrickWonIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for FirstPlayerCardLeadIndicator.
    /// </summary>
    public static readonly DependencyProperty FirstPlayerCardLeadIndicatorProperty =
        DependencyProperty.Register(nameof(FirstPlayerCardLeadIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for SecondPlayerFirstIndicator.
    /// </summary>
    public static readonly DependencyProperty SecondPlayerFirstIndicatorProperty =
        DependencyProperty.Register(nameof(SecondPlayerFirstIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for SecondPlayerSecondIndicator.
    /// </summary>
    public static readonly DependencyProperty SecondPlayerSecondIndicatorProperty =
        DependencyProperty.Register(nameof(SecondPlayerSecondIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for ThirdPlayerTrickWonIndicator.
    /// </summary>
    public static readonly DependencyProperty ThirdPlayerTrickWonIndicatorProperty =
        DependencyProperty.Register(nameof(ThirdPlayerTrickWonIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for ThirdPlayerCardLeadIndicator.
    /// </summary>
    public static readonly DependencyProperty ThirdPlayerCardLeadIndicatorProperty =
        DependencyProperty.Register(nameof(ThirdPlayerCardLeadIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for FourthPlayerFirstIndicator.
    /// </summary>
    public static readonly DependencyProperty FourthPlayerFirstIndicatorProperty =
        DependencyProperty.Register(nameof(FourthPlayerFirstIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for FourthPlayerSecondIndicator.
    /// </summary>
    public static readonly DependencyProperty FourthPlayerSecondIndicatorProperty =
        DependencyProperty.Register(nameof(FourthPlayerSecondIndicator), typeof(Image),
            typeof(PreviousTricksUserControl), new PropertyMetadata(null));

    #endregion Backing Properties

    /// <summary>
    /// Contains the list of card played for all tricks and the player that won the trick.
    /// </summary>
    public ObservableCollection<Trick> TricksHistory { get; set; } = [];

    /// <summary>
    /// Enables/disables the previous trick button based on the index of the current trick.
    /// </summary>
    public bool PreviousTrickButtonEnabled
    {
        get => (bool)GetValue(PreviousTrickButtonEnabledProperty);
        set => SetValue(PreviousTrickButtonEnabledProperty, value);
    }

    /// <summary>
    /// Enables/disables the next trick button based on the index of the current trick.
    /// </summary>
    public bool NextTrickButtonEnabled
    {
        get => (bool)GetValue(NextTrickButtonEnabledProperty);
        set => SetValue(NextTrickButtonEnabledProperty, value);
    }

    /// <summary>
    /// The card image for the first player.
    /// </summary>
    public Image FirstPlayerImage
    {
        get => (Image)GetValue(FirstPlayerImageProperty);
        set => SetValue(FirstPlayerImageProperty, value);
    }

    /// <summary>
    /// The card image for the second player.
    /// </summary>
    public Image SecondPlayerImage
    {
        get => (Image)GetValue(SecondPlayerImageProperty);
        set => SetValue(SecondPlayerImageProperty, value);
    }

    /// <summary>
    /// The card image for the third player.
    /// </summary>
    public Image ThirdPlayerImage
    {
        get => (Image)GetValue(ThirdPlayerImageProperty);
        set => SetValue(ThirdPlayerImageProperty, value);
    }

    /// <summary>
    /// The card image for the fourth player.
    /// </summary>
    public Image FourthPlayerImage
    {
        get => (Image)GetValue(FourthPlayerImageProperty);
        set => SetValue(FourthPlayerImageProperty, value);
    }

    /// <summary>
    /// The image to use for the first player's trick won indicator.
    /// </summary>
    public Image FirstPlayerTrickWonIndicator
    {
        get => (Image)GetValue(FirstPlayerTrickWonIndicatorProperty);
        set => SetValue(FirstPlayerTrickWonIndicatorProperty, value);
    }

    /// <summary>
    /// The image to use for the first player's card lead indicator.
    /// </summary>
    public Image FirstPlayerCardLeadIndicator
    {
        get => (Image)GetValue(FirstPlayerCardLeadIndicatorProperty);
        set => SetValue(FirstPlayerCardLeadIndicatorProperty, value);
    }

    /// <summary>
    /// The first indicator to show beside the second player's card.
    /// </summary>
    public Image SecondPlayerFirstIndicator
    {
        get => (Image)GetValue(SecondPlayerFirstIndicatorProperty);
        set => SetValue(SecondPlayerFirstIndicatorProperty, value);
    }

    /// <summary>
    /// The second indicator to show beside the second player's card.
    /// </summary>
    public Image SecondPlayerSecondIndicator
    {
        get => (Image)GetValue(SecondPlayerSecondIndicatorProperty);
        set => SetValue(SecondPlayerSecondIndicatorProperty, value);
    }

    /// <summary>
    /// The image to use for the third player's trick won indicator.
    /// </summary>
    public Image ThirdPlayerTrickWonIndicator
    {
        get => (Image)GetValue(ThirdPlayerTrickWonIndicatorProperty);
        set => SetValue(ThirdPlayerTrickWonIndicatorProperty, value);
    }

    /// <summary>
    /// The image to use for the third player's card lead indicator.
    /// </summary>
    public Image ThirdPlayerCardLeadIndicator
    {
        get => (Image)GetValue(ThirdPlayerCardLeadIndicatorProperty);
        set => SetValue(ThirdPlayerCardLeadIndicatorProperty, value);
    }

    /// <summary>
    /// The first indicator to show beside the fourth player's card.
    /// </summary>
    public Image FourthPlayerFirstIndicator
    {
        get => (Image)GetValue(FourthPlayerFirstIndicatorProperty);
        set => SetValue(FourthPlayerFirstIndicatorProperty, value);
    }

    /// <summary>
    /// The second indicator to show beside the fourth player's card.
    /// </summary>
    public Image FourthPlayerSecondIndicator
    {
        get => (Image)GetValue(FourthPlayerSecondIndicatorProperty);
        set => SetValue(FourthPlayerSecondIndicatorProperty, value);
    }

    /// <summary>
    /// Adds a trick to the collection of tricks.
    /// </summary>
    /// <param name="newTrick">The new trick to add to the collection.</param>
    public void AddTrick(Trick newTrick)
    {
        TricksHistory.Add(newTrick);
        _currentTrickIndex = TricksHistory.Count - 1;
        DisplayTrick();
    }

    /// <summary>
    /// Removes all tricks from the control.
    /// </summary>
    public void ClearTricks()
    {
        _currentTrickIndex = -1;
        TricksHistory.Clear();
        DisplayTrick();
    }

    /// <summary>
    /// Decrements the current trick index, updates the button enabled properties, and displays the current
    /// trick in the user interface.
    /// </summary>
    private void PreviousTrick()
    {
        if (_currentTrickIndex > 0)
        {
            _currentTrickIndex--;
        }
        DisplayTrick();
    }

    /// <summary>
    /// Increments the current trick index, updates the button enabled properties, and displays the current
    /// trick in the user interface.
    /// </summary>
    private void NextTrick()
    {
        if (_currentTrickIndex < TricksHistory.Count - 1)
        {
            _currentTrickIndex++;
        }
        DisplayTrick();
    }

    /// <summary>
    /// Enables or disables the navigation buttons based on the value of the current trick index.
    /// </summary>
    private void UpdateNavigationButtonsEnabledState()
    {
        PreviousTrickButtonEnabled = _currentTrickIndex > 0;
        NextTrickButtonEnabled = _currentTrickIndex < TricksHistory.Count - 1;
    }

    /// <summary>
    /// Updates the user interface with the trick specified by the current trick index.
    /// </summary>
    private void DisplayTrick()
    {
        ResetIndicatorImages();

        // Check if the list of tricks is empty.

        if (_currentTrickIndex == -1)
        {
            ResetCardImages();
        }
        else
        {
            // Set the indicator icons first.

            SetCardLeadIndicator(TricksHistory[_currentTrickIndex].Cards.First().Key.PlayerIndex);
            SetTrickWonIndicator(TricksHistory[_currentTrickIndex].GetWinner().PlayerIndex);

            // Set each card image in the trick.

            foreach (var cardPlayed in TricksHistory[_currentTrickIndex].Cards)
            {
                BitmapImage source = GenerateCardImage(cardPlayed.Value);
                AssignImageToPlayerSlot(cardPlayed.Key.PlayerIndex, source);
            }
        }
        UpdateNavigationButtonsEnabledState();
    }

    /// <summary>
    /// Sets the correct player's card image.
    /// </summary>
    /// <param name="playerIndex">The index of the player.</param>
    /// <param name="source">The card image for the player.</param>
    private void AssignImageToPlayerSlot(int playerIndex, BitmapImage source)
    {
        switch (playerIndex)
        {
            case 0:
                FirstPlayerImage = new Image()
                {
                    Source = source
                };
                break;

            case 1:
                SecondPlayerImage = new Image()
                {
                    Source = source
                };
                break;

            case 2:
                ThirdPlayerImage = new Image()
                {
                    Source = source
                };
                break;

            case 3:
                FourthPlayerImage = new Image()
                {
                    Source = source
                };
                break;
        }
    }

    /// <summary>
    /// Creates the image to be used for a specified card.
    /// </summary>
    /// <param name="cardPlayed">The card to generate the image for.</param>
    /// <returns>A BitmapImage containing the image for the card.</returns>
    private BitmapImage GenerateCardImage(Card cardPlayed)
    {
        return UIHelpers.GenerateCardImageSource(cardPlayed, false, false);
    }

    /// <summary>
    /// Initializes each trick won and card lead indicator.
    /// </summary>
    private void InitializeIndicatorImages()
    {
        FirstPlayerCardLeadIndicator = new Image();
        FirstPlayerTrickWonIndicator = new Image();
        SecondPlayerFirstIndicator   = new Image();
        SecondPlayerSecondIndicator  = new Image();
        ThirdPlayerCardLeadIndicator = new Image();
        ThirdPlayerTrickWonIndicator = new Image();
        FourthPlayerFirstIndicator   = new Image();
        FourthPlayerSecondIndicator  = new Image();
    }

    /// <summary>
    /// Resets each card image's source to blank out the trick display.
    /// </summary>
    private void ResetCardImages()
    {
        // The Image properties will be null until a trump suit is called.  One of them will be null when a
        //  player goes alone.

        if (FirstPlayerImage != null)
        {
            FirstPlayerImage.Source = null;
        }
        if (SecondPlayerImage != null)
        {
            SecondPlayerImage.Source = null;
        }
        if (ThirdPlayerImage != null)
        {
            ThirdPlayerImage.Source = null;
        }
        if (FourthPlayerImage != null)
        {
            FourthPlayerImage.Source = null;
        }
    }

    /// <summary>
    /// Resets each trick won and card lead indicator to null.
    /// </summary>
    private void ResetIndicatorImages()
    {
        FirstPlayerCardLeadIndicator.Source = null;
        FirstPlayerTrickWonIndicator.Source = null;
        SecondPlayerFirstIndicator.Source   = null;
        SecondPlayerSecondIndicator.Source  = null;
        ThirdPlayerCardLeadIndicator.Source = null;
        ThirdPlayerTrickWonIndicator.Source = null;
        FourthPlayerFirstIndicator.Source   = null;
        FourthPlayerSecondIndicator.Source  = null;
    }

    /// <summary>
    /// Sets the card lead indicator for a specified player.
    /// </summary>
    /// <param name="playerIndex">The index value of the player for the indicator.</param>
    private void SetCardLeadIndicator(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                FirstPlayerCardLeadIndicator = new Image()
                {
                    Source = GenerateCardLeadIndicatorSource()
                };
                break;

            case 1:
                if (SecondPlayerFirstIndicator.Source == null)
                {
                    SecondPlayerFirstIndicator = new Image()
                    {
                        Source = GenerateCardLeadIndicatorSource()
                    };
                }
                else
                {
                    SecondPlayerSecondIndicator = new Image()
                    {
                        Source = GenerateCardLeadIndicatorSource()
                    };
                }
                break;

            case 2:
                ThirdPlayerCardLeadIndicator = new Image()
                {
                    Source = GenerateCardLeadIndicatorSource()
                };
                break;

            case 3:
                if (FourthPlayerFirstIndicator.Source == null)
                {
                    FourthPlayerFirstIndicator = new Image()
                    {
                        Source = GenerateCardLeadIndicatorSource()
                    };
                }
                else
                {
                    FourthPlayerSecondIndicator = new Image()
                    {
                        Source = GenerateCardLeadIndicatorSource()
                    };
                }
                break;
        }
    }

    /// <summary>
    /// Sets the trick won indicator for a specified player.
    /// </summary>
    /// <param name="playerIndex">The index value of the player for the indicator.</param>
    private void SetTrickWonIndicator(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                FirstPlayerTrickWonIndicator = new Image()
                {
                    Source = GenerateTrickWonIndicatorSource()
                };
                break;

            case 1:
                if (SecondPlayerFirstIndicator.Source == null)
                {
                    SecondPlayerFirstIndicator = new Image()
                    {
                        Source = GenerateTrickWonIndicatorSource()
                    };
                }
                else
                {
                    SecondPlayerSecondIndicator = new Image()
                    {
                        Source = GenerateTrickWonIndicatorSource()
                    };
                }
                break;

            case 2:
                ThirdPlayerTrickWonIndicator = new Image()
                {
                    Source = GenerateTrickWonIndicatorSource()
                };
                break;

            case 3:
                if (FourthPlayerFirstIndicator.Source == null)
                {
                    FourthPlayerFirstIndicator = new Image()
                    {
                        Source = GenerateTrickWonIndicatorSource()
                    };
                }
                else
                {
                    FourthPlayerSecondIndicator = new Image()
                    {
                        Source = GenerateTrickWonIndicatorSource()
                    };
                }
                break;
        }
    }

    /// <summary>
    /// Creates the image to be used the card lead indicator.
    /// </summary>
    /// <returns>A BitmapImage containing the image for the indicator.</returns>
    private BitmapImage GenerateCardLeadIndicatorSource()
    {
        return UIHelpers.GenerateImageSource(
            "pack://application:,,,/Euchre;component/images/icons/lead card indicator.png");
    }

    /// <summary>
    /// Creates the image to be used the trick won indicator.
    /// </summary>
    /// <returns>A BitmapImage containing the image for the indicator.</returns>
    private BitmapImage GenerateTrickWonIndicatorSource()
    {
        return UIHelpers.GenerateImageSource(
            "pack://application:,,,/Euchre;component/images/icons/trick won indicator.png");
    }

    private void PreviousButton_Click(object sender, RoutedEventArgs e)
    {
        PreviousTrick();
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        NextTrick();
    }
}
