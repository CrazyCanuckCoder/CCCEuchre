using System.Windows;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for InformationWindow.xaml
/// </summary>
public partial class InformationWindow : Window
{
    public InformationWindow(string descriptionText, string titleText = "Information")
    {
        InitializeComponent();
        DescriptionText = descriptionText;
        TitleText = titleText;
        DataContext = this;
    }

    public string DescriptionText { get; set; }

    public string TitleText { get; set; }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
