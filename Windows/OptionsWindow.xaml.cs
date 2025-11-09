using Euchre.Logic.Helpers;
using System.Windows;

namespace Euchre.Windows;
/// <summary>
/// Interaction logic for OptionsWindow.xaml
/// </summary>
public partial class OptionsWindow : Window
{
    public OptionsWindow()
    {
        InitializeComponent();
        GameSettings = GameSettingsManager.Instance;
        SetupForm();
        DataContext = this;
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for GameSettings.
    /// </summary>
    public static readonly DependencyProperty GameSettingsProperty =
        DependencyProperty.Register(nameof(GameSettings), typeof(GameSettingsManager), typeof(OptionsWindow),
            new PropertyMetadata(null));

    /// <summary>
    /// A data bindable instance of the GameSettingsManager class.
    /// </summary>
    public GameSettingsManager GameSettings
    {
        get => (GameSettingsManager)GetValue(GameSettingsProperty); 
        set => SetValue(GameSettingsProperty, value);
    }



    /// <summary>
    /// Calls the method to load the current settings then sets up the control that allows the user to select
    /// the card back image.
    /// </summary>
    private void SetupForm()
    {
        cardBackSelectionUserControl.SetupControl();
    }

    /// <summary>
    /// Checks that the user has selected a card back image and returns true if one is selected.
    /// </summary>
    /// <returns>True to indicate the settings on the form are valid.</returns>
    private bool VerifySettings()
    {
        return cardBackSelectionUserControl.SelectedCardBack != null;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (VerifySettings())
        {
            GameSettingsManager.Instance.SelectedCardBack = cardBackSelectionUserControl?.SelectedCardBack ??
                                                            GameSettingsManager.Instance.SelectedCardBack;
            GameSettingsManager.Instance.SaveDataSetToXMLFile();
            Close();
        }
        else
        {
            MessageBox.Show(this, "A card back must be selected.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
