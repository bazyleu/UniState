#if UNISTATE_VCONTAINER_SUPPORT

namespace UniState
{
    public static class VContainerUniStateExtensions
    {
        public static ITypeResolver ToTypeResolver(this VContainer.IObjectResolver objectResolver) =>
            new VContainerTypeResolver(objectResolver);
    }
}

#endif