using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Sebastian.Toolkit.Converters
{
    public class DebugToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            return isDebug ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
