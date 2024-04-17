namespace TVScanner.Shared.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Caching.Memory;
    using NSubstitute;
    using NUnit.Framework;
    using Shouldly;
    using TVScanner.Shared;
    using T = System.String;

    [TestFixture]
    public class InMemoryCacheTests
    {
        private InMemoryCache _testClass;
        private IMemoryCache _memoryCache;

        [SetUp]
        public void SetUp()
        {
            _memoryCache = Substitute.For<IMemoryCache>();
            _testClass = new InMemoryCache(_memoryCache);
        }

        [Test]
        public void CanConstruct()
        {
            // Act
            var instance = new InMemoryCache(_memoryCache);

            // Assert
            instance.ShouldNotBeNull();
        }

        [Test]
        public void CannotConstructWithNullMemoryCache()
        {
            Should.Throw<ArgumentNullException>(() => new InMemoryCache(default(IMemoryCache)));
        }

        [Test]
        public async Task CanCallSubscribe()
        {
            // Arrange
            var messageName = "TestValue1949211290";
            var result = 1;
            Func<object, Task> value = async x => result = 0;

            // Act
            _testClass.Subscribe<T>(messageName, value);
            await _testClass.Set<T>(messageName, "TestValue1949211290");

            // Assert
            result.ShouldBe(0);
        }

        [Test]
        public void CannotCallSubscribeWithNullValue()
        {
            Should.Throw<ArgumentNullException>(() => _testClass.Subscribe<T>("TestValue856107380", default(Func<object, Task>)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallSubscribeWithInvalidMessageName(string value)
        {
            Should.Throw<ArgumentNullException>(() => _testClass.Subscribe<T>(value, x => Task.CompletedTask));
        }

        [Test]
        public void CanCallGet()
        {
            // Arrange
            var key = "TestValue1837332853";

            // Act
            var result = _testClass.Get<T>(key);

            // Assert
            _memoryCache.Received(1).Get<T>(key);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CannotCallGetWithInvalidKey(string value)
        {
            Should.Throw<ArgumentNullException>(() => _testClass.Get<T>(value));
        }

        [Test]
        public async Task CanCallSet()
        {
            // Arrange
            var key = "TestValue1122346741";
            var value = "TestValue1371978434";

            // Act
            await _testClass.Set<T>(key, value);

            // Assert
            _memoryCache.Received(1).Set(key, value);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public async Task CannotCallSetWithInvalidKey(string value)
        {
            await Should.ThrowAsync<ArgumentNullException>(() => _testClass.Set<T>(value, "TestValue1106051721"));
        }
    }
}