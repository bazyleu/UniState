using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniState;


[TestClass]
public class RegularTests
{
    public void VerifyStackPushInt(ILimitedStack<int> stack)
    {
        var maxSize = stack.Capacity();
        for (var i = 0; i < maxSize * 2; i++)
        {
            stack.Push(i);
        }

        for (var i = maxSize * 2 - 1; i >= maxSize; i--)
        {
            var temp = stack.Pop();
            Assert.AreEqual(i, temp);
        }
    }
    
    [TestMethod]
    public void Test_push()
    {
        int maxSize = 5;
        var stackOriginal = new LimitedStackOriginal<int>(maxSize);
        VerifyStackPushInt(stackOriginal);

        var stackArray = new LimitedStack_array<int>(maxSize);
        VerifyStackPushInt(stackArray);
        
        var slidingStackArray = new LimitedStack_sliding_array<int>(maxSize);
        VerifyStackPushInt(slidingStackArray);
    }
}