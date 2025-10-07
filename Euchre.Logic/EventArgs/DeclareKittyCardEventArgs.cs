using Euchre.Logic.Components;

namespace Euchre.Logic.EventArgs;

public class DeclareKittyCardEventArgs
{
    public Card Kitty { get; set; }

    public DeclareKittyCardEventArgs(Card card)
    {
        Kitty = card;
    }
}