namespace Benchmarks.Common
{
    public class BenchmarkHelper
    {
        private int _statesCountTarget;
        private int _executedStates;
        private int _executedMethods;

        public int ExecutedMethods => _executedMethods;

        public void SetStatesCountTarget(int stateCount)
        {
            _statesCountTarget = stateCount;
            _executedStates = 0;
            _executedMethods = 0;
        }

        public bool TargetReached() { return _executedStates >= _statesCountTarget; }

        public void ExecuteWasInvoked()
        {
            _executedStates++;
            _executedMethods++;
        }

        public void InitializeWasInvoked() => _executedMethods++;
        public void ExitWasInvoked() => _executedMethods++;
    }
}