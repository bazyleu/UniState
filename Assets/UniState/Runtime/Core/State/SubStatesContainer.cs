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

        public void SetTransitionFacade(IStateTransitionFacade transitionFacade) =>
            _subStates.ForEach(s => s.SetTransitionFacade(transitionFacade));

        public UniTask InitializeAsync(CancellationToken token) =>
            UniTask.WhenAll(List.Select(s => s.InitializeAsync(token)).ToArray());

        public async UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token)
        {
            if (List.Count == 0)
            {
                throw new NoSubStatesException();
            }

            StateTransitionInfo result;

            var ctx = CancellationTokenSource.CreateLinkedTokenSource(token);
            try
            {
                var first = await UniTask.WhenAny(List.Select(s => s.ExecuteAsync(ctx.Token)).ToArray());
                result = first.result;
            }
            finally
            {
                ctx.Cancel();
                ctx.Dispose();
            }

            return result;
        }

        public UniTask ExitAsync(CancellationToken token) =>
            UniTask.WhenAll(List.Select(s => s.ExitAsync(token)).ToArray());

        public void Dispose() => _subStates.ForEach(s => s.Dispose());
    }
}