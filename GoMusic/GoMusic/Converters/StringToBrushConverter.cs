using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GoMusic.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string color_code = value as string;
            Color color = Color.FromArgb(255, 255, 56, 56);

            switch (color_code)
            {
                case "Red": color = Color.FromArgb(255, 255, 56, 56);
                    break;
                case "Blue": color = Color.FromArgb(255, 49, 179, 255);
                    break;
                case "Pink": color = Color.FromArgb(255, 232, 84, 127);
                    break;
                case "Green": color = Color.FromArgb(255, 91, 255, 0);
                    break;
                case "Magneta": color = Color.FromArgb(255, 216, 0, 115);
                    break;
                case "Olive": color = Color.FromArgb(255, 109, 135, 100);
                    break;
                case "Brown": color = Color.FromArgb(255, 130, 90, 44);
                    break;
                case "Orange": color = Color.FromArgb(255, 250, 104, 0);
                    break;
                case "Yellow": color = Color.FromArgb(255, 227, 200, 0);
                    break;
            };
            return new SolidColorBrush(color);

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
