using System;
using System.Globalization;
using System.Windows.Data;

namespace Euchre.UILogic.Classes;

/// <summary>
/// Converts an enumeration value to a boolean value.
/// </summary>
public class EnumBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals(true) == true ? parameter : Binding.DoNothing;
    }
}
