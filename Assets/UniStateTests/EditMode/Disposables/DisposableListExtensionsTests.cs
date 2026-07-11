using System;
using System.Collections.Generic;
using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.Disposables
{
    [TestFixture]
    public class DisposableListExtensionsTests
    {
        private class DisposableSpy : IDisposable
        {
            private readonly Action _onDispose;

            public DisposableSpy(Action onDispose) => _onDispose = onDispose;

            public void Dispose() => _onDispose();
        }

        [Test]
        public void Add_WithAction_AddsDisposableAction()
        {
            var disposables = new List<IDisposable>();
            var disposeCount = 0;

            disposables.Add(() => disposeCount++);

            Assert.AreEqual(1, disposables.Count);
            Assert.IsInstanceOf<DisposableAction>(disposables[0]);

            disposables.Dispose();

            Assert.AreEqual(1, disposeCount);
        }

        [Test]
        public void Add_WithParams_AddsAllDisposables()
        {
            var first = new DisposableSpy(() => { });
            var second = new DisposableSpy(() => { });
            var third = new DisposableSpy(() => { });

            var disposables = new List<IDisposable> { first };

            disposables.Add(second, third);

            Assert.AreEqual(3, disposables.Count);
            Assert.AreSame(first, disposables[0]);
            Assert.AreSame(second, disposables[1]);
            Assert.AreSame(third, disposables[2]);
        }

        [Test]
        public void ThenAdd_WithDisposable_ReturnsSameList()
        {
            var disposables = new List<IDisposable>();
            var spy = new DisposableSpy(() => { });

            var result = disposables.ThenAdd(spy);

            Assert.AreSame(disposables, result);
            Assert.Contains(spy, disposables);
        }

        [Test]
        public void ThenAdd_WithAction_AddsDisposableActionAndReturnsList()
        {
            var disposables = new List<IDisposable>();

            var result = disposables.ThenAdd(() => { });

            Assert.AreSame(disposables, result);
            Assert.AreEqual(1, disposables.Count);
            Assert.IsInstanceOf<DisposableAction>(disposables[0]);
        }

        [Test]
        public void Dispose_DisposesInReverseOrderAndSkipsNull()
        {
            var callOrder = new List<int>();
            var first = new DisposableSpy(() => callOrder.Add(1));
            var second = new DisposableSpy(() => callOrder.Add(2));
            var third = new DisposableSpy(() => callOrder.Add(3));

            var disposables = new List<IDisposable> { first, null, second, third };

            disposables.Dispose();

            CollectionAssert.AreEqual(new[] { 3, 2, 1 }, callOrder);
        }

        [Test]
        public void Dispose_WithNullList_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => DisposableListExtensions.Dispose(null));
        }

        [Test]
        public void Dispose_WhenItemThrows_DisposesRemainingAndRethrows()
        {
            var callOrder = new List<int>();
            var disposables = new List<IDisposable>
            {
                new DisposableSpy(() => callOrder.Add(1)),
                new DisposableSpy(() => throw new InvalidOperationException("Dispose failure")),
                new DisposableSpy(() => callOrder.Add(3))
            };

            var exception = Assert.Throws<InvalidOperationException>(() => disposables.Dispose());

            Assert.AreEqual("Dispose failure", exception.Message);
            CollectionAssert.AreEqual(new[] { 3, 1 }, callOrder);
        }

        [Test]
        public void Dispose_WhenMultipleItemsThrow_ThrowsAggregateException()
        {
            var disposedCount = 0;
            var disposables = new List<IDisposable>
            {
                new DisposableSpy(() => throw new InvalidOperationException("First failure")),
                new DisposableSpy(() => disposedCount++),
                new DisposableSpy(() => throw new InvalidOperationException("Second failure"))
            };

            var exception = Assert.Throws<AggregateException>(() => disposables.Dispose());

            Assert.AreEqual(2, exception.InnerExceptions.Count);
            Assert.AreEqual(1, disposedCount);
        }
    }
}
