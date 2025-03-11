using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public class DefaultCompositeState : DefaultCompositeState<EmptyPayload>
    {
    }

    public class DefaultCompositeState<TPayload> : CompositeStateBase<TPayload>
    {
        public override UniTask<StateTransitionInfo> Execute(CancellationToken token) => SubStates.Execute(token);
        public override UniTask Initialize(CancellationToken token) => SubStates.Initialize(token);
        public override UniTask Exit(CancellationToken token) => SubStates.Exit(token);
    }
}