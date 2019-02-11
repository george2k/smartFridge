using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using McShane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McShane.SmartFridge.Common.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ItemTypeCollectionTests
    {
        private ItemTypeCollection _itemToTest;
        private readonly Random _random;
        private readonly ItemTests _itemTests;

        public ItemTypeCollectionTests()
        {
            _random = new Random();
            _itemTests = new ItemTests();
        }

        [TestInitialize]
        public void TestInitialization()
        {
            _itemToTest = new ItemTypeCollection(_random.NextLong());
        }

        [TestMethod]
        public void AddItemOfSameType()
        {
            Guid id;
            long type;
            double fillFactor;
            string name;

            _itemTests.RandomItemValues(out type, out id, out name, out fillFactor);
            type = _itemToTest.ItemType;
            
            //Make sure there are not items in the collection
            _itemToTest.InternalItems.Count.Should().Be(0);
            _itemToTest.Items.Count.Should().Be(0);
            
            _itemToTest.Add(new Item(type, id, name, fillFactor));
            
            //Make sure the item was added to the collection
            _itemToTest.InternalItems.Count.Should().Be(1);
            _itemToTest.Items.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddItemOfWrongType()
        {
            Guid id;
            long type;
            double fillFactor;
            string name;

            _itemTests.RandomItemValues(out type, out id, out name, out fillFactor);
            type = _itemToTest.ItemType + 1; //Force types to not match

            Action action = () => _itemToTest.Add(new Item(type, id, name, fillFactor));
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Item must be the same type as the collection*").And.ParamName.Should().Be("item");
        }

        [TestMethod]
        public void FillFactorAllZero()
        {
            Item[] items = GetRandomItems(5, _itemToTest.ItemType, 0);
            foreach (Item item in items)
            {
                _itemToTest.Add(item);
            }

            _itemToTest.FillFactor.Should().Be(0);
        }

        [TestMethod]
        public void FillFactorNoItems()
        {
            _itemToTest.FillFactor.Should().Be(0);
        }

        [TestMethod]
        public void FillFactorAllNonZero()
        {
            Item[] items = GetRandomItems(5, _itemToTest.ItemType);
            double sum = 0;
            int count = 0;

            foreach (Item item in items)
            {
                if (!(item.FillFactor > 0.0000001)) continue; //Empty item and need to skip
                sum += item.FillFactor;
                count += 1;
                _itemToTest.Add(item);
            }

            _itemToTest.FillFactor.Should().Be(sum / count);
        }

        private Item[] GetRandomItems(int itemCount, long? typeOverride = null, double? fillFactorOverride = null)
        {
            if (itemCount <= 0)
            {
                return null; //No items were requested
            }
            
            Item[] itemsToReturn = new Item[itemCount];
            
            for (int i = 0; i < itemCount; i++)
            {
                // if either typeOverride or fillFactorOverride are not provided then use a random value
                itemsToReturn[i] = new Item(typeOverride ??_random.NextLong(), Guid.NewGuid(), Guid.NewGuid().ToString("P"), fillFactorOverride ?? _random.NextDouble());
            }

            return itemsToReturn;
        }
    }
}
