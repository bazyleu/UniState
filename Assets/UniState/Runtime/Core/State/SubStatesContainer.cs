using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniState
{
    public class SubStatesContainer<TPayload> : ISubStatesContainer<TPayload>, ISetupable<TPayload>
    {
        private List<IState<TPayload>> _subStates = new();

        public List<IState<TPayload>> List => _subStates;

        public void Initialize(List<IState<TPayload>> subStates)
        {
            _subStates = new List<IState<TPayload>>(subStates);
        }

        public void SetPayload(TPayload payload) => _subStates.ForEach(s => s.SetPayload(payload));

        public void SetStateMachineFactory(IStateMachineFactory stateMachineFactory) =>
            _subStates.ForEach(s => s.SetStateMachineFactory(stateMachineFactory));

        public void SetTransitionFacade(IStateTransitionFacade transitionFacade) =>
            _subStates.ForEach(s => s.SetTransitionFacade(transitionFacade));

        public UniTask Initialize(CancellationToken token) =>
            UniTask.WhenAll(List.Select(s => s.Initialize(token)).ToArray());

        public async UniTask<StateTransitionInfo> Execute(CancellationToken token)
        {
            if (List.Count == 0)
            {
                throw new InvalidOperationException("No SubStates for execution");
            }

            StateTransitionInfo result;

            var ctx = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                var first = await UniTask.WhenAny(List.Select(s => s.Execute(ctx.Token)).ToArray());
                result = first.result;
            }
            finally
            {
                ctx.Cancel();
                ctx.Dispose();
            }

            return result;
        }

        public UniTask Exit(CancellationToken token) =>
            UniTask.WhenAll(List.Select(s => s.Exit(token)).ToArray());

        public void Dispose() => _subStates.ForEach(s => s.Dispose());
    }
}