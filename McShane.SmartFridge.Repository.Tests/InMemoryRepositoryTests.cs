using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using McShane.Extensions;
using McShane.SmartFridge.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McShane.SmartFridge.Repository.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class InMemoryRepositoryTests
    {
        private InMemoryRepository _repository;
        private Item[] _itemList;
        private readonly Random _random;

        public InMemoryRepositoryTests()
        {
            _random = new Random();
        }

        [TestInitialize]
        public void TestInitialization()
        {
            _itemList = GetRandomItems(5);

            _repository = new InMemoryRepository();
            //Add items to the underlying collections manually to avoid dependencies on the methods we are testing 
            foreach (Item item in _itemList)
            {
                _repository.Items.Add(item.Id, item); //Add to the Id list
                if (!_repository.ItemsByType.ContainsKey(item.Type)) //If this is the first time we have seen the type
                {
                    _repository.ItemsByType.Add(item.Type, new ItemTypeCollection(item.Type)); //Create then type
                }
                _repository.ItemsByType[item.Type].Add(item); //Add to the Type list
            }
        }

        [TestMethod]
        public void RemoveExistingItem()
        {
            _repository.Items.Count.Should().Be(_itemList.Length); //All the items should be in the collections

            Item toRemove = _itemList[_random.Next(0, _itemList.Length)]; //find a random item to remove

            _repository.Remove(toRemove.Id); //remove the item

            _repository.Items.Count.Should().Be(_itemList.Length - 1);
            _repository.Items.Values.Contains(toRemove).Should().BeFalse();
        }

        [TestMethod]
        public void RemoveNonExistingItem()
        {
            _repository.Items.Count.Should().Be(_itemList.Length);
            _repository.Remove(Guid.NewGuid());
            _repository.Items.Count.Should().Be(_itemList.Length);
        }

        [TestMethod]
        public void AddValidItem()
        {
            long type = _random.NextLong();
            Guid id = Guid.NewGuid();
            double fillFactor = _random.NextDouble();
            string name = Guid.NewGuid().ToString("P");

            _repository.Items.Count.Should().Be(_itemList.Length);
            _repository.Items.Should().NotContainKey(id);
            _repository.Add(type, id, name, fillFactor);
            _repository.Items.Count.Should().Be(_itemList.Length + 1);
            _repository.Items.Should().ContainKey(id);
        }

        [TestMethod]
        public void AddItemWithEmptyName()
        {
            long type = _random.NextLong();
            Guid id = Guid.NewGuid();
            double fillFactor = _random.NextDouble();

            try
            {
                _repository.Add(type, id, string.Empty, fillFactor);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ParamName.Should().Be("name");
                exception.Message.Should().StartWith("Must supply at least one character");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void AddItemWithNullName()
        {
            long type = _random.NextLong();
            Guid id = Guid.NewGuid();
            double fillFactor = _random.NextDouble();

            try
            {
                _repository.Add(type, id, null, fillFactor);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ParamName.Should().Be("name");
                exception.Message.Should().StartWith("Must supply at least one character");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void AddItemWithEmptyId()
        {
            long type = _random.NextLong();
            Guid id = Guid.Empty;
            double fillFactor = _random.NextDouble();
            string name = Guid.NewGuid().ToString("P");

            try
            {
                _repository.Add(type, id, name, fillFactor);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ParamName.Should().Be("id");
                exception.Message.Should().StartWith("Must provide a non-empty Guid");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void AddItemWithFillFactorBelowZero()
        {
            long type = _random.NextLong();
            Guid id = Guid.NewGuid();
            double fillFactor = -1;
            string name = Guid.NewGuid().ToString("P");

            try
            {
                _repository.Add(type, id, name, fillFactor);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ActualValue.Should().Be(fillFactor);
                exception.ParamName.Should().Be("fillFactor");
                exception.Message.Should().StartWith("Must be between 0 and 1 inclusively");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void AddItemWithFillFactorGreaterThenOne()
        {
            long type = _random.NextLong();
            Guid id = Guid.NewGuid();
            double fillFactor = _random.NextDouble() + 1.1; //Force the fill factor over 1
            string name = Guid.NewGuid().ToString("P");

            try
            {
                _repository.Add(type, id, name, fillFactor);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ActualValue.Should().Be(fillFactor);
                exception.ParamName.Should().Be("fillFactor");
                exception.Message.Should().StartWith("Must be between 0 and 1 inclusively");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void ForgetItemType()
        {
            _repository.Items.Clear();
            _repository.ItemsByType.Clear();

            Item[] items = GetRandomItems(5);
            foreach (Item item in items)
            {
                _repository.Add(item.Type, item.Id, item.Name, item.FillFactor);
            }

            _repository.ItemsByType[items[0].Type].IsForgotten.Should().BeFalse();
            _repository.Forget(items[0].Type);
            _repository.ItemsByType[items[0].Type].IsForgotten.Should().BeTrue();
        }

        [TestMethod]
        public void ForgetItemTypeBeforeAdded()
        {
            _repository.Items.Clear();
            _repository.ItemsByType.Clear();

            Item[] items = GetRandomItems(5);

            _repository.ItemsByType.Should().NotContainKey(items[0].Type);
            _repository.Forget(items[0].Type);
            _repository.ItemsByType.Should().ContainKey(items[0].Type);
            _repository.ItemsByType[items[0].Type].IsForgotten.Should().BeTrue();

            foreach (Item item in items)
            {
                _repository.Add(item.Type, item.Id, item.Name, item.FillFactor);
            }

            _repository.ItemsByType[items[0].Type].IsForgotten.Should().BeTrue();
        }

        [TestMethod]
        public void FillFactorAllZero()
        {
            _repository.Items.Clear();
            _repository.ItemsByType.Clear();

            long itemType = _random.NextLong();
            Item[] items = GetRandomItems(5, itemType, 0);
            foreach (Item item in items)
            {
                _repository.Add(item.Type, item.Id, item.Name, item.FillFactor);
            }

            _repository.FillFactor(itemType).Should().Be(0);
        }

        [TestMethod]
        public void FillFactorNoItems()
        {
            _repository.Items.Clear();
            _repository.ItemsByType.Clear();

            _repository.FillFactor(_random.NextLong()).Should().Be(0);
        }

        [TestMethod]
        public void FillFactorAllNonZero()
        {
            _repository.Items.Clear();
            _repository.ItemsByType.Clear();

            long itemType = _random.NextLong();
            Item[] items = GetRandomItems(5, itemType);
            double sum = 0;
            int count = 0;

            foreach (Item item in items)
            {
                if (item.FillFactor > 0.0000001)
                {
                    sum += item.FillFactor;
                    count += 1;
                    _repository.Add(item.Type, item.Id, item.Name, item.FillFactor);
                }
            }

            _repository.FillFactor(itemType).Should().Be(sum / count);
        }

        [TestMethod]
        public void GetItems()
        {
            List<Tuple<long,double,Item[]>> testValues = new List<Tuple<long, double, Item[]>>();
            for (int i = 0; i < _random.Next(1,11); i++)
            {
                testValues.Add(GetItemWithSameType(_random.Next(1,3)));
            }

            _repository.Items.Clear();
            _repository.ItemsByType.Clear();

            foreach (Tuple<long, double, Item[]> testValue in testValues)
            {
                _repository.ItemsByType.Should().NotContainKey(testValue.Item1);

                foreach (Item item in testValue.Item3)
                {
                    _repository.Add(item.Type, item.Id, item.Name, item.FillFactor);
                }

                _repository.ItemsByType.Should().ContainKey(testValue.Item1);
                _repository.ItemsByType[testValue.Item1].FillFactor.Should().Be(testValue.Item2);
            }

            object[] actualResults = _repository.GetItems(0.5);
            List<object> expectedResultsList = new List<object>();
            var expectedList = testValues.Where(v => v.Item2 <= 0.5).OrderBy(v => v.Item2);
            foreach (var expectedItem in expectedList)
            {
                List<object> entry = new List<object>();
                entry.Add(expectedItem.Item1);
                entry.Add(expectedItem.Item2);
                entry.AddRange(expectedItem.Item3.OrderBy(i => i.FillFactor));
                expectedResultsList.Add(entry.ToArray());
            }

            object[] expectedResults = expectedResultsList.ToArray();

            actualResults.Length.Should().Be(expectedResults.Length);
            for (int i = 0; i < actualResults.Length; i++)
            {
                object[] actual = (object[])actualResults[i];
                object[] expected = (object[])expectedResults[i];

                actual.Length.Should().Be(expected.Length);
                
                actual[0].Should().BeOfType<long>();
                actual[1].Should().BeOfType<double>();

                expected[0].Should().BeOfType<long>();
                expected[1].Should().BeOfType<double>();

                actual[0].Should().BeEquivalentTo(expected[0]);
                actual[1].Should().BeEquivalentTo(expected[1]);

                for (int j = 2; j < actual.Length; j++)
                {
                    actual[j].Should().BeOfType<Item>();
                    expected[j].Should().BeOfType<Item>();

                    Item actualItem = (Item) actual[j];
                    Item expectedItem = (Item) expected[j];

                    actualItem.Type.Should().Be(expectedItem.Type);
                    actualItem.Id.Should().Be(expectedItem.Id);
                    actualItem.Name.Should().Be(expectedItem.Name);
                    actualItem.FillFactor.Should().Be(expectedItem.FillFactor);
                }
            }
        }

        private Tuple<long, double, Item[]> GetItemWithSameType(int itemCount)
        {
            double sum = 0;
            int count = 0;

            long itemType = _random.NextLong();
            Item[] items = GetRandomItems(itemCount, itemType);
            foreach (Item item in items)
            {
                if (item.FillFactor > 0.0000001)
                {
                    sum += item.FillFactor;
                    count += 1;
                }
            }

            return new Tuple<long, double, Item[]>(itemType, sum / count, items);
        }

        private Item[] GetRandomItems(int itemCount, long? typeOverride = null, double? fillFactorOverride = null)
        {
            if (itemCount <= 0)
            {
                return null;
            }
            
            Item[] itemsToReturn = new Item[itemCount];
            
            for (int i = 0; i < itemCount; i++)
            {
                itemsToReturn[i] = new Item(typeOverride ??_random.NextLong(), Guid.NewGuid(), Guid.NewGuid().ToString("P"), fillFactorOverride ?? _random.NextDouble());
            }

            return itemsToReturn;
        }
    }
}
