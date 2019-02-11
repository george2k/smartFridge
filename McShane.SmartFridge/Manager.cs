using System;
using DiscoverOrg;
using McShane.SmartFridge.Repository;

namespace McShane.SmartFridge
{
    /// <inheritdoc cref="ISmartFridgeManager"/>
    public class Manager : ISmartFridgeManager
    {
        private readonly IRepository _repository;

        public Manager(IRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc cref="ISmartFridgeManager.HandleItemRemoved"/>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="itemUUID"/> can not be parsed as a <see cref="Guid"/></exception>
        public void HandleItemRemoved(string itemUUID)
        {
            //Check if the itemUUID provided is a Guid and throw exection if it is not
            if (!Guid.TryParse(itemUUID, out Guid id))
            {
                throw new ArgumentOutOfRangeException(nameof(itemUUID), "Unable to parse value into Guid");
            }
            _repository.Remove(id);
        }

        /// <inheritdoc cref="ISmartFridgeManager.HandleItemRemoved"/>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="itemUUID"/> can not be parsed as a <see cref="Guid"/></exception>
        public void HandleItemAdded(long itemType, string itemUUID, string name, double fillFactor)
        {
            if (!Guid.TryParse(itemUUID, out Guid id))
            {
                throw new ArgumentOutOfRangeException(nameof(itemUUID), "Unable to parse value into Guid");
            }
            _repository.Add(itemType, id, name, fillFactor);
        }

        /// <inheritdoc cref="ISmartFridgeManager.GetItems"/>
        public object[] GetItems(double fillFactor)
        {
            return _repository.GetItems(fillFactor);
        }

        /// <inheritdoc cref="ISmartFridgeManager.GetFillFactor"/>
        public double GetFillFactor(long itemType)
        {
            return _repository.FillFactor(itemType);
        }

        /// <inheritdoc cref="ISmartFridgeManager.ForgetItem"/>
        public void ForgetItem(long itemType)
        {
            _repository.Forget(itemType);
        }
    }
}
