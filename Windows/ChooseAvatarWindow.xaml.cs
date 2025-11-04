using Euchre.UILogic;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for ChooseAvatarWindow.xaml
/// </summary>
public partial class ChooseAvatarWindow : Window
{
    public ChooseAvatarWindow()
    {
        InitializeComponent();
        AvatarImages = [];
        SetupAvatars();
        DataContext = this;
    }


    /// <summary>
    /// The collection of images to display for the avatars.
    /// </summary>
    public ObservableCollection<AvatarDisplay> AvatarImages { get; set; }

    /// <summary>
    /// Gets the number associated to the avatar selected by the user.
    /// </summary>
    public int AvatarNumber { get; private set; }


    /// <summary>
    /// Adds the avatar images to the collection to display on the window.
    /// </summary>
    public void SetupAvatars()
    {
        // Load each avatar in order.

        for (int avatarNumber = 1; avatarNumber <= UIConstants.MaximumNumberOfAvatars; avatarNumber++)
        {
            // Add the image to the collection of images.

            AvatarImages.Add(new AvatarDisplay()
            {
                ImageData = new Image()
                {
                    Source = UIHelpers.GenerateAvatarImageSource(avatarNumber)
                },
                AvatarNumber = avatarNumber,
            });
        }
    }

    /// <summary>
    /// Sets the value for the AvatarNumber property based on which toggle button is checked.
    /// </summary>
    /// <param name="toggleButton">The toggle button that was checked by the user.</param>
    private void SetUpChosenAvatar(ToggleButton toggleButton)
    {
        if (toggleButton.Tag is int avatarNumber)
        {
            AvatarNumber = avatarNumber;
        }
    }


    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        // Uncheck the toggle buttons on the form except for the one just checked by the user.

        if (sender is ToggleButton currentButton)
        {
            this.UncheckOtherToggleButtons(currentButton);

            // Set up the selected card property.

            SetUpChosenAvatar(currentButton);
        }
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        if (AvatarNumber > 0)
        {
            DialogResult = true;
            Close();
        }
        else
        {
            DialogBoxes.ErrorDialog("An avatar must be selected.", this);
        }
    }
}
