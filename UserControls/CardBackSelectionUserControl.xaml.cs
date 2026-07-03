using CrazyCanuckCoder.Library.Common;
using Euchre.Logic.Helpers;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Euchre.UserControls;

/// <summary>
/// Interaction logic for CardBackSelectionUserControl.xaml
/// </summary>
public partial class CardBackSelectionUserControl : UserControl
{
    public CardBackSelectionUserControl()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// Contains the total number of card backs the user can select from.
    /// </summary>
    public ObservableCollection<CardBack> CardBacks { get; set; } = [];

    /// <summary>
    /// The file name of the card back currently selected by the user or null if no card back is selected.
    /// </summary>
    public string? SelectedCardBack
    {
        get => (  from CardBack cardBack in CardBacks
                 where cardBack.IsSelected
                select cardBack.CardFileName)
               .FirstOrNull();
    }

    /// <summary>
    /// Retrieves all the card backs available and sets the currently selected card back based on the values
    /// passed in the game settings parameter.
    /// </summary>
    /// <param name="gameSettings"></param>
    public void SetupControl()
    {
        LoadCardBacks();
        SetSelectedCardBack(GameSettingsManager.Instance.SelectedCardBack);
    }

    /// <summary>
    /// Gets all the file names of the card backs that the user can select from, ignoring the card back 
    /// images that are rotations of them.
    /// </summary>
    private void LoadCardBacks()
    {
        // Clear the collection of card back information in case this isn't the first execution of this 
        //  method.

        CardBacks.Clear();

        var resourceManager = new ResourceManager("Euchre.g", Assembly.GetExecutingAssembly());
        var resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        if (resources != null)
        {
            List<string> cardBackList = [];
            var folderName = "images/cardbacks/";
            foreach (var res in resources)
            {
                var resourceName = ((DictionaryEntry)res).Key.ToString();
                if (resourceName != null && resourceName.StartsWith(folderName) &&
                    !resourceName.Contains("rotated") &&
                    !resourceName.Contains("disabled"))
                {
                    cardBackList.Add(HttpUtility.UrlDecode(resourceName[folderName.Length..]));
                }
            }

            // Sort the card back list before adding it to the image collection.

            cardBackList.OrderBy(i => i)
                        .ToList()
                        .ForEach(i => CardBacks.Add(SetCardBackInformation(i)));
        }
    }

    /// <summary>
    /// Sets the currently selected card back based on file name parameter.
    /// </summary>
    /// <param name="selectedCardBackName">The name of the card back the user had selected previously.</param>
    private void SetSelectedCardBack(string selectedCardBackName)
    {
        var selectedCardBack = (  from CardBack cardBack in CardBacks
                                 where cardBack.CardFileName.Equals(selectedCardBackName, 
                                                                    StringComparison.OrdinalIgnoreCase)
                                  select cardBack)
                               .FirstOrNull();
        selectedCardBack?.IsSelected = true;
    }

    /// <summary>
    /// Loads the image for a specified card back and its description.
    /// </summary>
    /// <param name="CardBackFile">The file name containing the image for the card back.</param>
    public CardBack SetCardBackInformation(string CardBackFile)
    {
        // Set the bitmap as the source for the image.

        var cardBack = new CardBack(CardBackFile, false)
        {
            ImageData = new Image()
            {
                Source = UIHelpers.GenerateImageSource(
                    $"pack://application:,,,/Euchre;component/images/cardbacks/{CardBackFile}")
            },
            CardVisibility = Visibility.Visible,

            // Set the card back's description based on its filename.

            ImageName = CardBackFile[..CardBackFile.IndexOf("card back")].Capitalize()
        };

        return cardBack;
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        // Uncheck the toggle buttons on the control except for the one just checked by the user.

        if (sender is ToggleButton currentButton)
        {
            this.UncheckOtherToggleButtons(currentButton);
        }
    }
}
