using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public class StateMachine : IStateMachine
    {
        private LimitedStack<StateTransitionInfo> _history;
        private IStateTransitionFactory _transitionFactory;

        protected virtual int MaxHistorySize => 15;

        public virtual void Initialize(ITypeResolver resolver)
        {
            _transitionFactory = new StateTransitionFactory(resolver);
            _history = new LimitedStack<StateTransitionInfo>(MaxHistorySize);
        }

        public virtual async UniTask Execute<TState>(CancellationToken token) where TState : class, IState<EmptyPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState>(), token);
        }

        public virtual async UniTask Execute<TState, TPayload>(TPayload payload, CancellationToken token)
            where TState : class, IState<TPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState, TPayload>(payload), token);
        }

        protected virtual void HandleError(StateMachineErrorData errorData)
        {
        }

        protected virtual StateTransitionInfo BuildRecoveryTransition(IStateTransitionFactory transitionFactory) =>
            transitionFactory.CreateBackTransition();

        private async UniTask ExecuteInternal(StateTransitionInfo initialTransition, CancellationToken token)
        {
            var activeStateMetadata = new StateWithMetadata();
            var nextStateMetadata = new StateWithMetadata();

            activeStateMetadata.BuildState(initialTransition, initialTransition.StateBehaviourData);

            try
            {
                await InitializeSafe(activeStateMetadata.State, token);

                var transitionInfo = await ExecuteSafe(activeStateMetadata.State, token);

                ProcessTransitionInfo(transitionInfo, activeStateMetadata.TransitionInfo, nextStateMetadata);

                while (!nextStateMetadata.IsEmpty && !token.IsCancellationRequested)
                {
                    if (nextStateMetadata.BehaviourData.InitializeOnStateTransition)
                    {
                        await InitializeSafe(nextStateMetadata.State, token);
                        await ExitAndDisposeSafe(activeStateMetadata.State, token);
                    }
                    else
                    {
                        await ExitAndDisposeSafe(activeStateMetadata.State, token);
                        await InitializeSafe(nextStateMetadata.State, token);
                    }

                    activeStateMetadata.CopyData(nextStateMetadata);

                    transitionInfo = await ExecuteSafe(activeStateMetadata.State, token);

                    ProcessTransitionInfo(transitionInfo, activeStateMetadata.TransitionInfo, nextStateMetadata);
                }

                await ExitAndDisposeSafe(activeStateMetadata.State, token);
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
            if (nextTransition.HistorySelector == null)
            {
                return _history.Pop();
            }

            while (_history.Count() > 0)
            {
                var info = _history.Pop();
                if (nextTransition.HistorySelector(info))
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

        private async UniTask InitializeSafe(IExecutableState state, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await state.Initialize(token);
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

        private async UniTask ExitAndDisposeSafe(IExecutableState state, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await state.Exit(token);
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