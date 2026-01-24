using System.IO;
using System.Windows;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for HelpAboutWindow.xaml
/// </summary>
public partial class HelpAboutWindow : Window
{
    public HelpAboutWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadTextFiles();
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for AboutText.  This enables animation, styling, 
    /// binding, etc...
    /// </summary>
    public static readonly DependencyProperty AboutTextProperty =
        DependencyProperty.Register(nameof(AboutText), typeof(string), typeof(HelpAboutWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Using a DependencyProperty as the backing store for LicenseText.  This enables animation, styling, 
    /// binding, etc...
    /// </summary>
    public static readonly DependencyProperty LicenseTextProperty =
        DependencyProperty.Register(nameof(LicenseText), typeof(string), typeof(HelpAboutWindow),
            new PropertyMetadata(string.Empty));

    /// <summary>
    /// Using a DependencyProperty as the backing store for HowToPlayText.  This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty HowToPlayTextProperty =
        DependencyProperty.Register(nameof(HowToPlayText), typeof(string), typeof(HelpAboutWindow),
            new PropertyMetadata(string.Empty));


    /// <summary>
    /// Contains the text about the Solo application.
    /// </summary>
    public string AboutText
    {
        get => (string)GetValue(AboutTextProperty);
        set => SetValue(AboutTextProperty, value);
    }

    /// <summary>
    /// Contains the information about Solo's license.
    /// </summary>
    public string LicenseText
    {
        get => (string)GetValue(LicenseTextProperty);
        set => SetValue(LicenseTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the instructional text displayed to users explaining how to play the game.
    /// </summary>
    public string HowToPlayText
    {
        get => (string)GetValue(HowToPlayTextProperty);
        set => SetValue(HowToPlayTextProperty, value);
    }

    /// <summary>
    /// Retrieves the text for the AboutText and LicenseText properties from their files.
    /// </summary>
    private void LoadTextFiles()
    {
        AboutText = File.ReadAllText("About.txt");
        LicenseText = File.ReadAllText("License.txt");
        HowToPlayText = File.ReadAllText("HowToPlay.md");
    }

    private void ButtonSelect_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
