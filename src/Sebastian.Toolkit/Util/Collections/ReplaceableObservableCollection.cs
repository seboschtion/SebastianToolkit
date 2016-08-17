using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sebastian.Toolkit.Util.Collections
{
    public class ReplaceableObservableCollection<TItem> : ObservableCollection<TItem> where TItem : IComparable<TItem>
    {
        protected readonly IEqualityComparer<TItem> EqualityComparer;

        public ReplaceableObservableCollection(IEqualityComparer<TItem> equalityComparer)
        {
            EqualityComparer = equalityComparer;
        }

        public void ReplaceItemsWithList(IList<TItem> list)
        {
            var oldItems = GetOldItems(list);
            foreach (var oldItem in oldItems)
            {
                Remove(oldItem);
            }

            var newItems = GetNewItems(list);
            foreach (var newItem in newItems)
            {
                var index = GetInsertIndex(newItem);
                InsertItem(index, newItem);
            }
        }

        private IEnumerable<TItem> GetNewItems(IList<TItem> newList)
        {
            if (Count == newList.Count)
            {
                var newItems = new List<TItem>();
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].CompareTo(newList[i]) != 0)
                    {
                        newItems.Add(newList[i]);
                    }
                }
                return newItems;
            }

            return newList.Where(item => !this.Contains(item, EqualityComparer)).ToList();
        }

        private IEnumerable<TItem> GetOldItems(IList<TItem> oldList)
        {
            if (Count == oldList.Count)
            {
                var oldItems = new List<TItem>();
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].CompareTo(oldList[i]) != 0)
                    {
                        oldItems.Add(this[i]);
                    }
                }
                return oldItems;
            }

            return this.Where(item => !oldList.Contains(item, EqualityComparer)).ToList();
        }

        private int GetInsertIndex(TItem newItem)
        {
            foreach (var item in this)
            {
                if (item.CompareTo(newItem) <= 0)
                {
                    return IndexOf(item);
                }
            }
            return Count;
        }
    }
}
