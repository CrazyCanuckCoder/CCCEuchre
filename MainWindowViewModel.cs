using CrazyCanuckCoder.Library.WPF;
using Euchre.Logic.Components;

namespace Euchre;

internal class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The reference to the object running the game logic.
    /// </summary>
    public EuchreGame? CurrentGame { get; set; }
}
