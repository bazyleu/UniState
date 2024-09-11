using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Zenject;

namespace UniStateTests.Common
{
    public abstract class TestsBase
    {
        private const int TimeoutSec = 5;

        private CancellationTokenSource _ctx;
        private IDisposable _timeoutSlim;
        private DiContainer _container;

        [SetUp]
        public virtual void Setup()
        {
        }

        [TearDown]
        public virtual void TearDown()
        {
            _ctx?.Cancel();
            _ctx?.Dispose();
            _ctx = null;

            _timeoutSlim?.Dispose();
            _timeoutSlim = null;
        }

        protected CancellationToken GetTimeoutToken()
        {
            _ctx ??= new CancellationTokenSource();

            if (_timeoutSlim != null)
            {
                _timeoutSlim = _ctx.CancelAfterSlim(TimeSpan.FromSeconds(TimeoutSec), DelayType.UnscaledDeltaTime);
            }

            return _ctx.Token;
        }
    }
}