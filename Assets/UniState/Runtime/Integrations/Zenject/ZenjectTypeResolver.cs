#if UNISTATE_ZENJECT_SUPPORT

using System;
using Zenject;

namespace UniState
{
    public class ZenjectTypeResolver : ITypeResolver
    {
        private readonly DiContainer _container;
        private readonly bool _allowAutoBindings;

        public ZenjectTypeResolver(DiContainer container, bool allowAutoBindings)
        {
            _container = container;
            _allowAutoBindings = allowAutoBindings;
        }

        public object Resolve(Type type)
        {
            if (_allowAutoBindings && !type.IsAbstract && !type.IsInterface && !_container.HasBinding(type))
            {
                _container.BindState(type);
            }

            return _container.Resolve(type);
        }
    }
}

#endif