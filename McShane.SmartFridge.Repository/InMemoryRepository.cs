using System;
using System.Collections.Generic;
using System.Linq;
using McShane.SmartFridge.Common;

namespace McShane.SmartFridge.Repository
{
    /// <summary>
    /// Memory based non persistent repository
    /// </summary>
    public class InMemoryRepository : IRepository
    {
        // List of items by Id
        internal Dictionary<Guid, Item> Items;

        // List of items by Type
        internal Dictionary<long, ItemTypeCollection> ItemsByType;

        public InMemoryRepository()
        {
            Items = new Dictionary<Guid, Item>();
            ItemsByType = new Dictionary<long, ItemTypeCollection>();
        }

        /// <summary>
        /// Remove an <see cref="Item"/> from the repository
        /// </summary>
        /// <param name="id">The id of the <see cref="Item"/> to remove</param>
        public void Remove(Guid id)
        {
            if (!Items.ContainsKey(id))
            {
                return; //nothing to do if we don't have this item
            }

            //Get the item to remove
            Item itemToRemove = Items[id]; 

            //Remove it from the id collection
            Items.Remove(id);

            //Remove it from the type collection
            ItemsByType[itemToRemove.Type].Remove(id);
        }

        /// <summary>
        /// Add an <see cref="Item"/> to the repository
        /// </summary>
        /// <param name="type">The type of the <see cref="Item"/></param>
        /// <param name="id">The id of the <see cref="Item"/></param>
        /// <param name="name">The name of the <see cref="Item"/></param>
        /// <param name="fillFactor">The fill factor of the <see cref="Item"/></param>
        public void Add(long type, Guid id, string name, double fillFactor)
        {
            Item newItem = new Item(type, id, name, fillFactor);
            Items.Add(id, newItem);
            if (!ItemsByType.ContainsKey(newItem.Type))
            {
                ItemsByType.Add(newItem.Type, new ItemTypeCollection(newItem.Type));
            }
            ItemsByType[newItem.Type].Add(newItem);
        }

        /// <param name="itemType">The type of <see cref="Item"/></param>
        /// <returns>
        /// Average fill factor for the type.
        /// For detailed implementation <see cref="ItemTypeCollection.FillFactor"/>
        /// </returns>
        public double FillFactor(long itemType)
        {
            if (!ItemsByType.ContainsKey(itemType))
            {
                return 0;
            }

            return ItemsByType[itemType].FillFactor;
        }

        /// <summary>
        /// Ignore then given type of <see cref="Item"/>
        /// </summary>
        /// <param name="itemType">Type of <see cref="Item"/> to ignore</param>
        public void Forget(long itemType)
        {
            if (!ItemsByType.ContainsKey(itemType))
            {
                ItemsByType.Add(itemType, new ItemTypeCollection(itemType));
            }

            ItemsByType[itemType].IsForgotten = true;
        }

        /// <summary>
        /// Provides a list of items whose types are less then or equal to the provided fill factor
        /// </summary>
        /// <param name="fillFactor">Fill factor cut-off value</param>
        /// <returns>
        /// An <see cref="object"/> array when each object in the array is another <see cref="object"/>
        /// array containing at least 3 elements. The first element is the <see cref="Item.Type"/>.
        /// The second element is the average fill factor (see <see cref="ItemTypeCollection.FillFactor"/> for details)
        /// Elements 3+ are the actual <see cref="Item"/>
        /// </returns>
        public object[] GetItems(double fillFactor)
        {
            List<object> itemsToReturn = new List<object>();
            
            //Find the item types that are not forgotten and have a fill factor less then or equal to the provided value
            var itemGroups = ItemsByType.Where(i => !i.Value.IsForgotten && i.Value.FillFactor <= fillFactor).OrderBy(i => i.Value.FillFactor);

            //for each of the item types
            foreach (var itemGroup in itemGroups)
            {
                List<object> entry = new List<object>();
                entry.Add(itemGroup.Key); //element 1 - Type
                entry.Add(itemGroup.Value.FillFactor); //element 2 - Fill Factor
                entry.AddRange(itemGroup.Value.Items.OrderBy(i => i.FillFactor)); //elements 3+ - Items (sorted by Fill Factor)
                itemsToReturn.Add(entry.ToArray()); //Create an object[]
            }

            return itemsToReturn.ToArray();
        }
    }
}
