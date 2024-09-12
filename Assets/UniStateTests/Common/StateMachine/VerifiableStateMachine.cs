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

        protected override void HandleError(StateMachineErrorData errorData)
        {
            throw new Exception($"StateMachine HandleError. Current log: {_logger.FinishLogging()}", errorData.Exception);
        }

        public void Verify()
        {
            var actualLog = _logger.FinishLogging();
            Assert.AreEqual(ExpectedLog, actualLog);
        }
    }
}