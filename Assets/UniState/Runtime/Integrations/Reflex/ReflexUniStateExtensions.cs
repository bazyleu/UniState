#if UNISTATE_REFLEX_SUPPORT

namespace UniState
{
    public static class ReflexUniStateExtensions
    {
        public static ITypeResolver ToTypeResolver(this Reflex.Core.Container container)
        {
            return new ReflexTypeResolver(container);
        }
    }
}

#endif