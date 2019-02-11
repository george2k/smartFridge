using System;
using FluentAssertions;
using McShane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McShane.SmartFridge.Tests
{
    [TestClass]
    public class ManagerTests
    {
        private Manager _manager;
        private Random _random;

        public ManagerTests()
        {
            _random = new Random();
        }

        [TestInitialize]
        public void TestInitialization()
        {
            _manager = new Manager(new TestRepository());
        }
        
        [TestMethod]
        public void GetItems()
        {
            _manager.GetItems(0.5).Should().BeEquivalentTo(1, 2, 3, 4, 5);
        }

        [TestMethod]
        public void GetFillFactor()
        {
            _manager.GetFillFactor(_random.NextLong()).Should().Be(0.1);
        }

        [TestMethod]
        public void ForgetItem()
        {
            _manager.ForgetItem(_random.NextLong());
        }

        [TestMethod]
        public void HandleItemRemovedBadGuid()
        {
            Action action = () => _manager.HandleItemRemoved("Not a GUID");
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Unable to parse value into Guid*").And.ParamName.Should().Be("itemUUID");
        }

        [TestMethod]
        public void HandleItemRemovedValidGuidFormatP()
        {
            _manager.HandleItemRemoved(Guid.NewGuid().ToString("P"));
        }

        [TestMethod]
        public void HandleItemRemovedValidGuidFormatN()
        {
            _manager.HandleItemRemoved(Guid.NewGuid().ToString("N"));
        }

        [TestMethod]
        public void HandleItemRemovedValidGuidFormatD()
        {
            _manager.HandleItemRemoved(Guid.NewGuid().ToString("D"));
        }

        [TestMethod]
        public void HandleItemRemovedValidGuidFormatB()
        {
            _manager.HandleItemRemoved(Guid.NewGuid().ToString("B"));
        }

        [TestMethod]
        public void HandleItemAddedBadGuid()
        {
            Action action = () => _manager.HandleItemAdded(_random.NextLong(), "Not a GUID", "Name", _random.NextDouble());
            action.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Unable to parse value into Guid*").And.ParamName.Should().Be("itemUUID");
        }

        [TestMethod]
        public void HandleItemAddedValidGuidFormatP()
        {
            _manager.HandleItemAdded(_random.NextLong(), Guid.NewGuid().ToString("P"), "Name", _random.NextDouble());
        }

        [TestMethod]
        public void HandleItemAddedValidGuidFormatN()
        {
            _manager.HandleItemAdded(_random.NextLong(), Guid.NewGuid().ToString("N"), "Name", _random.NextDouble());
        }

        [TestMethod]
        public void HandleItemAddedValidGuidFormatD()
        {
            _manager.HandleItemAdded(_random.NextLong(), Guid.NewGuid().ToString("D"), "Name", _random.NextDouble());
        }

        [TestMethod]
        public void HandleItemAddedValidGuidFormatB()
        {
            _manager.HandleItemAdded(_random.NextLong(), Guid.NewGuid().ToString("B"), "Name", _random.NextDouble());
        }
    }
}
