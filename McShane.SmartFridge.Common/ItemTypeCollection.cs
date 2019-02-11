using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace McShane.SmartFridge.Common
{
    /// <summary>
    /// Contains a list of <see cref="Item"/> of the same type along with collection
    /// related information to tell if it is forgotten and what the fill factor is
    /// </summary>
    public class ItemTypeCollection
    {
        //List of the items in the collection
        internal Dictionary<Guid, Item> InternalItems;

        /// <param name="type">Item type of the collection</param>
        public ItemTypeCollection(long type)
        {
            InternalItems = new Dictionary<Guid, Item>();
            ItemType = type;
        }

        public long ItemType { get; }

        /// <summary>
        /// Returns the average fill factor for the type excluding items
        /// with a fill factor less then 0.0000001.  If all items have a
        /// fill factor less then 0.0000001 then 0 is returned;
        /// </summary>
        public double FillFactor
        {
            get
            {
                if (InternalItems.Values.Any(i => i.FillFactor > 0.0000001))
                {
                    return InternalItems.Values.Where(i => i.FillFactor > 0.0000001).Average(i => i.FillFactor);
                }
                return 0;
            }
        }

        /// <summary>
        /// Should the collection be forgotten by the manager
        /// </summary>
        public bool IsForgotten {get; set; }

        /// <summary>
        /// List of items in the collection
        /// </summary>
        public IReadOnlyCollection<Item> Items => new ReadOnlyCollection<Item>(InternalItems.Values.ToList());

        /// <summary>
        /// Add an item to the collection
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="Item.Type"/> of <paramref name="item"/> does not match <see cref="ItemType"/></exception>
        public void Add(Item item)
        {
            if (item.Type != ItemType)
            {
                throw new ArgumentOutOfRangeException(nameof(item), "Item must be the same type as the collection");
            }
            InternalItems.Add(item.Id, item);
        }

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="id">Id of the <see cref="Item"/></param>
        public void Remove(Guid id)
        {
            InternalItems.Remove(id);
        }
    }
}