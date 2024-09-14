#if UNISTATE_ZENJECT_SUPPORT

namespace UniState
{
    public static class ZenjectUniStateExtensions
    {
        public static ITypeResolver ToTypeResolver(
            this Zenject.DiContainer container, bool allowAutoBindings = true)
            => new ZenjectTypeResolver(container, allowAutoBindings);
    }
}

#endif