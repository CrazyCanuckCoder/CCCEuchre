using System.Globalization;
using System.Windows.Data;

namespace Euchre.UILogic.Classes;

/// <summary>
/// A converter to use on WPF UI Controls that converts a boolean value to any value that is specified in the
/// implementation of the converter.
/// </summary>
/// 
/// <remarks>
/// Original code from https://www.codeproject.com/Tips/592456/Bool-To-Content-Converter.
/// </remarks>
public class BoolToContentConverter : IValueConverter
{
    /// <summary>
    /// Sets default string values for the content properties.
    /// </summary>
    public BoolToContentConverter()
    {
        TrueContent = "True";
        FalseContent = "False";
        NullContent = "No Value";
    }

    /// <summary>
    /// The value to return when the boolean value is true.
    /// </summary>
    public object TrueContent { get; set; }

    /// <summary>
    /// The value to return when the boolean value is false.
    /// </summary>
    public object FalseContent { get; set; }

    /// <summary>
    /// The value to return when the boolean value is missing.
    /// </summary>
    public object NullContent { get; set; }

    /// <summary>
    /// Converts a boolean value to content specified by the properties of this class.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">Not used.</param>
    /// <param name="parameter">Not used.</param>
    /// <param name="culture">Not used.</param>
    /// <returns>The TrueContent, FalseContent, or the NullContent based on the specified value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool? boolValue = value as bool?;
        return boolValue.HasValue ?
             boolValue.Value ? TrueContent : FalseContent :
             NullContent;
    }


    // Not applicable.

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}