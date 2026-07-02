using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace UniState
{
    public static class DisposableListExtensions
    {
        public static void Add(this List<IDisposable> disposables, Action action)
            => disposables.Add(new DisposableAction(action));

        public static void Add(this List<IDisposable> disposables, params IDisposable[] newDisposables) =>
            disposables.AddRange(newDisposables);

        public static List<IDisposable> ThenAdd(this List<IDisposable> disposables, IDisposable disposable)
        {
            disposables.Add(disposable);

            return disposables;
        }

        public static List<IDisposable> ThenAdd(this List<IDisposable> disposables, Action action)
        {
            disposables.Add(action);

            return disposables;
        }

        public static void Dispose(this List<IDisposable> disposables)
        {
            if (disposables == null || disposables.Count == 0)
            {
                return;
            }

            List<Exception> exceptions = null;

            for (var i = disposables.Count - 1; i >= 0; i--)
            {
                try
                {
                    disposables[i]?.Dispose();
                }
                catch (Exception e)
                {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(e);
                }
            }

            if (exceptions == null)
            {
                return;
            }

            if (exceptions.Count == 1)
            {
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            }

            throw new AggregateException("One or more disposables failed.", exceptions);
        }
    }
}