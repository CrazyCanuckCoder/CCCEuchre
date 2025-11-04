using Euchre.Logic.Interfaces;

namespace Euchre.Logic.EventArgs;

public class UserChoseCardEventArgs : System.EventArgs
{
    public ICard ChosenCard { get; set; }

    public UserChoseCardEventArgs(ICard chosenCard)
    {
        ChosenCard = chosenCard;
    }
}