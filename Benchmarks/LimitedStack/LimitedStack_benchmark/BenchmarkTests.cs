using BenchmarkDotNet.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniState;


[MemoryDiagnoser]
[SimpleJob(launchCount: 3, warmupCount: 10, iterationCount: 30)]

public class BenchmarkTests
{
    public class Coordinate(float x, float y)
    {
        public float x = x;
        public float y = y;
    }
    
    
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
    
    public void VerifyStackPushCoordinate(ILimitedStack<Coordinate> stack)
    {
        var maxSize = stack.Capacity();
        for (var i = 0; i < maxSize * 2; i++)
        {
            var temp = new Coordinate(i, i);
            stack.Push(temp);
        }

        for (var i = maxSize * 2 - 1; i >= maxSize; i--)
        {
            var expected = new Coordinate(i, i);
            var temp = stack.Pop();
            Assert.AreEqual(expected.x, temp.x);
            Assert.AreEqual(expected.y, temp.y);
        }
    }
    

    private int _maxSize = 1000;
    
    [Benchmark]
    public void OriginalPushInt()
    {
        var stackOriginal = new LimitedStackOriginal<int>(_maxSize);
        VerifyStackPushInt(stackOriginal);
    }

    [Benchmark]
    public void ArrayPushInt()
    {
        var stackArray = new LimitedStack_array<int>(_maxSize);
        VerifyStackPushInt(stackArray);
    }
    
    [Benchmark]
    public void SlidingArrayPushInt()
    {
        var stackArray = new LimitedStack_sliding_array<int>(_maxSize);
        VerifyStackPushInt(stackArray);
    }
    
    [Benchmark]
    public void OriginalPushCoord()
    {
        var stackOriginal = new LimitedStackOriginal<Coordinate>(_maxSize);
        VerifyStackPushCoordinate(stackOriginal);
    }

    [Benchmark]
    public void ArrayPushCoord()
    {
        var stackArray = new LimitedStack_array<Coordinate>(_maxSize);
        VerifyStackPushCoordinate(stackArray);
    }
    
    [Benchmark]
    public void SlidingArrayPushCoord()
    {
        var stackArray = new LimitedStack_sliding_array<Coordinate>(_maxSize);
        VerifyStackPushCoordinate(stackArray);
    }
}