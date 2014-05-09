using System;

namespace TotalConnect.Converters
{
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class StatusIdToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            return (int)value == 0 ? new SolidColorBrush(Color.FromArgb(0x33, 0, 0, 0)) : new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
