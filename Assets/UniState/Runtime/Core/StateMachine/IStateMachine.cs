using System.Threading;
using Cysharp.Threading.Tasks;
using UniState;

namespace UniState
{
    public interface IStateMachine: IInitializableStateMachine, IExecutableStateMachine
    {
    }
}