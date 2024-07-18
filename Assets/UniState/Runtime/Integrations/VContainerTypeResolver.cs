using System;
using VContainer;

namespace UniState
{
    public class VContainerTypeResolver : ITypeResolver
    {
        private readonly IObjectResolver _resolver;

        public VContainerTypeResolver(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public Object Resolve(Type type) => _resolver.Resolve(type);
    }
}