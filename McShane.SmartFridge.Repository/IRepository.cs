using System;

namespace McShane.SmartFridge.Repository
{
    public interface IRepository
    {
        void Remove(Guid id);
        void Add(long type, Guid id, string name, double fillFactor);
        double FillFactor(long itemType);
        void Forget(long itemType);
        object[] GetItems(double fillFactor);
    }
}
