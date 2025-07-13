using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ACPF.UI.Converters
{
    /// <summary>
    /// Conversor que retorna uma cor do texto baseada em um valor booleano
    /// </summary>
    public class BooleanToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(51, 51, 51));
            }

            return new SolidColorBrush(Color.FromRgb(51, 51, 51));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 