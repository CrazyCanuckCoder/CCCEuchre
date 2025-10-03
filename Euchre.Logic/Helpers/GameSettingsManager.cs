using Euchre.Logic.Data;
using System.IO;

namespace Euchre.Logic.Helpers;

/// <summary>
/// A Singleton class to access the data in the strongly type dataset that contains the settings for this
/// application.
/// </summary>
public sealed class GameSettingsManager
{
    /// <summary>
    /// The name of the file where game data is saved and loaded from.
    /// </summary>
    internal const string FILE_NAME = @"Data\GameSettings.xml";

    /// <summary>
    /// The local instance of the GameSettingsManager.
    /// </summary>
    private static readonly Lazy<GameSettingsManager> _lazy = new(() => new GameSettingsManager());

    /// <summary>
    /// The constructor used by the lazy initializer.
    /// </summary>
    private GameSettingsManager() 
    {
        _settingsDS = new GameSettingsDS();
        LoadDataSetFromXMLFile();
    }

    /// <summary>
    /// Contains the game's settings in a strongly typed dataset.
    /// </summary>
    private readonly GameSettingsDS _settingsDS;

    /// <summary>
    /// A reference to the instance of the GameSettingsManager.
    /// </summary>
    public static GameSettingsManager Instance => _lazy.Value;

    /// <summary>
    /// True to indicate the last card in the user's hand should be played automatically.
    /// </summary>
    public bool PlayLastCardInHand { get; set; }

    /// <summary>
    /// The file name of the selected card back to display in the UI.
    /// </summary>
    public string SelectedCardBack { get; set; } = string.Empty;

    /// <summary>
    /// True to indicate the standard menu should be display and false to display the icon based menu.
    /// </summary>
    public bool UseStandardMenu { get; set; }

    /// <summary>
    /// Reads the current XML file and loads the information into the GameDataSet property.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    public void LoadDataSetFromXMLFile()
    {
        if (File.Exists(FILE_NAME))
        {
            _settingsDS.ReadXml(FILE_NAME);
            var settingsRow = _settingsDS.Settings.First();
            PlayLastCardInHand = settingsRow.PlayLastCardInHumansHand;
            SelectedCardBack = settingsRow.SelectedCardBackFile;
            UseStandardMenu = settingsRow.UseStandardMenu;
        }
        else
        {
            throw new FileNotFoundException(FILE_NAME);
        }
    }

    /// <summary>
    /// Saves the information in the GameDataSet property to the current XML file.
    /// </summary>
    /// <returns>True to indicate the file was saved successfully.</returns>
    public bool SaveDataSetToXMLFile()
    {
        if (File.Exists(FILE_NAME))
        {
            File.Delete(FILE_NAME);
        }

        var settingsRow = _settingsDS.Settings.First();
        settingsRow.PlayLastCardInHumansHand = PlayLastCardInHand;
        settingsRow.SelectedCardBackFile = SelectedCardBack;
        settingsRow.UseStandardMenu = UseStandardMenu;
        _settingsDS.AcceptChanges();
        _settingsDS.WriteXml(FILE_NAME);

        return true;
    }

    /// <summary>
    /// Retrieves the information for the automated players that are used in the game.
    /// </summary>
    /// <returns>An IEnumerable with the automated players information.</returns>
    public IEnumerable<AutomatedPlayerAvatar> GetAutomatedPlayers()
    {
        return   from player in _settingsDS.AutoPlayers
               select new AutomatedPlayerAvatar()
               {
                   PlayerName = player.Name,
                   AvatarNumber = player.AvatarID
               };
    }

}
