#if UNISTATE_ZENJECT_SUPPORT

using System;
using Zenject;

namespace UniState
{
    public class ZenjectTypeResolver : ITypeResolver
    {
        private readonly DiContainer _container;

        public ZenjectTypeResolver(DiContainer container)
        {
            _container = container;
        }

        public object Resolve(Type type) => _container.Resolve(type);
    }
}

#endif