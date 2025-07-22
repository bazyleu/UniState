using System;
using Reflex.Core;

namespace UniState
{
	public class ReflexTypeResolver : ITypeResolver
	{
		private readonly Container _container;

		public ReflexTypeResolver(Container container)
		{
			_container = container;
		}

		public object Resolve(Type type) => _container.Resolve(type);
	}
}
