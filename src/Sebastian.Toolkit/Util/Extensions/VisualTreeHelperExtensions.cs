using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Sebastian.Toolkit.Util.Extensions
{
    public static class VisualTreeHelperExtensions
    {
        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = child as T ?? child.GetChildOfType<T>();
                if (result != null)
                {
                    return result;
                }
            }
            return default(T);
        }
    }
}
