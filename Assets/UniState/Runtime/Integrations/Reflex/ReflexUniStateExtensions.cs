namespace UniState
{
	public static class ReflexUniStateExtensions
	{
		public static ITypeResolver ToTypeResolver(this Reflex.Core.Container container) =>
			new ReflexTypeResolver(container);
	}
}
