using System;
using McShane.SmartFridge.Repository;

namespace McShane.SmartFridge.Tests
{
    public class TestRepository : IRepository
    {
        public void Remove(Guid id)
        {}

        public void Add(long type, Guid id, string name, double fillFactor)
        {}

        public double FillFactor(long itemType)
        {
            return 0.1;
        }

        public void Forget(long itemType)
        {}

        public object[] GetItems(double fillFactor)
        {
            return new object[] {1, 2, 3, 4, 5};
        }
    }
}