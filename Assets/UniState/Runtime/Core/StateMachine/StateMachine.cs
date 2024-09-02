using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniState
{
    public class StateMachine : IStateMachine
    {
        private LimitedStack<StateTransitionInfo> _history;
        private IStateTransitionFactory _transitionFactory;

        public void Initialize(ITypeResolver resolver)
        {
            _transitionFactory = new StateTransitionFactory(resolver);
            _history = new LimitedStack<StateTransitionInfo>(15);
        }

        public async UniTask Execute<TState>(CancellationToken token) where TState : class, IState<EmptyPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState>(), token);
        }

        public async UniTask Execute<TState, TPayload>(TPayload payload, CancellationToken token)
            where TState : class, IState<TPayload>
        {
            await ExecuteInternal(_transitionFactory.CreateStateTransition<TState, TPayload>(payload), token);
        }

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
                OnError(e, StateMachineErrorType.StateMachineFail);
            }
            finally
            {
                nextStateMetadata.Dispose();
                nextStateMetadata.Clear();

                activeStateMetadata.Dispose();
                activeStateMetadata.Clear();
            }
        }

        private void OnError(Exception exception, StateMachineErrorType phase)
        {
            //Add Type (state type) to signature

            //Call Handler
        }

        private void ProcessTransitionInfo(StateTransitionInfo nextTransition,
            StateTransitionInfo previousTransition,
            StateWithMetadata stateWithMetadata)
        {
            stateWithMetadata.Clear();

            if (nextTransition.Transition != TransitionType.Exit)
            {
                var item = nextTransition;

                if (nextTransition.Transition == TransitionType.State)
                {
                    if (previousTransition != null)
                    {
                        _history.Push(previousTransition);
                    }
                }
                else
                {
                    item = GetInfoFromHistory();
                }

                if (item != null)
                {
                    stateWithMetadata.BuildState(item, item.StateBehaviourData);
                }
            }
        }

        private StateTransitionInfo GetInfoFromHistory()
        {
            var item = _history.Pop();

            while (item != null)
            {
                if (!item.StateBehaviourData.ProhibitReturnToState)
                {
                    return item;
                }

                item = _history.Pop();
            }

            return null;
        }

        private async UniTask<StateTransitionInfo> ExecuteSafe(IExecutableState state, CancellationToken token)
        {
            var transitionInfo = _transitionFactory.CreateBackTransition();

            try
            {
                token.ThrowIfCancellationRequested();

                transitionInfo = await state.Execute(token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                OnError(e, StateMachineErrorType.StateExecuting);
            }

            return transitionInfo;
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
                OnError(e, StateMachineErrorType.StateInitializing);
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
                OnError(e, StateMachineErrorType.StateExiting);
            }
            finally
            {
                state.Dispose();
            }
        }
    }
}