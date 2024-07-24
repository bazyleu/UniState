using System.Collections.Generic;

namespace UniState
{
    public class LimitedStack<T>
    {
        private readonly List<T> _list;

        public LimitedStack(int maxSize)
        {
            _list = new List<T>(maxSize);
        }

        public T Push(T element)
        {
            if (_list.Capacity <= _list.Count)
            {
                _list.RemoveAt(0);
            }

            _list.Add(element);

            return element;
        }

        public T Peek() => _list.Count > 0 ? _list[^1] : default;

        public T Pop()
        {
            var result = Peek();

            if (_list.Count > 0)
            {
                _list.RemoveAt(_list.Count - 1);
            }

            return result;
        }
    }
}