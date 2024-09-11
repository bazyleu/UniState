using System.Text;

namespace UniStateTests.Common
{
    public class ExecutionLogger
    {
        private readonly StringBuilder _sb = new(500);

        private string _lastEntity;

        public void LogStep(string entityName, string executionStep)
        {
            if (_lastEntity == entityName)
            {
                _sb.Append(", ");
                _sb.Append(executionStep);
            }
            else
            {
                if (!string.IsNullOrEmpty(_lastEntity))
                {
                    _sb.Append(") -> ");
                }

                _lastEntity = entityName;

                _sb.Append(entityName);
                _sb.Append(" (");
                _sb.Append(executionStep);
            }
        }

        public string FinishLogging()
        {
            if (!string.IsNullOrEmpty(_lastEntity))
            {
                _sb.Append(")");

                _lastEntity = null;
            }

            var result = _sb.ToString();

            _sb.Clear();

            return result;
        }
    }
}