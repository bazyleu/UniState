namespace UniState
{
    public class LimitedStack<T>
    {
        private readonly  T[] _items;
        private readonly int _maxSize;
        private int _topIndex = 0;
        private int _bottomIndex = 0;
        // top list index: (_topIndex - 1) % _maxSize
        // isNotEmpty: _topIndex != _bottomIndex

        public LimitedStack(int maxSize)
        {
            _maxSize = maxSize;
            _items = new T[_maxSize];
        }
        
        public int Count() => _topIndex - _bottomIndex;

        public T Push(T element)
        {
            // If max capacity reached - remove one from bottom
            // no need to clear as it will be replaced right away
            if (Count() == _maxSize)
            {
                _bottomIndex++;
            }

            _topIndex++;
            _items[(_topIndex-1)%_maxSize] = element;
            return element;
        }

        public T Peek() => _topIndex != _bottomIndex ? _items[(_topIndex-1)%_maxSize] : default(T);

        public T Pop()
        {
            var result = Peek();

            if (_topIndex != _bottomIndex)
            {
                _items[(_topIndex-1)%_maxSize] = default(T);
                _topIndex--;
            }

            return result;
        }
        
        public T[] ToArray()
        {
            var res = new T[Count()];
            if (_bottomIndex == _topIndex)
            {
                return [];
            }
            else if (_topIndex < _maxSize)
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