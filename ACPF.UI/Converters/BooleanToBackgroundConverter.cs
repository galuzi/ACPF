using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ACPF.UI.Converters
{
    /// <summary>
    /// Conversor que retorna uma cor de fundo baseada em um valor booleano
    /// </summary>
    public class BooleanToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? new SolidColorBrush(Color.FromRgb(0, 122, 204)) : new SolidColorBrush(Colors.Transparent);
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 