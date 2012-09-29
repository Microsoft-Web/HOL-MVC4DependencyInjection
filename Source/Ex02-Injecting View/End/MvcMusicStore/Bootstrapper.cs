using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using MvcMusicStore.Services;
using MvcMusicStore.Controllers;
using MvcMusicStore.Factories;

namespace MvcMusicStore
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new Unity.Mvc3.UnityDependencyResolver(container));

            IDependencyResolver resolver = DependencyResolver.Current;

            IDependencyResolver newResolver = new Factories.UnityDependencyResolver(container, resolver);

            DependencyResolver.SetResolver(newResolver);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IStoreService, StoreService>();
            container.RegisterType<IController, StoreController>("Store");

            container.RegisterInstance<IMessageService>(new MessageService
            {
                Message = "You are welcome to our Web Camps Training Kit!",
                ImageUrl = "/Content/Images/webcamps.png"
            });

            container.RegisterType<IViewPageActivator, CustomViewPageActivator>(new InjectionConstructor(container));

            return container;
        }
    }
}