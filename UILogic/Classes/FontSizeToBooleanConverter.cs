using System.Globalization;
using System.Windows.Data;

namespace Euchre.UILogic.Classes;

public class FontSizeToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string fontSize && value is string currentSize)
        {
            return currentSize == fontSize;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool isChecked && 
            isChecked && parameter is string fontSize ? fontSize : Binding.DoNothing;
    }
}
