using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Euchre.UILogic;

/// <summary>
/// Extension methods used for controls in the user interface.
/// </summary>
public static class UIExtensions
{
    /// <summary>
    /// Gets all the children of a WPF based control.
    /// </summary>
    /// <param name="parent">A reference to the control to find its children.</param>
    /// <param name="recurse">True to recurse through all levels of the control.</param>
    /// <returns>An IEnumerable containing the controls of the parent control.</returns>
    public static IEnumerable<Visual> GetChildren(this Visual parent, bool recurse = true)
    {
        if (parent != null)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int childIdx = 0; childIdx < childCount; childIdx++)
            {
                // Retrieve child visual at specified index value.

                if (VisualTreeHelper.GetChild(parent, childIdx) is Visual child)
                {
                    yield return child;

                    if (recurse)
                    {
                        foreach (var grandChild in child.GetChildren(true))
                        {
                            yield return grandChild;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Recursively retrieves the toggle buttons on a WPF based control.
    /// </summary>
    /// <returns>An IEnumerable containing the toggle buttons on the parent control.</returns>
    public static IEnumerable<ToggleButton> GetToggleButtons(this Visual parentControl)
    {
        // Check boxes can report as ToggleButtons, so ignore them.

        return from Visual child in parentControl.GetChildren()
               where child is not CheckBox
                  && child is ToggleButton
               select (ToggleButton)child;
    }

    /// <summary>
    /// Sets the toggle buttons unchecked on a visual control except for a specified toggle button.
    /// </summary>
    /// <param name="parentControl">The visual control containing the toggle buttons.</param>
    /// <param name="currentButton">The toggle button that is supposed to be checked.</param>
    public static void UncheckOtherToggleButtons(this Visual parentControl, ToggleButton currentButton)
    {
        foreach (var childButton in parentControl.GetToggleButtons())
        {
            if (!childButton.Equals(currentButton))
            {
                childButton.IsChecked = false;
            }
        }
    }
}
