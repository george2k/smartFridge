namespace DiscoverOrg
{
    /// <summary>
    /// Interface for the Smart Fridge Manager
    /// </summary>
    /// <remarks>
    /// Event Handlers - These are methods invoked by the SmartFridge hardware to send notification of items that have
    /// been added and/or removed from the fridge. Every time an item is removed by the fridge user, it will emit a
    /// <see cref="HandleItemRemoved"/> event to this class, every time a new item is added or a previously removed item is re-inserted,
    /// the fridge will emit a <see cref="HandleItemAdded"/> event with its updated fillFactor.
    /// </remarks>
    public interface ISmartFridgeManager
    {
        /// <summary>
        /// This method is called every time an item is removed from the fridge
        /// </summary>
        /// <param name="itemUUID">Item identifier</param>
        void HandleItemRemoved(string itemUUID);

        /// <summary>
        /// This method is called every time an item is stored in the fridge
        /// </summary>
        /// <param name="itemType">Type of item</param>
        /// <param name="itemUUID">Item identifier</param>
        /// <param name="name">Item name</param>
        /// <param name="fillFactor">How full the item is.</param>
        void HandleItemAdded(long itemType, string itemUUID, string name, double fillFactor);

        /// <summary>
        /// Returns a list of items based on their fill factor. This method is used by the
        /// fridge to display items that are running low and need to be replenished.
        /// </summary>
        /// <example>
        /// <code>GetItems(0.5)</code> Will return  any items that are 50% or less full, including
        /// items that are depleted. Unless all available containers are
        /// empty, this method should only consider the non-empty containers
        /// when calculating the overall fillFactor for a given item.
        /// </example>
        /// <param name="fillFactor"></param>
        /// <returns></returns>
        object[] GetItems(double fillFactor);

        /// <summary>
        /// Returns the fill factor for a given item type to be displayed to the owner. Unless all available containers are
        /// empty, this method should only consider the non-empty containers
        /// when calculating the overall fillFactor for a given item.
        /// </summary>
        /// <param name="itemType">Type of item</param>
        /// <returns></returns>
        double GetFillFactor(long itemType);

        /// <summary>
        /// Stop tracking a given item. This method is used by the fridge to signal that its
        /// owner will no longer stock this item and thus should not be returned from <see cref="GetItems"/>
        /// </summary>
        /// <param name="itemType"></param>
        void ForgetItem(long itemType);
    }
}
