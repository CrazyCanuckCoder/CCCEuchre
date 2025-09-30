using System.Windows;

namespace Euchre.UILogic.Classes;

/// <summary>
/// A class with common dialog boxes for use in the application.
/// </summary>
public static class DialogBoxes
{
    /// <summary>
    /// Shows a simple dialog box to display information text.
    /// </summary>
    /// <param name="message">The information text to display to the user.</param>
    /// <param name="owner">An optional Window that owns the dialog box.</param>
    /// <param name="title">The title text to display on the dialog box.</param>
    /// <returns>A MessageBoxResult containing the button the user clicked.</returns>
    public static MessageBoxResult InformationDialog(string message, Window? owner = null,
        string title = "Test")
    {
        return ShowDialog(message, title, MessageBoxImage.Information, owner);
    }

    /// <summary>
    /// Prompts the user for a yes or no answer to a question.
    /// </summary>
    /// <param name="message">The question to ask the user.</param>
    /// <param name="owner">An optional Window that owns the dialog box.</param>
    /// <param name="title">The title text to display on the dialog box.</param>
    /// <returns>A MessageBoxResult of either Yes or No.</returns>
    public static MessageBoxResult YesOrNoDialog(string message, Window? owner = null, 
        string title = "Question")
    {
        return MessageBox.Show(owner, message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
    }

    /// <summary>
    /// Shows a simple dialog box to display error text.
    /// </summary>
    /// <param name="message">The error text to display to the user.</param>
    /// <param name="owner">An optional Window that owns the dialog box.</param>
    /// <param name="title">The title text to display on the dialog box.</param>
    /// <returns>A MessageBoxResult containing the button the user clicked.</returns>
    public static MessageBoxResult ErrorDialog(string message, Window? owner = null, string title = "Error")
    {
        return ShowDialog(message, title, MessageBoxImage.Error, owner);
    }

    /// <summary>
    /// Shows a simple dialog box to display text to the user.
    /// </summary>
    /// <param name="message">The text to display to the user.</param>
    /// <param name="title">The title text to display on the dialog box.</param>
    /// <param name="messageBoxImage">The image to display as an icon on the dialog box.</param>
    /// <param name="owner">An optional Window that owns the dialog box.</param>
    /// <returns>A MessageBoxResult containing the button the user clicked.</returns>
    public static MessageBoxResult ShowDialog(string message, string title, MessageBoxImage messageBoxImage,
        Window? owner = null)
    {
        return MessageBox.Show(owner, message, title, MessageBoxButton.OK, messageBoxImage);
    }
}
