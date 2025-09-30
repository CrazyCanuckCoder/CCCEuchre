using Euchre.UILogic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Euchre.UserControls;
/// <summary>
/// Interaction logic for AvatarUserControl.xaml
/// </summary>
public partial class AvatarUserControl : UserControl
{
    public AvatarUserControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for AvatarImage.
    /// </summary>
    public static readonly DependencyProperty AvatarImageProperty =
        DependencyProperty.Register(nameof(AvatarImage), typeof(Image), typeof(AvatarUserControl),
            new PropertyMetadata(null));

    /// <summary>
    /// Using a DependencyProperty as the backing store for AvatarNumber.
    /// </summary>
    public static readonly DependencyProperty AvatarNumberProperty =
        DependencyProperty.Register(nameof(AvatarNumber), typeof(int), typeof(AvatarUserControl),
            new PropertyMetadata(AvatarNumberChanged));

    /// <summary>
    /// Using a DependencyProperty as the backing store for HasBorder.
    /// </summary>
    public static readonly DependencyProperty HasBorderProperty =
        DependencyProperty.Register(nameof(HasBorder), typeof(bool), typeof(AvatarUserControl),
            new PropertyMetadata(true));


    /// <summary>
    /// Gets/sets the image for the avatar.
    /// </summary>
    public Image AvatarImage
    {
        get => (Image)GetValue(AvatarImageProperty);
        set => SetValue(AvatarImageProperty, value);
    }

    /// <summary>
    /// Gets/sets the avatar number which is associated with an image.
    /// </summary>
    public int AvatarNumber
    {
        get => (int)GetValue(AvatarNumberProperty);
        set => SetValue(AvatarNumberProperty, value);
    }

    /// <summary>
    /// True to display a border around the avatar image and false to hide it.
    /// </summary>
    public bool HasBorder
    {
        get => (bool)GetValue(HasBorderProperty);
        set => SetValue(HasBorderProperty, value);
    }





    /// <summary>
    /// Sets the image on the control to the avatar associated with a number.
    /// </summary>
    /// <param name="avatarNumber">The number of the avatar, that is, from 1 to 24.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetAvatar(int avatarNumber)
    {
        if (avatarNumber < 1 || avatarNumber > UIConstants.MaximumNumberOfAvatars)
        {
            throw new ArgumentOutOfRangeException(nameof(avatarNumber));
        }

        string imageFilename = $"avatar{avatarNumber}.png";
        BitmapImage source = new();
        source.BeginInit();
        source.UriSource =
            new Uri($"pack://application:,,,/Euchre;component/images/avatars/{imageFilename}");
        source.EndInit();
        AvatarImage = new Image()
        {
            Source = source
        };
    }

    private static void AvatarNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AvatarUserControl avatarUserControl)
        {
            avatarUserControl.SetAvatar(avatarUserControl.AvatarNumber);
        }
    }
}
