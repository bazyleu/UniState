using System;

namespace UniState
{
    public class LimitedStack<T>
    {
        private readonly T[] _items;
        private readonly int _maxSize;
        private int _topIndex = 0;
        private int _bottomIndex = 0;

        private bool HasSize => _maxSize > 0;

        public int MaxSize => _maxSize;

        public LimitedStack(int maxSize)
        {
            _maxSize = maxSize;
            _items = maxSize > 0 ? new T[maxSize] : Array.Empty<T>();
        }

        public int Count() => _topIndex - _bottomIndex;

        public T Push(T element)
        {
            if (!HasSize)
            {
                return element;
            }

            if (Count() == _maxSize)
            {
                _bottomIndex++;
            }

            _topIndex++;
            _items[(_topIndex-1)%_maxSize] = element;

            return element;
        }

        public T Peek()
        {
            if (!HasSize)
            {
                return default;
            }

            return _topIndex != _bottomIndex ? _items[(_topIndex-1)%_maxSize] : default;
        }

        public T Pop()
        {
            if (!HasSize)
            {
                return default;
            }

            var result = Peek();

            if (_topIndex != _bottomIndex)
            {
                _items[(_topIndex-1)%_maxSize] = default;
                _topIndex--;
            }

            return result;
        }

        public void Clear()
        {
            if (HasSize)
            {
                Array.Clear(_items, 0, _maxSize);
            }

            _topIndex = 0;
            _bottomIndex = 0;
        }

        public T[] ToArray()
        {
            var count = Count();

            if (count == 0)
            {
                return Array.Empty<T>();
            }

            var res = new T[count];

            for (var i = 0; i < count; i++)
            {
                res[i] = _items[(_bottomIndex + i) % _maxSize];
            }

            return res;
        }
    }
}
