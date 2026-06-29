using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public class SubStatesContainer<TPayload> : ISubStatesContainer<TPayload>, ISetupable<TPayload>
    {
        private List<IState<TPayload>> _subStates = new();

        public List<IState<TPayload>> List => _subStates;

        public void Initialize(List<IState<TPayload>> subStates)
        {
            _subStates = subStates;
        }

        public void SetPayload(TPayload payload)
        {
            for (var i = 0; i < _subStates.Count; i++)
            {
                _subStates[i].SetPayload(payload);
            }
        }

        public void SetTransitionFacade(IStateTransitionFacade transitionFacade)
        {
            for (var i = 0; i < _subStates.Count; i++)
            {
                _subStates[i].SetTransitionFacade(transitionFacade);
            }
        }

        public UniTask Initialize(CancellationToken token)
        {
            var tasks = new UniTask[_subStates.Count];
            for (var i = 0; i < _subStates.Count; i++)
            {
                tasks[i] = InitializeSubState(_subStates[i], token);
            }

            return UniTask.WhenAll(tasks);
        }

        public async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            if (_subStates.Count == 0)
            {
                var exception = new NoSubStatesException();
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.NoSubStates(exception);
#endif
                throw exception;
            }

            StateTransitionInfo result;
            var winnerIndex = -1;

            var ctx = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                var tasks = new UniTask<StateTransitionInfo>[_subStates.Count];
                for (var i = 0; i < _subStates.Count; i++)
                {
                    tasks[i] = ExecuteSubState(_subStates[i], ctx.Token);
                }

                var first = await UniTask.WhenAny(tasks);
                winnerIndex = first.winArgumentIndex;
                result = first.result;
            }
            finally
            {
                ctx.Cancel();
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.SubStatesCancelled(_subStates, winnerIndex);
#endif
                ctx.Dispose();
            }

            return result;
        }

        public UniTask Exit(CancellationToken token)
        {
            var tasks = new UniTask[_subStates.Count];
            for (var i = 0; i < _subStates.Count; i++)
            {
                tasks[i] = ExitSubState(_subStates[i], token);
            }

            return UniTask.WhenAll(tasks);
        }

        public void Dispose()
        {
            List<Exception> exceptions = null;

            for (var i = 0; i < _subStates.Count; i++)
            {
#if UNISTATE_DEBUG_TREE
                var debugScope = UniStateDebugRegistry.BeginStatePhase(_subStates[i], DebugStatePhase.Dispose);
#endif
                try
                {
                    _subStates[i].Dispose();
#if UNISTATE_DEBUG_TREE
                    UniStateDebugRegistry.EndStatePhase(_subStates[i], DebugStatePhase.Dispose);
#endif
                }
                catch (Exception e)
                {
#if UNISTATE_DEBUG_TREE
                    UniStateDebugRegistry.Error(_subStates[i], StateMachineErrorType.StateDisposing, e);
#endif
                    exceptions ??= new List<Exception>();
                    exceptions.Add(e);
                }
#if UNISTATE_DEBUG_TREE
                finally
                {
                    debugScope.Dispose();
                }
#endif
            }

            if (exceptions == null)
            {
                return;
            }

            if (exceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            }

            throw new AggregateException("One or more substate dispose operations failed.", exceptions);
        }

        private async UniTask InitializeSubState(IState<TPayload> state, CancellationToken token)
        {
#if UNISTATE_DEBUG_TREE
            var debugScope = UniStateDebugRegistry.BeginStatePhase(state, DebugStatePhase.Initialize);
#endif
            try
            {
                await state.Initialize(token);
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.EndStatePhase(state, DebugStatePhase.Initialize);
#endif
            }
            catch (OperationCanceledException)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.StateCancelled(state);
#endif
                throw;
            }
            catch (Exception e)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.Error(state, StateMachineErrorType.StateInitializing, e);
#endif
                throw;
            }
#if UNISTATE_DEBUG_TREE
            finally
            {
                debugScope.Dispose();
            }
#endif
        }

        private async UniTask<StateTransitionInfo> ExecuteSubState(IState<TPayload> state, CancellationToken token)
        {
#if UNISTATE_DEBUG_TREE
            var debugScope = UniStateDebugRegistry.BeginStatePhase(state, DebugStatePhase.Execute);
#endif
            try
            {
                var result = await state.Execute(token);
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.EndStatePhase(state, DebugStatePhase.Execute);
                UniStateDebugRegistry.SubStateCompleted(state, result);
#endif
                return result;
            }
            catch (OperationCanceledException)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.StateCancelled(state);
#endif
                throw;
            }
            catch (Exception e)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.Error(state, StateMachineErrorType.StateExecuting, e);
#endif
                throw;
            }
#if UNISTATE_DEBUG_TREE
            finally
            {
                debugScope.Dispose();
            }
#endif
        }

        private async UniTask ExitSubState(IState<TPayload> state, CancellationToken token)
        {
#if UNISTATE_DEBUG_TREE
            var debugScope = UniStateDebugRegistry.BeginStatePhase(state, DebugStatePhase.Exit);
#endif
            try
            {
                await state.Exit(token);
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.EndStatePhase(state, DebugStatePhase.Exit);
#endif
            }
            catch (OperationCanceledException)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.StateCancelled(state);
#endif
                throw;
            }
            catch (Exception e)
            {
#if UNISTATE_DEBUG_TREE
                UniStateDebugRegistry.Error(state, StateMachineErrorType.StateExiting, e);
#endif
                throw;
            }
#if UNISTATE_DEBUG_TREE
            finally
            {
                debugScope.Dispose();
            }
#endif
        }
    }
}
