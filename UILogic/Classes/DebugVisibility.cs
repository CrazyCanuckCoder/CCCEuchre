using System.Windows;

namespace Euchre.UILogic.Classes;

/// <summary>
/// Contains visibility properties to show or hide controls in debug and release versions.
/// </summary>
/// 
/// <remarks>
/// Usage:
/// 
///     xmlns:Logic="clr-namespace:Euchre.UILogic.Classes"
///         
///     <MenuItem Header="_Test" x:Name="menuTest" Visibility="{x:Static Logic:DebugVisibility.DebugOnly}">
///     
/// </remarks>
public static class DebugVisibility
{
    /// <summary>
    /// For controls to be shown in the Debug version only.
    /// </summary>
    public static Visibility DebugOnly
    {
#if DEBUG
        get => Visibility.Visible;
#else
        get => Visibility.Collapsed;
#endif
    }

    /// <summary>
    /// For controls to be shown in the Release version only.
    /// </summary>
    public static Visibility ReleaseOnly
    {
#if DEBUG
        get => Visibility.Collapsed;
#else
        get => Visibility.Visible;
#endif
    }
}
