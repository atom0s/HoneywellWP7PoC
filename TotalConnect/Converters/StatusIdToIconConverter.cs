using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace TotalConnect.Converters
{
    using System.Globalization;

    public class StatusIdToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0:
                    return new Uri("../Assets/cell_status_zone_supervision_cleared.png", UriKind.Relative);
                case 1:
                    return new Uri("../Assets/cell_status_zone_bypass.png", UriKind.Relative);
                case 2:
                    return new Uri("../Assets/cell_status_zone_fault.png", UriKind.Relative);
                case 8:
                    return new Uri("../Assets/cell_status_zone_trouble.png", UriKind.Relative);
                case 16:
                    return new Uri("../Assets/cell_status_zone_tamper.png", UriKind.Relative);
                case 32:
                    return new Uri("../Assets/cell_status_zone_supervision.png", UriKind.Relative);
            }

            return new Uri("../Assets/cell_status_zone_supervision.png", UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
