using System.Collections.Generic;

namespace UniState
{
    public class LimitedStack_array<T>  : ILimitedStack<T>
    {
        private readonly  T[] _list;
        private int _currentSize = 0;
        private int _maxSize;

        public LimitedStack_array(int maxSize)
        {
            _maxSize = maxSize;
            _list = new T[_maxSize];
        }

        public T Push(T element)
        {
            if (_currentSize == _maxSize)
            {
                for (var i = 0; i < _currentSize - 1; i++)
                {
                    _list[i] = _list[i + 1];
                }

                _currentSize -= 1;
            }

            _currentSize++;
            _list[_currentSize-1] = element;
            return element;
        }

        public T Peek() => _currentSize > 0 ? _list[_currentSize-1] : default(T);

        public T Pop()
        {
            var result = Peek();

            if (_currentSize > 0)
            {
                _list[_currentSize-1] = default(T);
                _currentSize -= 1;
            }

            return result;
        }

        public int Capacity() => _maxSize;
    }
}