using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.StateMachineLifecycle
{
    [TestFixture]
    public class StateMachineErrorHandlingTests
    {
        [Test]
        public void Execute_WhenHandleErrorThrows_PropagatesExceptionToCallerOnce()
        {
            var resolver = new TestResolver();
            var stateMachine = new HaltingStateMachine();
            stateMachine.SetResolver(resolver);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                stateMachine.Execute<ThrowingExecuteState>(CancellationToken.None).GetAwaiter().GetResult());

            Assert.AreEqual("Halt", exception.Message);
            Assert.AreEqual("Execution failure", exception.InnerException?.Message);
            Assert.AreEqual(1, stateMachine.HandleErrorCalls);
            Assert.False(stateMachine.IsExecuting);
            Assert.AreEqual(1, resolver.ThrowingState.DisposeCount);
        }

        [Test]
        public void Execute_WithForeignOperationCanceled_RoutesToHandleErrorAndRecovers()
        {
            var resolver = new TestResolver();
            var stateMachine = new RecordingStateMachine();
            stateMachine.SetResolver(resolver);

            Assert.DoesNotThrow(() =>
                stateMachine.Execute<ForeignCancellationState>(CancellationToken.None).GetAwaiter().GetResult());

            Assert.NotNull(stateMachine.LastError);
            Assert.AreEqual(StateMachineErrorType.StateExecuting, stateMachine.LastError.ErrorType);
            Assert.IsInstanceOf<OperationCanceledException>(stateMachine.LastError.Exception);
            Assert.False(stateMachine.IsExecuting);
        }

        [Test]
        public void Execute_WithoutResolver_ThrowsInvalidOperationException()
        {
            var stateMachine = new StateMachine();

            Assert.Throws<InvalidOperationException>(() =>
                stateMachine.Execute<ThrowingExecuteState>(CancellationToken.None).GetAwaiter().GetResult());
        }

        private sealed class HaltingStateMachine : StateMachine
        {
            public int HandleErrorCalls { get; private set; }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                HandleErrorCalls++;

                throw new InvalidOperationException("Halt", errorData.Exception);
            }
        }

        private sealed class RecordingStateMachine : StateMachine
        {
            public StateMachineErrorData LastError { get; private set; }

            protected override void HandleError(StateMachineErrorData errorData)
            {
                LastError = errorData;
            }
        }

        private sealed class TestResolver : ITypeResolver
        {
            public ThrowingExecuteState ThrowingState { get; private set; }

            public object Resolve(Type type)
            {
                if (type == typeof(ThrowingExecuteState))
                {
                    return ThrowingState = new ThrowingExecuteState();
                }

                if (type == typeof(ForeignCancellationState))
                {
                    return new ForeignCancellationState();
                }

                throw new InvalidOperationException(type.FullName);
            }
        }

        private sealed class ThrowingExecuteState : StateBase
        {
            public int DisposeCount { get; private set; }

            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                throw new Exception("Execution failure");

            public override void Dispose()
            {
                DisposeCount++;
                base.Dispose();
            }
        }

        private sealed class ForeignCancellationState : StateBase
        {
            public override UniTask<StateTransitionInfo> Execute(CancellationToken token) =>
                throw new OperationCanceledException();
        }
    }
}
