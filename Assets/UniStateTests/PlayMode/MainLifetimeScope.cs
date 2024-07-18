using UniState;
using UniStateTests.PlayMode.States;
using VContainer;
using VContainer.Unity;

namespace UniStateTests
{
    public class MainLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ITypeResolver, VContainerTypeResolver>(Lifetime.Singleton);

            // Temp:
            builder.Register<Test1State>(Lifetime.Scoped);
            builder.Register<Test2State>(Lifetime.Scoped);
            builder.Register<Test3StateAbstract, Test3State>(Lifetime.Scoped);
            builder.Register<ITest4State, Test4State>(Lifetime.Scoped);
        }
    }
}