using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace YKW.Core.Events
{
    public enum CollectionChangeKind { Added, Removed, Replaced, Reset }

    public sealed class CollectionChangedEvent<TItem>
    {
        public object Source { get; private set; }
        public TItem Item { get; private set; }
        public TItem PreviousItem { get; private set; }
        public int Index { get; private set; }
        public CollectionChangeKind Kind { get; private set; }

        public CollectionChangedEvent(object source, TItem item, TItem previousItem, int index,
            CollectionChangeKind kind)
        {
            Source = source;
            Item = item;
            PreviousItem = previousItem;
            Index = index;
            Kind = kind;
        }
    }

    /// <summary>A small observable collection for inventories and other owned lists.</summary>
    public sealed class ObservableList<TItem> : Collection<TItem>
    {
        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            EventBus.Publish(new CollectionChangedEvent<TItem>(this, item, default(TItem), index,
                CollectionChangeKind.Added));
        }

        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            EventBus.Publish(new CollectionChangedEvent<TItem>(this, default(TItem), item, index,
                CollectionChangeKind.Removed));
        }

        protected override void SetItem(int index, TItem item)
        {
            TItem previous = this[index];
            if (EqualityComparer<TItem>.Default.Equals(previous, item)) return;
            base.SetItem(index, item);
            EventBus.Publish(new CollectionChangedEvent<TItem>(this, item, previous, index,
                CollectionChangeKind.Replaced));
        }

        protected override void ClearItems()
        {
            if (Count == 0) return;
            base.ClearItems();
            EventBus.Publish(new CollectionChangedEvent<TItem>(this, default(TItem), default(TItem), -1,
                CollectionChangeKind.Reset));
        }
    }
}
