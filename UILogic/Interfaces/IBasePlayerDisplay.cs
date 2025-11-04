using Euchre.Logic.Components;
using Euchre.Logic.Interfaces;
using Euchre.UILogic.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Euchre.UILogic.Interfaces;

public interface IBasePlayerDisplay
{
    PlayerDisplay ActivePlayer { get; set; }
    Visibility DealerIconVisibility { get; set; }
    Visibility SuitIconVisibility { get; set; }
    Image SuitImage { get; set; }
    ObservableCollection<Visibility> TrickIconsVisibility { get; set; }
    Brush TricksColour { get; }

    void ClearTrumpSuit();
    void InitializeComponent();
    void SetActivePlayer(IPlayer activePlayer, int avatarNumber);
    void SetDealerIconVisibility(bool isVisible);
    void SetTrumpSuit(Suit trump);
    void UpdateNumberOfTricks(int newNumberOfTricks);
}