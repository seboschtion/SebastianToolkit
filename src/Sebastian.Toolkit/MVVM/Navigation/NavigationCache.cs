using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Sebastian.Toolkit.MVVM.Navigation
{
    internal class NavigationCache
    {
        private readonly Stack<Page> _cache;

        internal NavigationCache(int maxCacheSize)
        {
            MaxCacheSize = maxCacheSize;
            _cache = new Stack<Page>();
        }

        internal int MaxCacheSize { get; set; }
        internal int CacheSize => _cache.Count;
        internal bool HasItems => _cache.Any();

        internal bool Cache(Page page)
        {
            return Cache(page, NavigationCacheMode.Disabled);
        }

        internal bool Cache(Page page, NavigationCacheMode navigationCacheMode)
        {
            if (CacheSize >= MaxCacheSize && navigationCacheMode != NavigationCacheMode.Required)
            {
                return false;
            }
            
            _cache.Push(page);
            return true;
        }

        internal Page GetAndRemoveLast()
        {
            return _cache.Pop();
        }
    }
}