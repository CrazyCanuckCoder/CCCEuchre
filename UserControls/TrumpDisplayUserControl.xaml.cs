using Euchre.Logic.Components;
using Euchre.UILogic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for TrumpDisplayUserControl.xaml
/// </summary>
public partial class TrumpDisplayUserControl : UserControl
{
    public TrumpDisplayUserControl()
    {
        InitializeComponent();
    }


    /// <summary>
    /// Using a DependencyProperty as the backing store for TrumpImage.
    /// </summary>
    public static readonly DependencyProperty TrumpImageProperty =
        DependencyProperty.Register(nameof(TrumpImage), typeof(Image), typeof(TrumpDisplayUserControl),
            new PropertyMetadata(null));

    /// <summary>
    /// The image to display for trump.
    /// </summary>
    public Image TrumpImage
    {
        get => (Image)GetValue(TrumpImageProperty); 
        set => SetValue(TrumpImageProperty, value);
    }

    /// <summary>
    /// Sets the image to display based on a specified suit.
    /// </summary>
    /// <param name="trumpSuit">The suit to display as trump.</param>
    public void SetTrump(Suit trumpSuit)
    {
        string uriSource = "pack://application:,,,/Euchre;component/images/suits/";
        switch (trumpSuit)
        {
            case Suit.Clubs:
                uriSource += "Clubs";
                break;

            case Suit.Diamonds:
                uriSource += "Diamonds";
                break;

            case Suit.Hearts:
                uriSource += "Hearts";
                break;

            case Suit.Spades:
                uriSource += "Spades";
                break;
        }
        TrumpImage.Source = UIHelpers.GenerateImageSource(uriSource + ".png");
    }

    /// <summary>
    /// Clears the image that was displaying the current trump suit.
    /// </summary>
    public void ClearTrump()
    {
        TrumpImage.Source = null; 
    }

}
