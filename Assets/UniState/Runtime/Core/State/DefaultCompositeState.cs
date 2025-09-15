using System.Threading;
using Cysharp.Threading.Tasks;

namespace UniState
{
    public class DefaultCompositeState : DefaultCompositeState<EmptyPayload>
    {
    }

    public class DefaultCompositeState<TPayload> : CompositeStateBase<TPayload>
    {
        public override UniTask<StateTransitionInfo> ExecuteAsync(CancellationToken token) => SubStates.ExecuteAsync(token);
        public override UniTask InitializeAsync(CancellationToken token) => SubStates.InitializeAsync(token);
        public override UniTask ExitAsync(CancellationToken token) => SubStates.ExitAsync(token);
    }
}