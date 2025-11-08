using System;
using System.Globalization;
using System.Windows.Data;

namespace VegAndFrut.Converters
{
    [ValueConversion(typeof(decimal), typeof(string))]
    public class DecimalToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, что значение — это decimal
            if (value is decimal decimalValue)
            {
                // Форматируем как F2 (два знака после запятой)
                return decimalValue.ToString("F2", culture);
            }

            if (value is object objectValue)
            {
                // Форматируем как F2 (два знака после запятой)
                return ((decimal)objectValue).ToString("F2", culture);
            }
                

            // Если значение null или не decimal — возвращаем пустую строку
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Парсим строку обратно в decimal
            if (decimal.TryParse(value?.ToString(), NumberStyles.Float, culture, out decimal result))
            {
                return result;
            }

            // Если не удалось распарсить — возвращаем 0
            return 0m;
        }
    }
}