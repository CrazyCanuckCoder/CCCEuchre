using Euchre.Logic.Helpers;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for NewGameWindow.xaml
/// </summary>
public partial class NewGameWindow : Window
{
    public NewGameWindow()
    {
        InitializeComponent();
        DataContext = this;
        SetupPlayerAvatarCollections();
    }


    /// <summary>
    /// Using a DependencyProperty as the backing store for ClickHereLabelVisibility.
    /// </summary>
    public static readonly DependencyProperty ClickHereLabelVisibilityProperty =
        DependencyProperty.Register(nameof(ClickHereLabelVisibility), typeof(Visibility),
            typeof(NewGameWindow), new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Using a DependencyProperty as the backing store for UserAvatarVisibility.
    /// </summary>
    public static readonly DependencyProperty UserAvatarVisibilityProperty =
        DependencyProperty.Register(nameof(UserAvatarVisibility), typeof(Visibility), typeof(NewGameWindow),
            new PropertyMetadata(Visibility.Hidden));

    /// <summary>
    /// Using a DependencyProperty as the backing store for PlayerName.
    /// </summary>
    public static readonly DependencyProperty PlayerNameProperty =
        DependencyProperty.Register(nameof(PlayerName), typeof(string), typeof(NewGameWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets/sets the visibility setting for the description label on the button that selects the user's
    /// avatar.
    /// </summary>
    public Visibility ClickHereLabelVisibility
    {
        get => (Visibility)GetValue(ClickHereLabelVisibilityProperty);
        set => SetValue(ClickHereLabelVisibilityProperty, value);
    }

    /// <summary>
    /// Gets/sets the visibility setting for the user's avatar.
    /// </summary>
    public Visibility UserAvatarVisibility
    {
        get => (Visibility)GetValue(UserAvatarVisibilityProperty);
        set => SetValue(UserAvatarVisibilityProperty, value);
    }

    /// <summary>
    /// Gets the name the player entered.
    /// </summary>
    public string PlayerName
    {
        get => (string)GetValue(PlayerNameProperty);
        set => SetValue(PlayerNameProperty, value);
    }

    /// <summary>
    /// The number of the avatar image that the user selected for themselves.
    /// </summary>
    public int UserAvatarNumber { get; set; }

    /// <summary>
    /// The list of automated players and their avatars that the user can select from.
    /// </summary>
    public ObservableCollection<AutomatedPlayerAvatar> AvailablePlayerAvatars { get; set; }

    /// <summary>
    /// The list of automated players and their avatars that the user has selected.
    /// </summary>
    public ObservableCollection<AutomatedPlayerAvatar> SelectedPlayerAvatars { get; set; }


    /// <summary>
    /// Retrieves the selected players and their avatars.
    /// </summary>
    /// <returns>An IEnumerable containing the player names and selected avatars.</returns>
    public IEnumerable<AutomatedPlayerAvatar> GetPlayers()
    {
        // Return the user's information.

        yield return new AutomatedPlayerAvatar()
        {
            PlayerName = PlayerName,
            AvatarNumber = UserAvatarNumber,
        };

        // Return each automated player's information.

        foreach (var avatar in SelectedPlayerAvatars)
        {
            yield return avatar;
        }
    }

    /// <summary>
    /// Sets up the collections of avatars for the automated players.
    /// </summary>
    private void SetupPlayerAvatarCollections()
    {
        try
        {
            AvailablePlayerAvatars = new(GameSettingsManager.Instance.GetAutomatedPlayers());
            SelectedPlayerAvatars =
            [
                AvailablePlayerAvatars[0].Clone(),
                AvailablePlayerAvatars[1].Clone(),
                AvailablePlayerAvatars[2].Clone()
            ];

            // The selected index needs to be set for the combo boxes since binding does not initialize them
            //   to the bound value.

            comboBoxPlayer2.SelectedIndex = 0;
            comboBoxPlayer3.SelectedIndex = 1;
            comboBoxPlayer4.SelectedIndex = 2;
        }
        catch (FileNotFoundException)
        {
            DialogBoxes.ErrorDialog("The data file for Euchre is missing.  Please reinstall the application.",
                this, "Data File Missing");
        }
    }

    /// <summary>
    /// Sets the image on the control for the user avatar and hides the help label.
    /// </summary>
    private void SetupUserAvatarImage()
    {
        // Load the image for the avatar.

        avatarUserControl.SetAvatar(UserAvatarNumber);
        ClickHereLabelVisibility = Visibility.Collapsed;
        UserAvatarVisibility = Visibility.Visible;
    }

    /// <summary>
    /// Checks that the user added their name and chose an avatar.
    /// </summary>
    /// <returns>True to indicate the required information has been entered.</returns>
    private bool UserEnteredAllInformation()
    {
        var message = string.Empty;

        if (PlayerName == string.Empty)
        {
            message = "Player's name must be entered.";
        }
        else if (UserAvatarNumber == 0)
        {
            message = "An avatar must be selected.";
        }

        if (message != string.Empty)
        {
            DialogBoxes.ErrorDialog(message, this);
        }

        return message == string.Empty;
    }

    private void ChooseAvatarButton_Click(object sender, RoutedEventArgs e)
    {
        // Display all the avatar images to the user to select one.

        ChooseAvatarWindow chooseAvatarWindow = new()
        {
            Owner = this,
        };
        if (chooseAvatarWindow.ShowDialog() == true)
        {
            UserAvatarNumber = chooseAvatarWindow.AvatarNumber;
            SetupUserAvatarImage();
        }
    }

    private void ButtonStartGame_Click(object sender, RoutedEventArgs e)
    {
        if (UserEnteredAllInformation())
        {
            DialogResult = true;
            Close();
        }
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
