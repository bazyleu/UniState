#if !UNISTATE_DEBUG_TREE

using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.Debug
{
    [TestFixture]
    public class UniStateDebugDisabledTests
    {
        [Test]
        public void Facade_WhenDebugFlagIsDisabled_ReturnsNoOpValues()
        {
            var snapshot = UniStateDebug.GetSnapshot();

            Assert.False(UniStateDebug.IsEnabled);
            Assert.AreEqual(0, snapshot.Sequence);
            Assert.AreEqual(0, snapshot.Machines.Count);
            Assert.AreEqual("UniState debug is disabled.", UniStateDebug.DumpTree());
            Assert.That(UniStateDebug.DumpJson(false), Does.Contain("\"isEnabled\":false"));
            Assert.That(UniStateDebug.DumpJson(false), Does.Contain("UniState debug is disabled."));
        }
    }
}

#endif
