using System.Text;

namespace UniStateTests.PlayMode.Common
{
    public class StateLogger : IStateLogger
    {
        private readonly StringBuilder _stringBuilder = new(500);
        public void LogLine(string line) => _stringBuilder.AppendLine(line);
        public string GetFullLog() => _stringBuilder.ToString();
    }
}