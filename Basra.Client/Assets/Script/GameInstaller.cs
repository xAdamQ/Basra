using UnityEngine;
using Zenject;

namespace Basra.Client
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<Room.User>().AsSingle();
            Container.Bind<Room.IRoomManager>().To<Room.RoomManager>();

            //Container.Bind<Contract>().To<Imp>();

            //WithId(string id) options lets you assign an id for the binding and you can use it while injecting
            //usful if you have more than implementation for a contract

            //FromNew() crate the impl by the new keyword
            //Container.Bind<Player>().FromNew();

            //FromInstance(object) get the imp from instance, maybe the same across program(singleton)?
            //Container.Bind<IPlayer>().FromInstance(new Player());

            //FromFactory(fac)
            //by implementing the factory interface
            //useful when you have long configure logic and more depencies for your dependancy

            ////scope

            //AsTransient() new per injection

            //AsCached()
            //AsSingle()
            //the different between them is:
            //Container.Bind<Room.User>()
            //Container.Bind<Room.IUser>().To<User>()
            //in single who ask for User or IUser will get the same user instance
            //in cached they will get 2


            //WithAruments()


            //When()
            //e.g. When(c=>c.ObjectType==typeof(PlayerX))


            //WhenInjectedInto
            //specify the classes who can inject this


            //NonLazy()
            //create instance even before injecting

            AnotherInstaller.Install(Container);

        }
    }

    public class AnotherInstaller : Installer<AnotherInstaller/*this type is used for the ststic method Install, which calls InstallBindings*/>
    {
        public override void InstallBindings()
        {
            throw new System.NotImplementedException();
        }
    }
}
