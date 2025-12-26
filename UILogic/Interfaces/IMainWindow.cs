using Euchre.UserControls;

namespace Euchre.UILogic.Interfaces;

public interface IMainWindow
{
    IMainWindowViewModel ViewModel { get; }

    void InitializeComponent();

    CurrentScoreUserControl CurrentPlayersScoresUserControl { get; }
    CardDisplayUserControl Player3DealtCardsDisplayUserControl { get; }
    HorizontalPlayerDisplayUserControl Player3DisplayUserControl { get; }
    CardDisplayUserControl Player3CardDisplayUserControl { get; }
    CurrentRoundInfoUserControl CurrentRoundInfoUserControl { get; }
    VerticalPlayerDisplayUserControl Player2DisplayUserControl { get; }
    VerticalCardDisplayUserControl Player2CardDisplayUserControl { get; }
    CardDisplayUserControl Player3PlayedCardsDisplayUserControl { get; }
    VerticalCardDisplayUserControl Player2DealtCardsDisplayUserControl { get; }
    CardDisplayUserControl Player2PlayedCardsDisplayUserControl { get; }
    VerticalCardDisplayUserControl Player4DealtCardsDisplayUserControl { get; }
    CardDisplayUserControl Player4PlayedCardsDisplayUserControl { get; }
    CardDisplayUserControl Player1DealtCardsDisplayUserControl { get; }
    CardDisplayUserControl Player1PlayedCardsDisplayUserControl { get; }
    VerticalCardDisplayUserControl Player4CardDisplayUserControl { get; }
    VerticalPlayerDisplayUserControl Player4DisplayUserControl { get; }
    TrumpDisplayUserControl TrumpDisplayUserControl { get; }
    CardDisplayUserControl Player1CardDisplayUserControl { get; }
    HorizontalPlayerDisplayUserControl Player1DisplayUserControl { get; }
    PreviousTricksUserControl PreviousTricksUserControl { get; }
}