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

        public async UniTask Initialize(CancellationToken token)
        {
            var errors = new List<Exception>();

            var tasks = new UniTask[_subStates.Count];
            for (var i = 0; i < _subStates.Count; i++)
            {
                tasks[i] = InitializeGuarded(_subStates[i], token, errors);
            }

            await UniTask.WhenAll(tasks);

            ThrowIfAny(errors, "One or more substate initialize operations failed.");
            token.ThrowIfCancellationRequested();
        }

        public async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            if (_subStates.Count == 0)
            {
                throw new NoSubStatesException();
            }

            StateTransitionInfo result = null;
            var errors = new List<Exception>();

            var ctx = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                var tasks = new UniTask[_subStates.Count];
                for (var i = 0; i < _subStates.Count; i++)
                {
                    tasks[i] = ExecuteGuarded(_subStates[i]);
                }

                await UniTask.WhenAll(tasks);
            }
            finally
            {
                ctx.Cancel();
                ctx.Dispose();
            }

            ThrowIfAny(errors, "One or more substate execute operations failed.");

            if (result == null)
            {
                token.ThrowIfCancellationRequested();
            }

            return result;

            async UniTask ExecuteGuarded(IState<TPayload> subState)
            {
                try
                {
                    var transition = await subState.Execute(ctx.Token);

                    result ??= transition;
                    ctx.Cancel();
                }
                catch (OperationCanceledException) when (ctx.IsCancellationRequested)
                {
                }
                catch (Exception e)
                {
                    errors.Add(e);
                    ctx.Cancel();
                }
            }
        }

        public async UniTask Exit(CancellationToken token)
        {
            var errors = new List<Exception>();

            var tasks = new UniTask[_subStates.Count];
            for (var i = 0; i < _subStates.Count; i++)
            {
                tasks[i] = ExitGuarded(_subStates[i], token, errors);
            }

            await UniTask.WhenAll(tasks);

            ThrowIfAny(errors, "One or more substate exit operations failed.");
            token.ThrowIfCancellationRequested();
        }

        public void Dispose()
        {
            List<Exception> exceptions = null;

            for (var i = 0; i < _subStates.Count; i++)
            {
                try
                {
                    _subStates[i].Dispose();
                }
                catch (Exception e)
                {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(e);
                }
            }

            if (exceptions == null)
            {
                return;
            }

            ThrowIfAny(exceptions, "One or more substate dispose operations failed.");
        }

        private static async UniTask InitializeGuarded(IState<TPayload> subState, CancellationToken token,
            List<Exception> errors)
        {
            try
            {
                await subState.Initialize(token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
            catch (Exception e)
            {
                errors.Add(e);
            }
        }

        private static async UniTask ExitGuarded(IState<TPayload> subState, CancellationToken token,
            List<Exception> errors)
        {
            try
            {
                await subState.Exit(token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
            }
            catch (Exception e)
            {
                errors.Add(e);
            }
        }

        private static void ThrowIfAny(List<Exception> exceptions, string message)
        {
            if (exceptions.Count == 0)
            {
                return;
            }

            if (exceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            }

            throw new AggregateException(message, exceptions);
        }
    }
}
