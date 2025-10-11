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
}
