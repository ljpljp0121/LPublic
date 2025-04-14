using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Game.RunTime
{
    public class Pool
    {
        private readonly Type objectType;
        private readonly int maxCapacity;
        private int numItems;
        private readonly ConcurrentQueue<object> items = new();
        private object fastItem;

        public Pool(Type objectType, int maxCapacity)
        {
            this.objectType = objectType;
            this.maxCapacity = maxCapacity;
        }

        public object Get()
        {
            object item = fastItem;
            if (item == null || Interlocked.CompareExchange(ref fastItem, null, item) != null)
            {
                if (items.TryDequeue(out item))
                {
                    Interlocked.Decrement(ref numItems);
                }
                else
                {
                    item = Activator.CreateInstance(objectType);
                }
            }
            return item;
        }

        public void Return(object obj)
        {
            if (fastItem != null || Interlocked.CompareExchange(ref fastItem, obj, null) != null)
            {
                if (Interlocked.Increment(ref numItems) <= maxCapacity)
                {
                    items.Enqueue(obj);
                    return;
                }

                Interlocked.Decrement(ref numItems);
            }
        }
    }
}