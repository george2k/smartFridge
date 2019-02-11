using System;

namespace McShane.SmartFridge.Common
{
    /// <summary>
    /// This class represents an item in the Smart Fridge
    /// </summary>
    public class Item
    {
        /// <param name="type">Item Type</param>
        /// <param name="id">Item Id</param>
        /// <param name="name">Item Name</param>
        /// <param name="fillFactor">Fill Factor</param>
        /// <exception cref="ArgumentOutOfRangeException">Throw when <paramref name="id"/> equals <see cref="Guid.Empty"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw when <paramref name="fillFactor"/> is less than 0 or greater then 1</exception>
        /// <exception cref="ArgumentOutOfRangeException">Throw when <paramref name="name"/> is <c>null</c> or <see cref="string.Empty"/></exception>
        public Item(long type, Guid id, string name, double fillFactor)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Must provide a non-empty Guid");
            }

            if (fillFactor < 0 || fillFactor > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(fillFactor), fillFactor, "Must be between 0 and 1 inclusively");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name), "Must supply at least one character");
            }

            Id = id;
            Type = type;
            FillFactor = fillFactor;
            Name = name;
        }

        public Guid Id { get; }
        public long Type { get; }
        public double FillFactor { get; }
        public string Name { get; }
    }
}