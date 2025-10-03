using CrazyCanuckCoder.Library.WPF;
using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using System.Windows;

namespace Euchre;

public class MainWindowViewModel : ViewModelBase
{
    // The fields used for the properties.

    private bool _continueMenuEnabled;
    private Visibility _iconMenuVisibility = Visibility.Collapsed;
    private Visibility _standardMenuVisibility = Visibility.Visible;

    /// <summary>
    /// Enables the continue game menu option when a game data file exists.
    /// </summary>
    public bool ContinueMenuEnabled
    {
        get => _continueMenuEnabled;
        set 
        { 
            _continueMenuEnabled = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Displays or collapses the regular menu.
    /// </summary>
    public Visibility StandardMenuVisibility
    {
        get => _standardMenuVisibility; 
        set 
        { 
            _standardMenuVisibility = value; 
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Displays or collapses the icon menu.
    /// </summary>
    public Visibility IconMenuVisibility
    {
        get => _iconMenuVisibility;
        set 
        { 
            _iconMenuVisibility = value; 
            OnPropertyChanged();
        }
    }

    private bool _playLastCardInHand;

    /// <summary>
    /// True to indicate the last card in a human player's hand can be automatically played.
    /// </summary>
    public bool PlayLastCardInHand
    {
        get => _playLastCardInHand;
        set 
        { 
            _playLastCardInHand = value; 
            OnPropertyChanged();
        }
    }


    /// <summary>
    /// The reference to the object running the game logic.
    /// </summary>
    public EuchreGame? CurrentGame { get; set; }

    /// <summary>
    /// Sets up some of the properties for this view model class.
    /// </summary>
    internal void Initialize()
    {
        ContinueMenuEnabled = GameStateManager.DataExists();
        IconMenuVisibility = GameSettingsManager.Instance.UseStandardMenu ? 
            Visibility.Collapsed : Visibility.Visible;
        StandardMenuVisibility = GameSettingsManager.Instance.UseStandardMenu ? 
            Visibility.Visible : Visibility.Collapsed;
        PlayLastCardInHand = GameSettingsManager.Instance.PlayLastCardInHand;
    }
}
