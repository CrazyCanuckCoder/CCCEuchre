namespace Euchre.Logic.EventArgs;

public class PromptToChooseCardsForPlayersEventArgs : System.EventArgs
{
    /// <summary>
    /// Set to true to indicate the cards should be chosen for the players.
    /// </summary>
    /// <remarks>
    /// Associated event is only fired during testing.
    /// </remarks>
    public bool ChooseCardsForPlayers { get; set; }
}