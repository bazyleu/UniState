#if UNISTATE_ZENJECT_SUPPORT

namespace UniState
{
    public static class ZenjectUniStateExtensions
    {
        public static ITypeResolver ToTypeResolver(this Zenject.DiContainer container) =>
            new ZenjectTypeResolver(container);
    }
}

#endif