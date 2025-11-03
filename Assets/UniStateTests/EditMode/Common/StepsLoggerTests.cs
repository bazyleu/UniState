using NUnit.Framework;
using UniStateTests.Common;

namespace UniStateTests.EditMode.Disposables
{
    [TestFixture]
    public class StepsLoggerTests
    {
        [Test]
        public void LogStep_BasicLogic()
        {
            const string expectedLog = "State1 (Initialize, Execute, Exit) -> " +
                                       "State2 (Initialize, Execute, Exit) -> " +
                                       "StateFoo (Initialize) -> StateBar (Execute) -> StateFoo (Exit)";

            var logger = new ExecutionLogger();

            logger.LogStep("State1", "Initialize");
            logger.LogStep("State1", "Execute");
            logger.LogStep("State1", "Exit");

            logger.LogStep("State2", "Initialize");
            logger.LogStep("State2", "Execute");
            logger.LogStep("State2", "Exit");

            logger.LogStep("StateFoo", "Initialize");
            logger.LogStep("StateBar", "Execute");
            logger.LogStep("StateFoo", "Exit");

            var actualLog = logger.FinishLogging();

            Assert.AreEqual(expectedLog, actualLog);
        }
    }
}