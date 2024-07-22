using NUnit.Framework;

namespace UniStateTests.EditMode.Common
{
    [TestFixture]
    public class EditModeStubTest
    {
        [Test]
        public void SuccessCheck()
        {
            Assert.False(false);
        }

        [Test]
        public void FailCheck()
        {
            Assert.False(true);
        }
    }
}