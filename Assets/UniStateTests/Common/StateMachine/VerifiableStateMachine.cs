using System;
using NUnit.Framework;
using UniState;

namespace UniStateTests.Common
{
    public abstract class VerifiableStateMachine : StateMachine, IVerifiableStateMachine
    {
        private readonly ExecutionLogger _logger;

        protected abstract string ExpectedLog { get; }

        protected VerifiableStateMachine(ExecutionLogger logger)
        {
            _logger = logger;
        }

        protected override void OnError(Exception exception, StateMachineErrorType phase)
        {
            throw new Exception($"StateMachine OnError. Current log: {_logger.FinishLogging()}", exception);
        }

        public void Verify()
        {
            Assert.AreEqual(ExpectedLog, _logger.FinishLogging());
        }
    }
}