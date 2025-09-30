using Euchre.Logic.Interfaces;

namespace Euchre.UILogic.Classes;

public class PlayerDisplay
{
    public IPlayer Player { get; set; }

    public int TotalTricks { get; set; }

    public string TotalTricksDisplay => $"Tricks: {TotalTricks}";

    public int AvatarNumber { get; set; }

}
