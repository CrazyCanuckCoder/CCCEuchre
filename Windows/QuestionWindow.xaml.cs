using System.Windows;

namespace Euchre.Windows;

/// <summary>
/// Interaction logic for QuestionWindow.xaml
/// </summary>
public partial class QuestionWindow : Window
{
    public QuestionWindow(string descriptionText, string titleText = "Question")
    {
        InitializeComponent();
        DescriptionText = descriptionText;
        TitleText = titleText;
        Owner = Application.Current.MainWindow;
        DataContext = this;
    }

    public string DescriptionText { get; set; }

    public string TitleText { get; set; }


    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
