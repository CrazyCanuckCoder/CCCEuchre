using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class NoAceNoFaceNoTrumpDeclaredEventArgs
{
    public IPlayer Player { get; set; }

    public NoAceNoFaceNoTrumpDeclaredEventArgs(IPlayer player)
    {
        Player = player;
    }
}