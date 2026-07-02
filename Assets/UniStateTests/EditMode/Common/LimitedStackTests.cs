using NUnit.Framework;
using UniState;

namespace UniStateTests.EditMode.Common
{
    [TestFixture]
    public class LimitedStackTests
    {
        [Test]
        public void PushAndPop_ReturnsElementsInReverseOrder()
        {
            var stack = new LimitedStack<string>(3);

            stack.Push("a");
            stack.Push("b");

            Assert.AreEqual(2, stack.Count());
            Assert.AreEqual("b", stack.Pop());
            Assert.AreEqual("a", stack.Pop());
            Assert.AreEqual(0, stack.Count());
        }

        [Test]
        public void Push_OverCapacity_EvictsOldestElement()
        {
            var stack = new LimitedStack<string>(3);

            stack.Push("a");
            stack.Push("b");
            stack.Push("c");
            stack.Push("d");

            Assert.AreEqual(3, stack.Count());
            Assert.AreEqual("d", stack.Pop());
            Assert.AreEqual("c", stack.Pop());
            Assert.AreEqual("b", stack.Pop());
            Assert.AreEqual(0, stack.Count());
            Assert.IsNull(stack.Pop());
        }

        [Test]
        public void ToArray_AfterWrapAndPop_ReturnsRemainingElements()
        {
            var stack = new LimitedStack<string>(3);

            stack.Push("a");
            stack.Push("b");
            stack.Push("c");
            stack.Push("d");

            stack.Pop();
            stack.Pop();

            CollectionAssert.AreEqual(new[] { "b" }, stack.ToArray());
        }

        [Test]
        public void ToArray_AfterWrap_ReturnsElementsInOrder()
        {
            var stack = new LimitedStack<string>(3);

            stack.Push("a");
            stack.Push("b");
            stack.Push("c");
            stack.Push("d");
            stack.Push("e");

            CollectionAssert.AreEqual(new[] { "c", "d", "e" }, stack.ToArray());
        }

        [Test]
        public void ToArray_WhenEmpty_ReturnsEmptyArray()
        {
            var stack = new LimitedStack<string>(3);

            Assert.IsEmpty(stack.ToArray());
        }

        [Test]
        public void Clear_ResetsStackForReuse()
        {
            var stack = new LimitedStack<string>(2);

            stack.Push("a");
            stack.Push("b");
            stack.Push("c");

            stack.Clear();

            Assert.AreEqual(0, stack.Count());
            Assert.IsNull(stack.Peek());
            Assert.IsEmpty(stack.ToArray());

            stack.Push("d");

            Assert.AreEqual(1, stack.Count());
            Assert.AreEqual("d", stack.Pop());
        }

        [Test]
        public void ZeroSizeStack_IgnoresAllOperations()
        {
            var stack = new LimitedStack<string>(0);

            stack.Push("a");

            Assert.AreEqual(0, stack.Count());
            Assert.IsNull(stack.Peek());
            Assert.IsNull(stack.Pop());
            Assert.IsEmpty(stack.ToArray());
        }
    }
}
