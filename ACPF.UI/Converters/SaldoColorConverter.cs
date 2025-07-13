using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ACPF.UI.Converters
{
    /// <summary>
    /// Conversor que retorna uma cor baseada no valor do saldo
    /// </summary>
    public class SaldoColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal saldo)
            {
                if (saldo > 0)
                    return new SolidColorBrush(Colors.Green);
                else if (saldo < 0)
                    return new SolidColorBrush(Colors.Red);
                else
                    return new SolidColorBrush(Colors.Black);
            }

            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 