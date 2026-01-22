using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;
using Euchre.UILogic;
using Euchre.UILogic.Classes;
using System.Windows;
using System.Windows.Controls;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for ChooseDiscardWindow.xaml
/// </summary>
public partial class ChooseDiscardWindow : Window
{
    public ChooseDiscardWindow(List<Card> cards, Card kitty)
    {
        InitializeComponent();
        KittyImage = new Image()
        {
            Source = UIHelpers.GenerateCardImageSource(kitty, false, false)
        };
        cardDisplayControl.GetDiscard(cards);
        Owner = Application.Current.MainWindow;
        DataContext = this;
    }

    /// <summary>
    /// The image of the card that the user will pick up.
    /// </summary>
    public Image KittyImage { get; private set; } 

    /// <summary>
    /// Gets the card chosen by the user.
    /// </summary>
    public ICard? ChosenCard => cardDisplayControl.ChosenCard;

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        if (ChosenCard != null)
        {
            DialogResult = true;
            Close();
        }
        else
        {
            DialogBoxes.ErrorDialog("No card was chosen. Please choose a card.", this);
        }
    }
}
