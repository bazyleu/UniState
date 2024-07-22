using UniStateTests.PlayMode.States;
using Zenject;

namespace UniStateTests.PlayMode
{
    public class ZenjectTestInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SimpleStateMachine>().ToSelf().AsTransient();
            Container.Bind<Test1State>().ToSelf().AsTransient();
            Container.Bind<Test2State>().ToSelf().AsTransient();
            Container.Bind<Test3StateAbstract>().To<Test3StateAbstract>().AsTransient();
            Container.Bind<ITest4State>().To<Test4State>().AsTransient();
        }
    }
}