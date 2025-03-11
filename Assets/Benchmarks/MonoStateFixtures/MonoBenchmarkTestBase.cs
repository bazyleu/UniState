using System.Collections;
using System.Collections.Generic;
using Benchmarks.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Benchmarks.MonoStateFixtures
{
    public class MonoBenchmarkTestBase : MonoBehaviour
    {
        private List<MonoStateBase> _history;
        private BenchmarkHelper _benchmarkHelper;

        private bool _executionCompleted = false;

        protected async UniTask Run(int stateCount)
        {
            _history = new List<MonoStateBase>();
            _benchmarkHelper = new BenchmarkHelper();

            _executionCompleted = false;
            _benchmarkHelper.SetStatesCountTarget(stateCount);

            StartCoroutine(StartStateMachine());

            await UniTask.WaitUntil(() => _executionCompleted);
        }

        public void Clear()
        {
            for (int i = _history.Count - 1; i >=0; i--)
            {
                Destroy(_history[i].gameObject);
            }

            _history.Clear();
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