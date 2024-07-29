using System;
using System.Collections.Generic;

namespace UniState
{
    public static class DisposableListExtensions
    {
        public static void Add(this List<IDisposable> disposables, Action action)
            => disposables.Add(new DisposableAction(action));
    }
}