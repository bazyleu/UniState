using System.Collections;
using System.Collections.Generic;
using Benchmarks.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Benchmarks.MonoStateFixtures
{
    public class MonoStateMachine : MonoBehaviour
    {
        private readonly List<MonoStateBase> _history = new();
        private readonly BenchmarkHelper _benchmarkHelper = new();

        private bool _executionCompleted = false;

        private void Start()
        {
            Run(5).Forget();
        }

        public async UniTask Run(int stateCount)
        {
            _executionCompleted = false;
            _benchmarkHelper.SetStatesCountTarget(stateCount);

            StartCoroutine(StartStateMachine());

            await UniTask.WaitUntil(() => _executionCompleted);

            Debug.Log("ExecutedMethods:" + _benchmarkHelper.ExecutedMethods);
        }

        private IEnumerator StartStateMachine()
        {
            var initState = new GameObject().AddComponent<MonoFooState>();
            initState.SetBenchmarkHelper(_benchmarkHelper);

            MonoStateBase nextState = initState;

            while (nextState!=null)
            {
                yield return StartCoroutine(nextState.Initialize());
                yield return StartCoroutine(nextState.Execute());
                yield return StartCoroutine(nextState.Exit());

                _history.Add(nextState);

                nextState = nextState.NextState;
            }

            _executionCompleted = true;
        }
    }
}