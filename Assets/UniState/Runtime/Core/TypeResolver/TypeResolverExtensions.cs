using System.Runtime.CompilerServices;

namespace UniState
{
    public static class TypeResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Resolve<T>(this ITypeResolver resolver) => (T)resolver.Resolve(typeof(T));
    }
}