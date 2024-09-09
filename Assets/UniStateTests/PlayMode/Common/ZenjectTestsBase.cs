using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Zenject;

namespace UniStateTests.PlayMode.Common
{
    public abstract class ZenjectTestsBase
    {
        private const int TimeoutSec = 5;

        private CancellationTokenSource _ctx;
        private IDisposable _timeoutSlim;
        private DiContainer _container;

        protected DiContainer Container => _container;


        [SetUp]
        public virtual void Setup()
        {
            _container = new DiContainer(StaticContext.Container);

            SetupBindings();
        }

        [TearDown]
        public virtual void Teardown()
        {
            _ctx?.Dispose();
            _ctx = null;

            _timeoutSlim?.Dispose();
            _timeoutSlim = null;

            StaticContext.Clear();
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

        protected virtual void SetupBindings()
        {
        }
    }
}