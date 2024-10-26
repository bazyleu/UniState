namespace UniStateTests.PlayMode.HistoryTests.Infrastructure
{
    internal class HistorySizeTestHelper
    {
        private int _maxTransitionCount = 0;
        private int _currentTransitionCount = 0;

        public void SetMaxTransitionCount(int count)
        {
            _maxTransitionCount = count;
            _currentTransitionCount = 0;
        }

        public void ReportTransition() =>  _currentTransitionCount++;

        public bool ShouldGoBack => _currentTransitionCount >= _maxTransitionCount;
    }
}