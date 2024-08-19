using System;
using System.Collections.Generic;

namespace UniState
{
    public static class DisposableListExtensions
    {
        public static void Add(this List<IDisposable> disposables, Action action)
            => disposables.Add(new DisposableAction(action));

        public static void Dispose(this List<IDisposable> disposables)
        {
            if (disposables?.Count > 0)
            {
                for (var i = disposables.Count - 1; i >= 0; i--)
                {
                    disposables[i]?.Dispose();
                }
            }
        }
    }
}