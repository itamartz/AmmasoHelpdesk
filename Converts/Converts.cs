using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace AmmasoConverts
{
    public class StringToEnable : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = value as string;
            return !String.IsNullOrEmpty(str);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                if ((bool)value)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            else
                return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToColorBackground : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string colorString = (string)value;

            if (colorString.ToLower().Contains("cyan"))
                return new SolidColorBrush(Colors.Cyan);
            else if (colorString.ToLower().Contains("yellow"))
                return new SolidColorBrush(Colors.Yellow);
            else if (colorString.ToLower().Contains("magenta"))
                return new SolidColorBrush(Colors.Magenta);
            else if (colorString.ToLower().Contains("black"))
                return new SolidColorBrush(Colors.Black);


            else
                return Colors.Green;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class TonerToString : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string colorString = (string)value;

            if (colorString.ToLower().Contains("cyan"))
                return "Cyan";
            else if (colorString.ToLower().Contains("yellow"))
                return "Yellow";
            else if (colorString.ToLower().Contains("magenta"))
                return "Magenta";
            else if (colorString.ToLower().Contains("black"))
                return "Black";


            else
                return "Green";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class StringToColorForeground : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string colorString = (string)value;

            if (colorString.ToLower().Contains("cyan"))
                return new SolidColorBrush(Colors.Black);
            else if (colorString.ToLower().Contains("yellow"))
                return new SolidColorBrush(Colors.Black);
            else if (colorString.ToLower().Contains("magenta"))
                return new SolidColorBrush(Colors.Black);
            else if (colorString.ToLower().Contains("black"))
                return new SolidColorBrush(Colors.White);


            else
                return Colors.Green;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToVisibility : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 0)
            {

                return Visibility.Visible;
            }
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PrincipalToString : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullOrEmptyToVisibility : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string str = (string)value;
            if( string.IsNullOrEmpty(str))
            {
                return Visibility.Visible;
               
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
