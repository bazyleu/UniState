using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public class StateMachine : IStateMachine
    {
        private LimitedStack<StateTransitionInfo> _history;
        private IStateTransitionFactory _transitionFactory;

        public bool IsExecuting => _isExecuting;
        protected virtual int MaxHistorySize => 15;

        private bool _isExecuting = false;

        public virtual void SetResolver(ITypeResolver resolver)
        {
            _transitionFactory = new StateTransitionFactory(resolver);
        }

        public virtual async UniTask Execute<TState>(CancellationToken token) where TState : class, IState<EmptyPayload>
        {
            ThrowIfResolverNotSet();

            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState>(), token);
        }

        public virtual async UniTask Execute<TState, TPayload>(TPayload payload, CancellationToken token)
            where TState : class, IState<TPayload>
        {
            ThrowIfResolverNotSet();

            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState, TPayload>(payload), token);
        }

        protected virtual void HandleError(StateMachineErrorData errorData)
        {
            UnityEngine.Debug.LogError(errorData.Exception);
        }

        protected virtual StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory) =>
            transitionFactory.CreateBackTransition();

        protected virtual void HandleStateChanged(StateMachineStateChangedData changeData)
        {
        }

        private void ThrowIfResolverNotSet()
        {
            if (_transitionFactory == null)
            {
                throw new InvalidOperationException(
                    "Resolver is not set. Call SetResolver() before Execute() or register the state machine via a DI integration.");
            }
        }

        private void Initialize()
        {
            if (_history == null || _history.MaxSize != MaxHistorySize)
            {
                _history = new LimitedStack<StateTransitionInfo>(MaxHistorySize);
            }
            else
            {
                _history.Clear();
            }

            _isExecuting = true;
        }

        private async UniTask ExecuteInternal(StateTransitionInfo initialTransition, CancellationToken token)
        {
            if (_isExecuting)
            {
                throw new AlreadyExecutingException();
            }

            Initialize();

            var activeStateMetadata = new StateWithMetadata();
            var nextStateMetadata = new StateWithMetadata();
            StateTransitionInfo transitionInfo = null;

            try
            {
                activeStateMetadata.BuildState(initialTransition, initialTransition.StateBehaviourData);

                await InitializeSafe(activeStateMetadata.State, token);

                ProcessStateChanged(new StateMachineStateChangedData(
                    null,
                    activeStateMetadata.TransitionInfo.Creator?.StateType,
                    null,
                    activeStateMetadata.TransitionInfo,
                    initialTransition,
                    StateMachineStateChangeType.Started));

                transitionInfo = await ExecuteSafe(activeStateMetadata.State, token);

                ProcessTransitionInfo(transitionInfo, activeStateMetadata.TransitionInfo, nextStateMetadata);

                while (!nextStateMetadata.IsEmpty && !token.IsCancellationRequested)
                {
                    var previousTransition = activeStateMetadata.TransitionInfo;

                    if (nextStateMetadata.BehaviourData.InitializeOnStateTransition)
                    {
                        await InitializeSafe(nextStateMetadata.State, token);
                        await ExitAndDisposeSafe(activeStateMetadata, token);
                    }
                    else
                    {
                        await ExitAndDisposeSafe(activeStateMetadata, token);
                        await InitializeSafe(nextStateMetadata.State, token);
                    }

                    activeStateMetadata.CopyData(nextStateMetadata);
                    nextStateMetadata.Clear();

                    ProcessStateChanged(new StateMachineStateChangedData(
                        previousTransition.Creator?.StateType,
                        activeStateMetadata.TransitionInfo.Creator?.StateType,
                        previousTransition,
                        activeStateMetadata.TransitionInfo,
                        transitionInfo,
                        StateMachineStateChangeType.Changed));

                    transitionInfo = await ExecuteSafe(activeStateMetadata.State, token);

                    ProcessTransitionInfo(transitionInfo, activeStateMetadata.TransitionInfo, nextStateMetadata);
                }

                var exitedTransition = activeStateMetadata.TransitionInfo;

                await ExitAndDisposeSafe(activeStateMetadata, token);

                ProcessStateChanged(new StateMachineStateChangedData(
                    exitedTransition.Creator?.StateType,
                    null,
                    exitedTransition,
                    null,
                    transitionInfo,
                    StateMachineStateChangeType.Exited));
                activeStateMetadata.Clear();
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                NotifyCanceled(activeStateMetadata, transitionInfo);

                throw;
            }
            catch (HandlerFailedException e)
            {
                ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            }
            catch (Exception e)
            {
                try
                {
                    ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateMachineFail));
                }
                catch (HandlerFailedException handlerException)
                {
                    ExceptionDispatchInfo.Capture(handlerException.InnerException).Throw();
                }
            }
            finally
            {
                Exception disposeException = null;

                try
                {
                    DisposeSafe(nextStateMetadata);
                }
                catch (HandlerFailedException e)
                {
                    disposeException ??= e.InnerException;
                }
                catch (Exception e)
                {
                    disposeException ??= e;
                }

                nextStateMetadata.Clear();

                try
                {
                    DisposeSafe(activeStateMetadata);
                }
                catch (HandlerFailedException e)
                {
                    disposeException ??= e.InnerException;
                }
                catch (Exception e)
                {
                    disposeException ??= e;
                }

                activeStateMetadata.Clear();

                _isExecuting = false;

                if (disposeException != null)
                {
                    ExceptionDispatchInfo.Capture(disposeException).Throw();
                }
            }
        }

        private void ProcessTransitionInfo(StateTransitionInfo nextTransition,
            StateTransitionInfo previousTransition,
            StateWithMetadata stateWithMetadata)
        {
            stateWithMetadata.Clear();

            if (nextTransition.Transition == TransitionType.Exit)
            {
                return;
            }

            var transitionToState = nextTransition.Transition == TransitionType.State;

            var item = transitionToState ? nextTransition : GetInfoFromHistory(nextTransition);

            if (transitionToState && previousTransition.CanBeAddedToHistory())
            {
                _history.Push(previousTransition);
            }

            if (item != null)
            {
                stateWithMetadata.BuildState(item, item.StateBehaviourData);
            }
        }

        private StateTransitionInfo GetInfoFromHistory(StateTransitionInfo nextTransition)
        {
            if (nextTransition.GoBackToType == null)
            {
                return _history.Pop();
            }

            while (_history.Count() > 0)
            {
                var info = _history.Pop();
                if (nextTransition.GoBackToType == info.Creator?.StateType)
                {
                    return info;
                }
            }

            return null;
        }

        private async UniTask<StateTransitionInfo> ExecuteSafe(IExecutableState state, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                return await state.Execute(token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateExecuting, state));
            }

            return BuildRecoveryTransition(_transitionFactory);
        }

        private async UniTask InitializeSafe(IExecutableState state, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await state.Initialize(token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateInitializing, state));
            }
        }

        private async UniTask ExitAndDisposeSafe(StateWithMetadata metadata, CancellationToken token)
        {
            var state = metadata.State;

            try
            {
                token.ThrowIfCancellationRequested();
                await state.Exit(token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateExiting, state));
            }
            finally
            {
                DisposeSafe(metadata);
            }
        }

        private void DisposeSafe(StateWithMetadata metadata)
        {
            var state = metadata.State;

            try
            {
                metadata.Dispose();
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateDisposing, state));
            }
        }

        private void NotifyCanceled(StateWithMetadata activeStateMetadata, StateTransitionInfo requestedTransition)
        {
            try
            {
                ProcessStateChanged(new StateMachineStateChangedData(
                    activeStateMetadata.TransitionInfo?.Creator?.StateType,
                    null,
                    activeStateMetadata.TransitionInfo,
                    null,
                    requestedTransition,
                    StateMachineStateChangeType.Canceled));
            }
            catch (Exception e)
            {
                try
                {
                    ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateMachineFail));
                }
                catch (Exception)
                {
                    // Cancellation must propagate even if user callbacks throw here.
                }
            }
        }

        private void ProcessError(StateMachineErrorData errorData)
        {
            try
            {
                HandleError(errorData);
            }
            catch (Exception e)
            {
                throw new HandlerFailedException(e);
            }
        }

        private void ProcessStateChanged(StateMachineStateChangedData changeData) => HandleStateChanged(changeData);

        private sealed class HandlerFailedException : Exception
        {
            public HandlerFailedException(Exception innerException)
                : base(innerException.Message, innerException)
            {
            }
        }
    }
}
