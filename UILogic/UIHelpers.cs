using Euchre.Logic.Components;
using Euchre.Logic.Helpers;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Euchre.UILogic;

/// <summary>
/// A collection of methods to assist the user interface.
/// </summary>
internal static class UIHelpers
{
    /// <summary>
    /// Ensures the UI is updated.
    /// </summary>
    public static void AllowUIToUpdate()
    {
        DispatcherFrame frame = new();

        // DispatcherPriority set to Input, the highest priority.

        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Input,
            new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                Thread.Sleep(20); // Stop all processes to make sure the UI update is perform
                return null;
            }), null);
        Dispatcher.PushFrame(frame);

        // DispatcherPriority set to Input, the highest priority.

        Application.Current?.Dispatcher.Invoke(DispatcherPriority.Input, new Action(delegate { }));
    }

    /// <summary>
    /// Ensures the provided <paramref name="action"/> runs on the UI thread.
    /// </summary>
    /// <param name="action">Action to execute.</param>
    /// <param name="priority">Dispatcher priority to use when marshalling.</param>
    public static void RunOnUIThread(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        ArgumentNullException.ThrowIfNull(action);

        var dispatcher = Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        if (dispatcher.CheckAccess())
        {
            action();
        }
        else
        {
            dispatcher.Invoke(action, priority);
        }
    }

    /// <summary>
    /// Creates the image source based on a specified URI string.
    /// </summary>
    /// <param name="uriSource">The URI based path to the image.</param>
    /// <returns>A BitmapImage containing the image.</returns>
    public static BitmapImage GenerateImageSource(string uriSource)
    {
        BitmapImage source = new();
        source.BeginInit();
        source.UriSource = new(uriSource);
        source.EndInit();

        return source;
    }

    /// <summary>
    /// Creates the image file for a specified card.
    /// </summary>
    /// <param name="card">The card to create the image for.</param>
    /// <param name="isRotated">True if the image should be the rotated version.</param>
    /// <param name="isDisabled">True if the image should be the disabled version.</param>
    /// <returns>A BitmapImage containing the specified card.</returns>
    public static BitmapImage GenerateCardImageSource(Card card, bool isRotated, bool isDisabled)
    {
        string cardFilename = card.ToString().ToLower() + ".png";
        if (isRotated)
        {
            cardFilename = cardFilename.Insert(cardFilename.LastIndexOf('.'), " rotated");
        }
        if (isDisabled)
        {
            cardFilename = cardFilename.Insert(cardFilename.LastIndexOf('.'), " disabled");
        }
        string cardUri = $"pack://application:,,,/Euchre;component/images/cards/{cardFilename}";

        return GenerateImageSource(cardUri);
    }

    /// <summary>
    /// Creates the image file for a specified suit.
    /// </summary>
    /// <param name="suit">The suit to create the image for.</param>
    /// <returns>A BitmapImage containing the specified suit image.</returns>
    public static BitmapImage GenerateSuitImageSource(Suit suit)
    {
        string uriSource = "pack://application:,,,/Euchre;component/images/suits/";
        switch (suit)
        {
            case Suit.Clubs:
                uriSource += "Clubs";
                break;

            case Suit.Diamonds:
                uriSource += "Diamonds";
                break;

            case Suit.Hearts:
                uriSource += "Hearts";
                break;

            case Suit.Spades:
                uriSource += "Spades";
                break;
        }
        return GenerateImageSource(uriSource + ".png");
    }

    /// <summary>
    /// Generates the image for the current card back specified in the game's settings.
    /// </summary>
    /// <param name="isRotated">True if the image should be the rotated version.</param>
    /// <param name="isDisabled">True if the image should be the disabled version.</param>
    /// <returns>A BitmapImage containing the specified card back.</returns>
    public static BitmapImage GenerateCardBackImageSource(bool isRotated, bool isDisabled)
    {
        string cardFilename = GameSettingsManager.Instance.SelectedCardBack;

        if (isRotated)
        {
            cardFilename = cardFilename.Insert(cardFilename.LastIndexOf('.'), " rotated");
        }
        if (isDisabled)
        {
            cardFilename = cardFilename.Insert(cardFilename.LastIndexOf('.'), " disabled");
        }
        string cardbackUri = $"pack://application:,,,/Euchre;component/images/cardbacks/{cardFilename}";

        return GenerateImageSource(cardbackUri);
    }

    /// <summary>
    /// Generates the image for a specified avatar.
    /// </summary>
    /// <param name="avatarNumber">The number corresponding to the avatar.</param>
    /// <returns>A BitmapImage containing the specified avatar.</returns>
    public static BitmapImage GenerateAvatarImageSource(int avatarNumber)
    {
        string filename = $"avatar{avatarNumber}.png";
        return GenerateImageSource($"pack://application:,,,/Euchre;component/images/avatars/{filename}");
    }
}
