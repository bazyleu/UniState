using System;
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

        public virtual async UniTask ExecuteAsync<TState>(CancellationToken token) where TState : class, IState<EmptyPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState>(), token);
        }

        public virtual async UniTask ExecuteAsync<TState, TPayload>(TPayload payload, CancellationToken token)
            where TState : class, IState<TPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState, TPayload>(payload), token);
        }

        protected virtual void HandleError(StateMachineErrorData errorData)
        {
            UnityEngine.Debug.LogError(errorData.Exception);
        }

        protected virtual StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory) =>
            transitionFactory.CreateBackTransition();

        private void Initialize()
        {
            _history = new LimitedStack<StateTransitionInfo>(MaxHistorySize);
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

            activeStateMetadata.BuildState(initialTransition, initialTransition.StateBehaviourData);

            try
            {
                await InitializeSafeAsync(activeStateMetadata.State, token);

                var transitionInfo = await ExecuteSafeAsync(activeStateMetadata.State, token);

                ProcessTransitionInfo(transitionInfo, activeStateMetadata.TransitionInfo, nextStateMetadata);

                while (!nextStateMetadata.IsEmpty && !token.IsCancellationRequested)
                {
                    if (nextStateMetadata.BehaviourData.InitializeOnStateTransition)
                    {
                        await InitializeSafeAsync(nextStateMetadata.State, token);
                        await ExitAndDisposeSafeAsync(activeStateMetadata.State, token);
                    }
                    else
                    {
                        await ExitAndDisposeSafeAsync(activeStateMetadata.State, token);
                        await InitializeSafeAsync(nextStateMetadata.State, token);
                    }

                    activeStateMetadata.CopyData(nextStateMetadata);

                    transitionInfo = await ExecuteSafeAsync(activeStateMetadata.State, token);

                    ProcessTransitionInfo(transitionInfo, activeStateMetadata.TransitionInfo, nextStateMetadata);
                }

                await ExitAndDisposeSafeAsync(activeStateMetadata.State, token);
                activeStateMetadata.Clear();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateMachineFail));
            }
            finally
            {
                nextStateMetadata.Dispose();
                nextStateMetadata.Clear();

                activeStateMetadata.Dispose();
                activeStateMetadata.Clear();

                _isExecuting = false;
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

        private async UniTask<StateTransitionInfo> ExecuteSafeAsync(IExecutableState state, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                return await state.ExecuteAsync(token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateExecuting, state));
            }

            return BuildRecoveryTransition(_transitionFactory);
        }

        private async UniTask InitializeSafeAsync(IExecutableState state, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await state.InitializeAsync(token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateInitializing, state));
            }
        }

        private async UniTask ExitAndDisposeSafeAsync(IExecutableState state, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await state.ExitAsync(token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(new StateMachineErrorData(e, StateMachineErrorType.StateExiting, state));
            }
            finally
            {
                state.Dispose();
            }
        }

        private void ProcessError(StateMachineErrorData errorData) => HandleError(errorData);
    }
}