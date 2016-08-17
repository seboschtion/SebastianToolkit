using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sebastian.Toolkit.Util.Extensions
{
    public static class ListExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
        {
            var observableCollection = new ObservableCollection<T>();
            foreach (var item in list)
            {
                observableCollection.Add(item);
            }
            return observableCollection;
        }
    }
}
