using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using McShane.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace McShane.SmartFridge.Common.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ItemTests
    {
        private readonly Random _random;
        private Guid _testId;
        private long _testType;
        private double _testFill;
        private string _testName;

        public ItemTests()
        {
            _random = new Random();
        }

        public void RandomItemValues(out long type, out Guid id, out string name, out double fillFactor)
        {
            id = Guid.NewGuid();
            type = _random.NextLong();
            fillFactor = _random.NextDouble();
            name = Guid.NewGuid().ToString("P");
        }

        [TestInitialize]
        public void TestInitialization()
        {
            RandomItemValues(out _testType, out _testId, out _testName, out _testFill);
        }

        [TestMethod]
        public void Valid()
        {
            Item newItem = new Item(_testType, _testId, _testName, _testFill);
            newItem.Type.Should().Be(_testType);
            newItem.Id.Should().Be(_testId);
            newItem.FillFactor.Should().Be(_testFill);
            newItem.Name.Should().Be(_testName);
        }

        [TestMethod]
        public void EmptyId()
        {
            Action testAction = () => new Item(_testType, Guid.Empty, _testName, _testFill);
            testAction.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Must provide a non-empty Guid*").And.ParamName.Should().Be("id");
        }

        [TestMethod]
        public void FillFactorBelowZero()
        {
            _testFill = -1;
            try
            {
                Item test = new Item(_testType, _testId, _testName, _testFill);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ActualValue.Should().Be(_testFill);
                exception.ParamName.Should().Be("fillFactor");
                exception.Message.Should().StartWith("Must be between 0 and 1 inclusively");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void FillFactorGreaterThenOne()
        {
            _testFill += 1; //Force the value to be greater then 1
            try
            {
                Item test = new Item(_testType, _testId, _testName, _testFill);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ActualValue.Should().Be(_testFill);
                exception.ParamName.Should().Be("fillFactor");
                exception.Message.Should().StartWith("Must be between 0 and 1 inclusively");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }

        [TestMethod]
        public void NameNull()
        {
            _testName = null;
            try
            {
                Item test = new Item(_testType, _testId, _testName, _testFill);
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
        public void NameEmpty()
        {
            _testName = string.Empty;
            try
            {
                Item test = new Item(_testType, _testId, _testName, _testFill);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                exception.ParamName.Should().Be("name");
                exception.Message.Should().StartWith("Must supply at least one character");
                return; //Pass test
            }
            Assert.Fail("Method should throw an ArgumentOutOfRangeException");
        }
    }
}
