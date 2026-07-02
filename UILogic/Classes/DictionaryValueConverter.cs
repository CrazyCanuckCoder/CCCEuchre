using System.Globalization;
using System.Windows.Data;

namespace Euchre.UILogic.Classes;

public class DictionaryValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string fontSize && value is Dictionary<string, int> fontSizes)
        {
            return fontSizes.TryGetValue(fontSize, out var size) ? size : Binding.DoNothing;
        }

        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
