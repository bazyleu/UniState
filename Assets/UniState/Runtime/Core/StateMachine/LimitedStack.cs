using System;

namespace UniState
{
    public class LimitedStack<T>
    {
        private readonly  T[] _items;
        private readonly int _maxSize;
        private int _topIndex = 0;
        private int _bottomIndex = 0;

        public LimitedStack(int maxSize)
        {
            _maxSize = maxSize;
            _items = new T[_maxSize];
        }

        public int Count() => _topIndex - _bottomIndex;

        public T Push(T element)
        {
            if (Count() == _maxSize)
            {
                _bottomIndex++;
            }

            _topIndex++;
            _items[(_topIndex-1)%_maxSize] = element;

            return element;
        }

        public T Peek() => _topIndex != _bottomIndex ? _items[(_topIndex-1)%_maxSize] : default;

        public T Pop()
        {
            var result = Peek();

            if (_topIndex != _bottomIndex)
            {
                _items[(_topIndex-1)%_maxSize] = default;
                _topIndex--;
            }

            return result;
        }

        public T[] ToArray()
        {
            var res = new T[Count()];

            if (_bottomIndex == _topIndex)
            {
                return Array.Empty<T>();
            }

            if (_topIndex < _maxSize)
            {
                Array.Copy(_items, res, Count());
            }
            else
            {
                var remainderSize = _maxSize - (_bottomIndex % _maxSize);

                Array.Copy(
                    sourceArray: _items,
                    sourceIndex: _bottomIndex % _maxSize,
                    destinationArray: res,
                    destinationIndex: 0,
                    length: remainderSize
                );

                Array.Copy(
                    sourceArray: _items,
                    sourceIndex: 0,
                    destinationArray: res,
                    destinationIndex: remainderSize,
                    length: _topIndex % _maxSize
                );
            }

            return res;
        }
    }
}