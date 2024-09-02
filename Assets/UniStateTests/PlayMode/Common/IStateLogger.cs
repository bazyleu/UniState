namespace UniStateTests.PlayMode.Common
{
    //TODO: Refactor to more complex logger
    public interface IStateLogger
    {
        void LogLine(string line);
        string GetFullLog();
    }
}