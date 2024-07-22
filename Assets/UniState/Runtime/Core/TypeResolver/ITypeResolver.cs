using System;

namespace UniState
{
    public interface ITypeResolver
    {
        Object Resolve(Type type);
    }
}