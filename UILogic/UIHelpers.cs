using System.Windows;
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
}
