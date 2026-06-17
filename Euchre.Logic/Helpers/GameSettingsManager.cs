using System.IO;
using System.Xml.Linq;
using Euchre.Logic.Data;

namespace Euchre.Logic.Helpers;

/// <summary>
/// A Singleton class to access the data in the strongly type dataset that contains the settings for this
/// application.
/// </summary>
public sealed class GameSettingsManager
{
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
    /// The name and path of the file where game data is saved and loaded from.
    /// </summary>
    internal string _configFilePath = string.Empty;

    /// <summary>
    /// A reference to the instance of the GameSettingsManager.
    /// </summary>
    public static GameSettingsManager Instance => _lazy.Value;

    /// <summary>
    /// True to indicate the last card in the user's hand should be played automatically.
    /// </summary>
    public bool PlayLastCardInHand { get; set; } = true;

    /// <summary>
    /// The file name of the selected card back to display in the UI.
    /// </summary>
    public string SelectedCardBack { get; set; } = "Grid Blue Card Back.png";

    /// <summary>
    /// True to indicate the standard menu should be display and false to display the icon based menu.
    /// </summary>
    public bool UseStandardMenu { get; set; } = false;

    /// <summary>
    /// True to indicate the Canadian Lone rule (if you order up your partner, you have to go alone) is in
    /// effect.
    /// </summary>
    public bool CanadianLonerRule { get; set; } = true;

    /// <summary>
    /// True to indicate the player must have a card in a suit to call it as trump.
    /// </summary>
    public bool MustHaveSuitToCall { get; set; } = true;

    /// <summary>
    /// True to indicate the player must call suit if they are the dealer and no trump has been called in the
    /// second round.
    /// </summary>
    public bool StickTheDealer { get; set; } = false;

    /// <summary>
    /// True to indicate the rule called No Ace, No Face, No Trump is active.  The rule allows a player to 
    /// declare their hand has no Aces, no cards above the 10 card and contains no trump.  When declared, the
    /// hand is stopped and then redealt.
    /// </summary>
    public bool NoAceNoFaceNoTrumpRule { get; set; } = true;

    /// <summary>
    /// True to indicate the player can choose to go under when ordering up the kitty card, if they have 3 or
    /// more cards under the rank of Jack.
    /// </summary>
    public bool CanGoUnder { get; set; } = false;

    /// <summary>
    /// Reads the current XML file and loads the information into the GameDataSet property.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    public void LoadDataSetFromXMLFile()
    {
        SetConfigFilePath();
        if (File.Exists(_configFilePath))
        {
            _settingsDS.ReadXml(_configFilePath);
            var settingsRow = _settingsDS.Settings.First();
            PlayLastCardInHand = settingsRow.PlayLastCardInHumansHand;
            SelectedCardBack = settingsRow.SelectedCardBackFile;
            UseStandardMenu = settingsRow.UseStandardMenu;
            CanadianLonerRule = settingsRow.UseCanadianLonerRule;
            MustHaveSuitToCall = settingsRow.MustHaveSuitToCall;
            StickTheDealer = settingsRow.StickTheDealer;
            NoAceNoFaceNoTrumpRule = settingsRow.UseNoAceNoFaceNoTrumpRule;
            CanGoUnder = settingsRow.CanGoUnder;
        }
        else
        {
            CreateDefaultAutomatedPlayers();
            SaveDataSetToXMLFile();
        }
    }

    /// <summary>
    /// Creates the default automated players in the dataset.  This is used when the application is first run
    /// and there are no existing settings to load from an XML file.  The default players are added and then 
    /// saved to a new XML file.
    /// </summary>
    private void CreateDefaultAutomatedPlayers()
    {
        _settingsDS.AutoPlayers.AddAutoPlayersRow("Monica", 10);
        _settingsDS.AutoPlayers.AddAutoPlayersRow("Simon", 1);
        _settingsDS.AutoPlayers.AddAutoPlayersRow("Suri", 4);
        _settingsDS.AutoPlayers.AddAutoPlayersRow("Brad", 19);
        _settingsDS.AutoPlayers.AddAutoPlayersRow("Julia", 17);
        _settingsDS.AutoPlayers.AddAutoPlayersRow("Tim", 24);
        _settingsDS.AutoPlayers.AcceptChanges();
    }

    /// <summary>
    /// Saves the information in the GameDataSet property to the current XML file.
    /// </summary>
    /// <returns>True to indicate the file was saved successfully.</returns>
    public bool SaveDataSetToXMLFile()
    {
        if (File.Exists(_configFilePath))
        {
            File.Delete(_configFilePath);
        }

        if (_settingsDS.Settings.Count > 0)
        {
            var settingsRow = _settingsDS.Settings.First();
            settingsRow.PlayLastCardInHumansHand = PlayLastCardInHand;
            settingsRow.SelectedCardBackFile = SelectedCardBack;
            settingsRow.UseStandardMenu = UseStandardMenu;
            settingsRow.UseCanadianLonerRule = CanadianLonerRule;
            settingsRow.MustHaveSuitToCall = MustHaveSuitToCall;
            settingsRow.StickTheDealer = StickTheDealer;
            settingsRow.UseNoAceNoFaceNoTrumpRule = NoAceNoFaceNoTrumpRule;
            settingsRow.CanGoUnder = CanGoUnder;
        }
        else
        {
            _settingsDS.Settings.AddSettingsRow(SelectedCardBack, PlayLastCardInHand, UseStandardMenu, 
                CanadianLonerRule, MustHaveSuitToCall, StickTheDealer, NoAceNoFaceNoTrumpRule, CanGoUnder);
        }

        _settingsDS.AcceptChanges();
        _settingsDS.WriteXml(_configFilePath);

        return true;
    }

    /// <summary>
    /// Retrieves the information for the automated players that are used in the game.
    /// </summary>
    /// <returns>An IEnumerable with the automated players information.</returns>
    public IEnumerable<AutomatedPlayerAvatar> GetAutomatedPlayers()
    {
        return   from playerRow in _settingsDS.AutoPlayers
               select new AutomatedPlayerAvatar()
               {
                   PlayerName = playerRow.Name,
                   AvatarNumber = playerRow.AvatarID
               };
    }

    /// <summary>
    /// Gets the directory where game settings are stored.  If the application directory does not exist in 
    /// the user's AppData folder, it is created. Returns the Data subdirectory path.
    /// </summary>
    /// <returns>The full path to the directory where game settings are stored.</returns>
    public string GetSettingsDirectory()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var settingsDirectory = Path.Combine(appDataPath, AppDomain.CurrentDomain.FriendlyName);

        // Check if app directory exists, if not create it.

        if (!Directory.Exists(settingsDirectory))
        {
            Directory.CreateDirectory(settingsDirectory);
        }

        var dataDirectory = Path.Combine(settingsDirectory, "Data");
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        return dataDirectory;
    }

    /// <summary>
    /// Sets up the config file name and its path.  If the application directory does not exist in the user's 
    /// AppData folder, it is created. The configuration file is stored in this directory.
    /// </summary>
    private void SetConfigFilePath()
    {
        _configFilePath = Path.Combine(GetSettingsDirectory(), "GameSettings.xml");
    }

}
