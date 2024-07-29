using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.Common
{
    [TestFixture]
    internal class StateResourcesTests
    {
        private class ResourcesState : StateBase<IDisposable>
        {
            public int DisposeCount;

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token)
            {
                Resources.Add(Payload);
                Resources.Add(() => DisposeCount++);

                return default;
            }
        }

        [Test]
        public void Dispose_DisposesResources()
        {
            var wasDisposed = false;
            var state = SetUpStateForDisposal(new DisposableAction(() => wasDisposed = true));

            Assert.IsFalse(wasDisposed);
            state.Dispose();
            Assert.IsTrue(wasDisposed);
        }

        [Test]
        public void Dispose_Twice_DisposedOnce()
        {
            var state = SetUpStateForDisposal(default);
            
            Assert.AreEqual(state.DisposeCount, 0);
            state.Dispose();
            Assert.AreEqual(state.DisposeCount, 1);
            state.Dispose();
            Assert.AreEqual(state.DisposeCount, 1);
        }

        private static ResourcesState SetUpStateForDisposal(IDisposable disposable)
        {
            var state = new ResourcesState();

            state.SetPayload(disposable);
            state.Execute(default).GetAwaiter().GetResult();

            return state;
        }
    }
}