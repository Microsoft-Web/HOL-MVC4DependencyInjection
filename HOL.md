<a name="HOLTop" />

# ASP.NET MVC 4 Dependency Injection #
---

<a name="Overview" />
## Overview ##

>**Note:** This Hands-on Lab assumes you have basic knowledge of **ASP.NET MVC** and **ASP.NET MVC 4 filters**. If you have not used **ASP.NET MVC 4 filters** before, we recommend you to go over **ASP.NET MVC Custom Action Filters** Hands-on Lab.

In **Object Oriented Programming** paradigm, objects work together in a collaboration model where there are contributors and consumers. Naturally, this communication model generates dependencies between objects and components, becoming difficult to manage when complexity increases.

![Class dependencies and model complexity](./images/Class_dependencies_and_model_complexity.png?raw=true "Class dependencies and model complexity")
 
_Class dependencies and model complexity_

You have probably heard about the **Factory Pattern** and the separation between the interface and the implementation using services, where the client objects are often responsible for service location.

The Dependency Injection pattern is a particular implementation of Inversion of Control. **Inversion of Control (IoC)** means that objects do not create other objects on which they rely to do their work. Instead, they get the objects that they need from an outside source (for example, an xml configuration file).

**Dependency Injection (DI)** means that this is done without the object intervention, usually by a framework component that passes constructor parameters and set properties.

### The Dependency Injection (DI) Design Pattern ###

At a high level, the goal of Dependency Injection is that a client class (e.g. _the golfer_) needs something that satisfies an interface (e.g. _IClub_). It doesn't care what the concrete type is  (e.g. _WoodClub, IronClub, WedgeClub_ or _PutterClub_), it wants someone else to handle that (e.g. a good _caddy_). The Dependency Resolver in ASP.NET MVC can allow you to register your dependency logic somewhere else (e.g. a container or a _bag of clubs_).

![Dependency Injection diagram](./images/dependency-injection-golf.png?raw=true "Dependency Injection illustration")
 
_Dependency Injection - Golf analogy_

The advantages of using Dependency Injection pattern and Inversion of Control are the following:

- Reduces class coupling

- Increases code reusing

- Improves code maintainability

- Improves application testing

 
>**Note:** Dependency Injection is sometimes compared with Abstract Factory Design Pattern, but there is a slight difference between both approaches. DI has a Framework working behind to solve dependencies by calling the factories and the registered services.

Now that you understand the Dependency Injection Pattern, you will learn throughout this lab how to apply it in ASP.NET MVC 4. You will start using Dependency Injection in the **Controllers** to include a database access service. Next, you will apply Dependency Injection to the **Views** to consume a service and show information. Finally, you will extend the DI to ASP.NET MVC 4 Filters, injecting a custom action filter in the solution.

In this Hands-on Lab, you will learn how to:

- Integrate ASP.NET MVC 4 with Unity for Dependency Injection using NuGet Packages

- Use Dependency Injection inside an ASP.NET MVC Controller

- Use Dependency Injection inside an ASP.NET MVC View

- Use Dependency Injection inside an ASP.NET MVC Action Filter

 
>**Note:** This Lab is using Unity.Mvc3 NuGet Package for dependency resolution, but it is possible to adapt any Dependency Injection Framework to work with ASP.NET MVC 4.

<a name="Prerequisites" />
### Prerequisites ###

You must have the following items to complete this lab:

- [Microsoft Visual Studio Express 2012 for Web](http://www.microsoft.com/visualstudio/eng/products/visual-studio-express-for-web) or superior (read [Appendix A](#AppendixA) for instructions on how to install it).

<a name="Setup" /> 
### Setup ###

**Installing Code Snippets**

For convenience, much of the code you will be managing along this lab is available as Visual Studio code snippets. To install the code snippets run **.\Source\Assets\CodeSnippets.vsi** file.

If you are not familiar with the Visual Studio Code Snippets, and want to learn how to use them, you can refer to the appendix from this document "[Appendix C: Using Code Snippets](#AppendixC)".

---

<a name="Exercises" />
## Exercises ##

This Hands-On Lab is comprised by the following exercises:

1. [Exercise 1: Injecting a Controller](#Exercise1)

1. [Exercise 2: Injecting a View](#Exercise2)

1. [Exercise 3: Injecting Filters](#Exercise3)
 
> **Note:** Each exercise is accompanied by an **End** folder containing the resulting solution you should obtain after completing the exercises. You can use this solution as a guide if you need additional help working through the exercises.

Estimated time to complete this lab: **30 minutes**.

<a name="Exercise1" />
### Exercise 1: Injecting a Controller ###

In this exercise, you will learn how to use Dependency Injection in ASP.NET MVC Controllers by integrating Unity using a NuGet Package. For that reason, you will include services into your MvcMusicStore controllers to separate the logic from the data access. The services will create a new dependence in the controller constructor, which will be resolved using Dependency Injection with the help of **Unity**.

This approach will show you how to generate less coupled applications, which are more flexible and easier to maintain and test. You will also learn how to integrate ASP.NET MVC with Unity.

#### About StoreManager Service ####

The MVC Music Store provided in the begin solution now includes a service that manages the Store Controller data named **StoreService**. Below you will find the Store Service implementation. Note that all the methods return Model entities.

````C#
namespace MvcMusicStore.Controllers
{    
    using System.Web.Mvc;
    using MvcMusicStore.Filters;
    using MvcMusicStore.Services;

    [MyNewCustomActionFilter(Order = 1)]
    [CustomActionFilter(Order = 2)]
    public class StoreController : Controller
    {
        private IStoreService service;

        public StoreController(IStoreService service)
        {
            this.service = service;
        }

        // GET: /Store/
        public ActionResult Details(int id)
        {
            var album = this.service.GetAlbum(id);
            if (album == null)
            {
                return this.HttpNotFound();
            }

            return this.View(album);
        }

        public ActionResult Browse(string genre)
        {
            // Retrieve Genre and its Associated Albums from database
            var genreModel = this.service.GetGenreByName(genre);

            return this.View(genreModel);
        }

        public ActionResult Index()
        {
            var genres = this.service.GetGenres();

            return this.View(genres);
        }

        // GET: /Store/GenreMenu
        public ActionResult GenreMenu()
        {
            var genres = this.service.GetGenres();

            return this.PartialView(genres);
        }
    }
}
````

 **StoreController** from the begin solution now consumes **StoreService**. All the data references were removed from **StoreController**, and now possible to modify the current data access provider without changing any method that consumes **StoreService**.

You will find below that the **StoreController** implementation has a dependency with  **StoreService** inside the class constructor.

> **Note:** The dependency introduced in this exercise is related to **Inversion of Control** (IoC).
>
> The **StoreController** class constructor receives an **IStoreService** type parameter, which is essential to perform service calls from inside the class. However, **StoreController** does not implement the default constructor (with no parameters) that any controller must have to work with ASP.NET MVC.
>
> To resolve the dependency, the controller has to be created by an abstract factory (a class that returns any object of the specified type).

````C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.ViewModels;
using MvcMusicStore.Models;
using MvcMusicStore.Services;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        private IStoreService service;

        public StoreController(IStoreService service)
        {
            this.service = service;
        }

        //
        // GET: /Store/
        public ActionResult Index()
        {
            // Create list of genres
            var genres = this.service.GetGenreNames();

            // Create your view model
            var viewModel = new StoreIndexViewModel
            {
                Genres = genres.ToList(),
                NumberOfGenres = genres.Count()
            };

            return View(viewModel);
        }

        //
        // GET: /Store/Browse?genre=Disco
        public ActionResult Browse(string genre)
        {
            var genreModel = this.service.GetGenreByName(genre);

            var viewModel = new StoreBrowseViewModel()
            {
                Genre = genreModel,
                Albums = genreModel.Albums.ToList()
            };

            return View(viewModel);
        }

        //
        // GET: /Store/Details/5
        public ActionResult Details(int id)
        {
            var album = this.service.GetAlbum(id);

            return View(album);
        }
    }
}
````

>**Note:** You will get an error when the class tries to create the StoreController without sending the service object, as there is no parameterless constructor declared.

<a name="Ex1Task1" />
#### Task 1 - Running the Application ####

In this task, you will run the Begin application, which includes the service into the Store Controller that separates the data access from the application logic.

When running the application, you will receive an exception, as the controller service is not passed as a parameter by default:

1. Open the **Begin** solution located in **Source\Ex01-Injecting Controller\Begin**.

1.	Follow these steps to install the **NuGet** package dependencies.

	1. click the **Project** menu and select **Manage NuGet Packages**.

	1. In the **Manage NuGet Packages** dialog, click **Restore** in order to download missing packages.

	1. Finally, build the solution by clicking **Build** | **Build Solution**.

	>**Note:** One of the advantages of using NuGet is that you don't have to ship all the libraries in your project, reducing the project size. With NuGet Power Tools, by specifying the package versions in the Packages.config file, you will be able to download all the required libraries the first time you run the project. This is why you will have to run these steps after you open an existing solution from this lab.
	
	>For more information, see this article: <http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages>.

1. Press **Ctrl + F5** to run the application without debugging. You will get the error message "**No parameterless constructor defined for this object**":

 	![Error while running ASP.NET MVC Begin Application](./images/Error_while_running_MVC_Begin_Application.png?raw=true "Error while running ASP.NET MVC Begin Application")
 
	_Error while running ASP.NET MVC Begin Application_

1. Close the browser.

In the following steps you will work on the Music Store Solution to inject the dependency this controller needs.

<a name="Ex1Task2" />
#### Task 2 - Including Unity into MvcMusicStore Solution ####

In this task, you will include **Unity.Mvc3** NuGet Package to the solution.

> **Note:** Unity.Mvc3 package was designed for ASP.NET MVC 3, but it is fully compatible with ASP.NET MVC 4.
>
>Unity is a lightweight, extensible dependency injection container with optional support for instance and type interception. It is a general-purpose container for use in any type of .NET application. It provides all the common features found in dependency injection mechanisms including: object creation, abstraction of requirements by specifying dependencies at runtime and flexibility, by deferring the component configuration to the container.


1. Install **Unity.Mvc3** NuGet Package in the **MvcMusicStore** project. To do this, open the **Package Manager Console** from **View** | **Other Windows**.

1. Run the following command.

	````PMC
	Install-Package Unity.Mvc3
	````

	![Installing Unity.Mvc3 NuGet Package](images/installing-unitymvc3-nuget-package.png?raw=true "Installing Unity.Mvc3 NuGet Package")

	_Installing Unity.Mvc3 NuGet Package_

1. Once the **Unity.Mvc3** package is installed, explore the files and folders it automatically adds in order to simplify Unity configuration.

	![Unity.Mvc3 package installed](images/unitymvc3-package-installed.png?raw=true "Unity.Mvc3 package installed")

	_Unity.Mvc3 package installed_


<a name="Ex1Task3" />
####Task 3 - Registering Unity in Global.asax.cs Application_Start ####

In this task, you will update the **Application_Start** method located in **Global.asax.cs** to call the Unity Bootstrapper initializer and then, update the Bootstrapper file registering the Service and Controller you will use for Dependency Injection.

1. Now, you will hook up the Bootstrapper which is the file that initializes the Unity container and Dependency Resolver. To do this, open **Global.asax.cs** and add the following highlighted code within the **Application_Start** method.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex01 - Initialize Unity_)
	<!-- mark: 10 -->
	````C#
	protected void Application_Start()
	  {
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

			Bootstrapper.Initialise();

			AppConfig.Configure();
	  }
	````

1. Open **Bootstrapper.cs** file.

1. Include the following namespaces: **MvcMusicStore.Services** and **MusicStore.Controllers**.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex01 - Bootstrapper Adding Namespaces_)
	<!-- mark: 4-5 -->
	````C#
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	using Unity.Mvc3;
	using MvcMusicStore.Services;
	using MvcMusicStore.Controllers;
	````

1. Replace **BuildUnityContainer** method's content with the following code that registers Store Controller and Store Service.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex01 - Register Store Controller and Service_)
	<!-- mark:3-8 -->
	````C#
	private static IUnityContainer BuildUnityContainer()
	  {
			var container = new UnityContainer();

			container.RegisterType<IStoreService, StoreService>();
			container.RegisterType<IController, StoreController>("Store");        

			return container;
	  }
	````

<a name="Ex1Task4" />
####Task 4 - Running the Application ####

In this task, you will run the application to verify that it can now be loaded after including Unity.

1. Press **F5** to run the application, the application should now load without showing any error message.

	![Running Application with Dependency Injection](images/running-application-with-dependency-injection.png?raw=true "Running Application with Dependency Injection")

	_Running Application with Dependency Injection_

1. Browse to **/Store**. This will invoke **StoreController**, which is now created using **Unity**.

 	![MVC Music Store](./images/MVC_Music_Store.png?raw=true "MVC Music Store")
 
	_MVC Music Store_

1. Close the browser.

In the following exercises you will learn how to extend the Dependency Injection scope to use it inside ASP.NET MVC Views and Action Filters.

<a name="Exercise2" />
###Exercise 2: Injecting a View ###

In this exercise, you will learn how to use Dependency Injection in a view with the new features of ASP.NET MVC 4 for Unity integration. In order to do that, you will call a custom service inside the Store Browse View, which will show a message and an image below.

Then, you will integrate the project with Unity and create a custom dependency resolver to inject the dependencies.

<a name="Ex2Task1" />
#### Task 1 - Creating a View that Consumes a Service ####

In this task, you will create a view that performs a service call to generate a new dependency. The service consists in a simple messaging service included in this solution.

1. Open the **Begin** solution located in the **Source\Ex02-Injecting View\Begin** folder. Otherwise, you might continue using the **End** solution obtained by completing the previous exercise.
	1. If you opened the provided **Begin** solution, you will need to download some missing NuGet packages before continue. To do this, click the **Project** menu and select **Manage NuGet Packages**.
	1. In the **Manage NuGet Packages** dialog, click **Restore** in order to download missing packages.
	1. Finally, build the solution by clicking **Build** | **Build Solution**.

	>**Note:** One of the advantages of using NuGet is that you don't have to ship all the libraries in your project, reducing the project size. With NuGet Power Tools, by specifying the package versions in the Packages.config file, you will be able to download all the required libraries the first time you run the project. This is why you will have to run these steps after you open an existing solution from this lab.
	
	>For more information, see this article: <http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages>.

1. Include the **MessageService.cs** and the **IMessageService.cs** classes located in the **Source\Assets** folder in **/Services**. To do this, right-click **Services** folder and select **Add Existing Item**. Browse to the files' location and include them.

	![Adding Message Service and Service Interface](images/adding-message-service-and-service-interface.png?raw=true "Adding Message Service and Service Interface")

	_Adding Message Service and Service Interface_

	>**Note:** The **IMessageService** interface defines two properties implemented by the **MessageService** class. These properties -**Message** and **ImageUrl**- store the message and the URL of the image to be displayed.

1. Create the folder **/Pages** in the project's root folder, and then add the existing class **MyBasePage.cs** from **Source\Assets**. The base page you will inherit from has the following structure.

	![Pages folder](images/pages-folder.png?raw=true "Pages folder")

	````C#
	namespace MvcMusicStore.Pages
	{
		 using System;
		 using System.Collections.Generic;
		 using System.Linq;
		 using System.Web;
		 using Microsoft.Practices.Unity;
		 using MvcMusicStore.Models;
		 using MvcMusicStore.Services;

		 public class MyBasePage : System.Web.Mvc.WebViewPage<Genre>
		 {
			  [Dependency]
			  public IMessageService MessageService { get; set; }

			  public override void Execute()
			  {
			  }
		 }
	}
	````

1. Open **Browse.cshtml** view from **/Views/Store** folder, and make it inherit from **MyBasePage.cs**.

	<!-- mark: 1 -->
	````C#
	@inherits MvcMusicStore.Pages.MyBasePage
	@{
		 ViewBag.Title = "Browse Albums";
	}
	````

1. In the **Browse** view, add a call to **MessageService** to display an image and a message retrieved by the service.

	<!-- mark: 4-9 -->
	````HTML(C#)
	@inherits MvcMusicStore.Pages.MyBasePage	
	@{
		Viewbag.Title = "Browse Albums";
	}
	<div>
		@this.MessageService.Message
		<br />
		<img alt="@this.MessageService.Message" src="@this.MessageService.ImageUrl" />
	</div>
	...
	````

<a name="Ex2Task2" />
#### Task 2 - Including a Custom Dependency Resolver and a Custom View Page Activator ####

In the previous task, you injected a new dependency inside a view to perform a service call inside it. Now, you will resolve that dependency by implementing the ASP.NET MVC Dependency Injection interfaces **IViewPageActivator** and **IDependencyResolver**. You will include in the solution an implementation of **IDependencyResolver** that will deal with the service retrieval by using Unity. Then, you will include another custom implementation of **IViewPageActivator** interface that will solve the creation of the views.

> **Note:** Since ASP.NET MVC 3, the implementation for Dependency Injection had simplified the interfaces to register services. **IDependencyResolver** and **IViewPageActivator** are part of ASP.NET MVC 3 features for Dependency Injection.
>
>**- IDependencyResolver** interface replaces the previous IMvcServiceLocator. Implementers of IDependencyResolver must return an instance of the service or a service collection.
>
>````C#
>	public interface IDependencyResolver {
>		object GetService(Type serviceType);
>		IEnumerable<object> GetServices(Type serviceType);
>	}
>````
>
> **- IViewPageActivator** interface provides more fine-grained control over how view pages are instantiated via dependency injection. The classes that implement **IViewPageActivator** interface can create view instances using context information.
>
>````C#
>	public interface IViewPageActivator {
>		object Create(ControllerContext controllerContext, Type type);
>	}
>````

1. Create the /**Factories** folder in the project's root folder.

1. Include **CustomViewPageActivator.cs** to your solution from **/Sources/Assets/** to **Factories** folder. To do that, right-click  the **/Factories** folder, select **Add | Existing Item** and then select **CustomViewPageActivator.cs**. This class implements the **IViewPageActivator** interface to hold the Unity Container.
 
	````C#
	namespace MvcMusicStore.Factories
	{
		 using System;
		 using System.Collections.Generic;
		 using System.Linq;
		 using System.Web;
		 using System.Web.Mvc;
		 using Microsoft.Practices.Unity;

		 public class CustomViewPageActivator : IViewPageActivator
		 {
			  private IUnityContainer container;

			  public CustomViewPageActivator(IUnityContainer container)
			  {
					this.container = container;
			  }

			  public object Create(ControllerContext controllerContext, Type type)
			  {
					return this.container.Resolve(type);
			  }
		 }
	}
	````

	>**Note:** **CustomViewPageActivator** is responsible for managing the creation of a view by using a Unity container.

1. Include **UnityDependencyResolver.cs** file from **/Sources/Assets** to  **/Factories** folder. To do that, right-click  the **/Factories** folder, select **Add | Existing Item** and then select **UnityDependencyResolver.cs** file.

	````C#
	namespace MvcMusicStore.Factories
	{
		 using System;
		 using System.Collections.Generic;
		 using System.Linq;
		 using System.Web;
		 using System.Web.Mvc;
		 using Microsoft.Practices.Unity;

		 public class UnityDependencyResolver : IDependencyResolver
		 {
			  private IUnityContainer container;

			  private IDependencyResolver resolver;

			  public UnityDependencyResolver(IUnityContainer container, IDependencyResolver resolver)
			  {
					this.container = container;
					this.resolver = resolver;
			  }

			  public object GetService(Type serviceType)
			  {
					try
					{
						 return this.container.Resolve(serviceType);
					}
					catch
					{
						 return this.resolver.GetService(serviceType);
					}
			  }

			  public IEnumerable<object> GetServices(Type serviceType)
			  {
					try
					{
						 return this.container.ResolveAll(serviceType);
					}
					catch
					{
						 return this.resolver.GetServices(serviceType);
					}
			  }
		 }
	}
	````

	>**Note:** **UnityDependencyResolver** class is a custom DependencyResolver for Unity. When a service cannot be found inside the Unity container, the base resolver is invocated.  

	
In the following task both implementations will be registered to let the model know the location of the services and the views.

<a name="Ex2Task3" />
#### Task 3 - Registering for Dependency Injection within Unity container ####

In this task, you will put all the previous things together to make Dependency Injection work.

Up to now your solution has the following elements:

- A **Browse** View that inherits from **MyBaseClass** and consumes **MessageService**.

- An intermediate class -**MyBaseClass**- that has dependency injection declared for the service interface.

- A service - **MessageService** - and its interface **IMessageService**.

- A custom dependency resolver for Unity - **UnityDependencyResolver** - that deals with service retrieval.

- A View Page activator - **CustomViewPageActivator ** that creates the page.

 
To inject **Browse** View, you will now register the custom dependency resolver in the Unity container.

1. Open **Bootstrapper.cs** file.

1. Register an instance of **MessageService** into the Unity container to initialize the service:

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex02 - Register Message Service_)
	<!-- mark: 8-12 -->
	````C#
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
		...
	}
	````

1. Add a reference to **MvcMusicStore.Factories** namespace.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex02 - Factories Namespace_)
	<!-- mark: 6 -->
	````C#
	using System.Web.Mvc; 
	using Microsoft.Practices.Unity; 
	using Unity.Mvc3; 
	using MvcMusicStore.Services; 
	using MvcMusicStore.Controllers; 
	using MvcMusicStore.Factories; 
	````

1. Register **CustomViewPageActivator** as a View Page activator into the Unity container:

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex02 - Register CustomViewPageActivator_)
	<!-- mark: 14 -->
	````C#
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
	````

1. Replace ASP.NET MVC 4 default dependency resolver with an instance of **UnityDependencyResolver**. To do this, replace **Initialise** method content with the following code:

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex02 - Update Dependency Resolver_)
	<!-- mark: 3-11 -->
	````C#
	public static void Initialise()
	  {
			var container = BuildUnityContainer();

			DependencyResolver.SetResolver(new Unity.Mvc3.UnityDependencyResolver(container));

			IDependencyResolver resolver = DependencyResolver.Current;

			IDependencyResolver newResolver = new Factories.UnityDependencyResolver(container, resolver);

			DependencyResolver.SetResolver(newResolver);
	  }
	````

	>**Note:** ASP.NET MVC provides a default dependency resolver class. To work with custom dependency resolvers as the one we have created for unity, this resolver has to be replaced.

<a name="Ex2Task4" />
#### Task 4 - Running the Application ####

In this task, you will run the application to verify that the Store Browser consumes the service and shows the image and the message retrieved:

1. Press **F5** to run the application.

1. Click **Rock** within the Genres Menu and see how the **MessageService** was injected to the view and loaded the welcome message and the image. In this example, we are entering to "**Rock**":

 	![MVC Music Store - View Injection](./images/Music_Store.png?raw=true "MVC Music Store - View Injection")
 
	_MVC Music Store - View Injection_

1. Close the browser.

<a name="Exercise3" />
### Exercise 3: Injecting Action Filters ###

In the previous Hands-On lab **Custom Action Filters** you have worked with filters customization and injection. In this exercise, you will learn how to inject filters with Dependency Injection by using the Unity container. To do that, you will add to the Music Store solution a custom action filter that will trace the activity of the site.

<a name="Ex3Task1" />
#### Task 1 - Including the Tracking Filter in the Solution ####

In this task, you will include in the Music Store a custom action filter to trace events. As custom action filter concepts are already treated in the previous Lab "Custom Action Filters", you will just include the filter class from the Assets folder of this lab, and then create a Filter Provider for Unity:

1. Open the **Begin** solution located in the **Source\Ex03 - Injecting Action Filter\Begin** folder. Otherwise, you might continue using the **End** solution obtained by completing the previous exercise.
	1. If you opened the provided **Begin** solution, you will need to download some missing NuGet packages before continue. To do this, click the **Project** menu and select **Manage NuGet Packages**. 

	1. In the **Manage NuGet Packages** dialog, click **Restore** in order to download missing packages. 

	1. Finally, build the solution by clicking **Build** | **Build Solution**. 

	>**Note:** One of the advantages of using NuGet is that you don't have to ship all the libraries in your project, reducing the project size. With NuGet Power Tools, by specifying the package versions in the Packages.config file, you will be able to download all the required libraries the first time you run the project. This is why you will have to run these steps after you open an existing solution from this lab.
	
	>For more information, see this article: <http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages>.

1. Include **TraceActionFilter.cs** file from **/Sources/Assets** to **/Filters** folder.

	````C#
	namespace MvcMusicStore.Filters
	{
		 using System;
		 using System.Collections.Generic;
		 using System.Linq;
		 using System.Web;
		 using System.Web.Mvc;

		 public class TraceActionFilter : IActionFilter
		 {
			  public void OnActionExecuted(ActionExecutedContext filterContext)
			  {
					filterContext.HttpContext.Trace.Write("OnActionExecuted");
					filterContext.HttpContext.Trace.Write("Action " + filterContext.ActionDescriptor.ActionName);
					filterContext.HttpContext.Trace.Write("Controller " + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
			  }

			  public void OnActionExecuting(ActionExecutingContext filterContext)
			  {
					filterContext.HttpContext.Trace.Write("OnActionExecuting");
					filterContext.HttpContext.Trace.Write("Action " + filterContext.ActionDescriptor.ActionName);
					filterContext.HttpContext.Trace.Write("Controller " + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
			  }
		 }
	}
	````

	>**Note:** This custom action filter performs ASP.NET tracing. You can check "ASP.NET MVC 4 local and Dynamic Action Filters" Lab for more reference.

1. Add the empty class **FilterProvider.cs** to the project in the folder **/Filters.**

1. Add the **System.Web.Mvc** and **Microsoft.Practices.Unity** namespaces in **FilterProvider.cs**.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex03 - Filter Provider Adding Namespaces_)
	<!-- mark: 5-6 -->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;

	namespace MvcMusicStore.Filters
	{
		 public class FilterProvider
		 {
		 }
	}
	````

1. Make the class inherit from **IFilterProvider** Interface.

	<!-- mark: 3 -->
	````C#
	namespace MvcMusicStore.Filters
	{
		 public class FilterProvider : IFilterProvider
		 {
		 }
	}
	````

1. Add a **IUnityContainer** property in the **FilterProvider** class, and then create a class constructor to assign the container.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex03 - Filter Provider Constructor_)
	<!-- mark: 3-8 -->
	````C#
	public class FilterProvider : IFilterProvider
    {
        private IUnityContainer container;

        public FilterProvider(IUnityContainer container)
        {
            this.container = container;
        }
    }
	````

	>**Note:** The filter provider class constructor is not creating a **new** object inside. The container is passed as a parameter, and the dependency is solved by Unity.

1. In the **FilterProvider** class, implement the method **GetFilters** from **IFilterProvider** interface.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex03 - Filter Provider GetFilters_)
	<!-- mark: 9-16 -->
	````C#
	public class FilterProvider : IFilterProvider
	 {
		  private IUnityContainer container;

		  public FilterProvider(IUnityContainer container)
		  {
				this.container = container;
		  }

		  public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
		  {
				foreach (IActionFilter actionFilter in this.container.ResolveAll<IActionFilter>())
				{
					 yield return new Filter(actionFilter, FilterScope.First, null);
				}
		  }
	 }
	````

<a name="Ex3Task2" />
#### Task 2 - Registering and Enabling the Filter ####

In this task, you will enable site tracking. To do that, you will register the filter in **Bootstrapper.cs BuildUnityContainer** method to start tracing:

1. Open **Web.config** located in the project root and enable trace tracking at System.Web group.

	<!-- mark: 2 -->
	````XML
	  <system.web>
	    <trace enabled="true"/>
	    <compilation debug="true" targetFramework="4.5">
	````

1. Open **Bootstrapper.cs** at project root.

1. Add a reference to the Filters namespace.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex03 - Bootstrapper Adding Namespaces_)
	<!-- mark: 7 -->
	````C#
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	using Unity.Mvc3;
	using MvcMusicStore.Services;
	using MvcMusicStore.Controllers;
	using MvcMusicStore.Factories;
	using MvcMusicStore.Filters;
	````

1. Select the **BuildUnityContainer** method and register the filter in the Unity Container. You will have to register the filter provider as well as the action filter.

	(Code Snippet - _ASP.NET Dependency Injection Lab - Ex03 - Register FilterProvider and ActionFilter_)
	<!-- mark: 7-8 -->
	````C#
	private static IUnityContainer BuildUnityContainer()
	  {
			var container = new UnityContainer();

			...
			
			container.RegisterInstance<IFilterProvider>("FilterProvider", new FilterProvider(container));
			container.RegisterInstance<IActionFilter>("LogActionFilter", new TraceActionFilter());

			return container;
	  }
	````

<a name="Ex3Task3" />
#### Task 3 - Running the Application ####

In this task, you will run the application and test that the custom action filter is tracing the activity:

1. Press **F5** to run the application.

1. Click **Rock** within the Genres Menu. You can browse to more genres if you want to.

 	![Music Store](./images/Music_Store2.png?raw=true "Music Store")
 
	_Music Store_

1. Browse to **/Trace.axd** to see the Application Trace page, and then click **View Details**.

 	![Application Trace Log](./images/Application_Trace_Log.png?raw=true "Application Trace Log")
 
	_Application Trace Log_

 	![Application Trace - Request Details](./images/Application_Trace_Request_Details.png?raw=true "Application Trace - Request Details")
 
	_Application Trace - Request Details_

1. Close the browser.

>**Note:** Additionally, you can deploy this application to Windows Azure Web Sites following [Appendix B: Publishing an ASP.NET MVC 4 Application using Web Deploy](#AppendixB).

---

<a name="Summary" />
## Summary ##

By completing this Hands-On Lab you have learned how to use Dependency Injection in ASP.NET MVC 4 by integrating Unity using a NuGet Package. To achieve that, you have used Dependency Injection inside controllers, views and action filters.

The following concepts were covered:

- ASP.NET MVC 4 Dependency Injection features

- Unity integration using Unity.Mvc3 NuGet Package

- Dependency Injection in Controllers

- Dependency Injection in Views

- Dependency injection of Action Filters

<a name="AppendixA" />
## Appendix A: Installing Visual Studio Express 2012 for Web ##

You can install **Microsoft Visual Studio Express 2012 for Web** or another "Express" version using the **[Microsoft Web Platform Installer](http://www.microsoft.com/web/downloads/platform.aspx)**. The following instructions guide you through the steps required to install _Visual studio Express 2012 for Web_ using _Microsoft Web Platform Installer_.

1. Go to [http://go.microsoft.com/?linkid=9810169](http://go.microsoft.com/?linkid=9810169). Alternatively, if you already have installed Web Platform Installer, you can open it and search for the product "_Visual Studio Express 2012 for Web with Windows Azure SDK_".

1. Click on **Install Now**. If you do not have **Web Platform Installer** you will be redirected to download and install it first.

1. Once **Web Platform Installer** is open, click **Install** to start the setup.

	![Install Visual Studio Express](images/install-visual-studio-express.png?raw=true "Install Visual Studio Express")

 	_Install Visual Studio Express_

1. Read all the products' licenses and terms and click **I Accept** to continue.

	![Accepting the license terms](images/accepting-the-license-terms.png?raw=true)

	_Accepting the license terms_

1. Wait until the downloading and installation process completes.

	![Installation progress](images/installation-progress.png?raw=true)

	_Installation progress_

1. When the installation completes, click **Finish**.

	![Installation completed](images/installation-completed.png?raw=true)

	_Installation completed_

1. Click **Exit** to close Web Platform Installer.

1. To open Visual Studio Express for Web, go to the **Start** screen and start writing "**VS Express**", then click on the **VS Express for Web** tile.

	![VS Express for Web tile](images/vs-express-for-web-tile.png?raw=true)

	_VS Express for Web tile_

<a name="AppendixB" />
## Appendix B: Publishing an ASP.NET MVC 4 Application using Web Deploy ##

This appendix will show you how to create a new web site from the Windows Azure Management Portal and publish the application you obtained by following the lab, taking advantage of the Web Deploy publishing feature provided by Windows Azure.

<a name="ApxBTask1"></a>
#### Task 1 – Creating a New Web Site from the Windows Azure Portal ####

1. Go to the [Windows Azure Management Portal](https://manage.windowsazure.com/) and sign in using the Microsoft credentials associated with your subscription.

	![Log on to Windows Azure portal](images/login.png?raw=true "Log on to Windows Azure portal")

	_Log on to Windows Azure Management Portal_

1. Click **New** on the command bar.

	![Creating a new Web Site](images/new-website.png?raw=true "Creating a new Web Site")

	_Creating a new Web Site_

1. Click **Compute** | **Web Site**. Then select **Quick Create** option. Provide an available URL for the new web site and click **Create Web Site**.

	> **Note:** A Windows Azure Web Site is the host for a web application running in the cloud that you can control and manage. The Quick Create option allows you to deploy a completed web application to the Windows Azure Web Site from outside the portal. It does not include steps for setting up a database.

	![Creating a new Web Site using Quick Create](images/quick-create.png?raw=true "Creating a new Web Site using Quick Create")

	_Creating a new Web Site using Quick Create_

1. Wait until the new **Web Site** is created.

1. Once the Web Site is created click the link under the **URL** column. Check that the new Web Site is working.

	![Browsing to the new web site](images/navigate-website.png?raw=true "Browsing to the new web site")

	_Browsing to the new web site_

	![Web site running](images/website-working.png?raw=true "Web site running")

	_Web site running_

1. Go back to the portal and click the name of the web site under the **Name** column to display the management pages.

	![Opening the web site management pages](images/go-to-the-dashboard.png?raw=true "Opening the web site management pages")
	
	_Opening the Web Site management pages_

1. In the **Dashboard** page, under the **quick glance** section, click the **Download publish profile** link.

	> **Note:** The _publish profile_ contains all of the information required to publish a web application to a Windows Azure website for each enabled publication method. The publish profile contains the URLs, user credentials and database strings required to connect to and authenticate against each of the endpoints for which a publication method is enabled. **Microsoft WebMatrix 2**, **Microsoft Visual Studio Express for Web** and **Microsoft Visual Studio 2012** support reading publish profiles to automate configuration of these programs for publishing web applications to Windows Azure websites. 

	![Downloading the web site publish profile](images/download-publish-profile.png?raw=true "Downloading the web site publish profile")
	
	_Downloading the Web Site publish profile_

1. Download the publish profile file to a known location. Further in this exercise you will see how to use this file to publish a web application to a Windows Azure Web Sites from Visual Studio.

	![Saving the publish profile file](images/save-link.png?raw=true "Saving the publish profile")
	
	_Saving the publish profile file_

<a name="ApxBTask2"></a>
#### Task 2 – Configuring the Database Server ####

If your application makes use of SQL Server databases you will need to create a SQL Database server. If you want to deploy a simple application that does not use SQL Server you might skip this task.

1. You will need a SQL Database server for storing the application database. You can view the SQL Database servers from your subscription in the Windows Azure Management portal at **Sql Databases** | **Servers** | **Server's Dashboard**. If you do not have a server created, you can create one using the **Add** button on the command bar. Take note of the **server name and URL, administrator login name and password**, as you will use them in the next tasks. Do not create the database yet, as it will be created in a later stage.

	![SQL Database Server Dashboard](images/sql-database-server-dashboard.png?raw=true "SQL Database Server Dashboard")

	_SQL Database Server Dashboard_

1. In the next task you will test the database connection from Visual Studio, for that reason you need to include your local IP address in the server's list of **Allowed IP Addresses**. To do that, click **Configure**, select the IP address from **Current Client IP Address** and paste it on the **Start IP Address** and **End IP Address** text boxes and click the ![add-client-ip-address-ok-button](images/add-client-ip-address-ok-button.png?raw=true) button.

	![Adding Client IP Address](images/add-client-ip-address.png?raw=true)

	_Adding Client IP Address_

1. Once the **Client IP Address** is added to the allowed IP addresses list, click on **Save** to confirm the changes.

	![Confirm Changes](images/add-client-ip-address-confirm.png?raw=true)

	_Confirm Changes_

<a name="ApxBTask3"></a>
#### Task 3 – Publishing an ASP.NET MVC 4 Application using Web Deploy ####

1. Go back to the ASP.NET MVC 4 solution. In the **Solution Explorer**,  right-click the web site project and select **Publish**.

	![Publishing the Application](images/publishing-the-application.png?raw=true "Publishing the Application")

	_Publishing the web site_

1. Import the publish profile you saved in the first task.

	![Importing the publish profile](images/importing-the-publish-profile.png?raw=true "Importing the publish profile")

	_Importing publish profile_

1. Click **Validate Connection**. Once Validation is complete click **Next**.

	> **Note:** Validation is complete once you see a green checkmark appear next to the Validate Connection button.

	![Validating connection](images/validating-connection.png?raw=true "Validating connection")

	_Validating connection_

1. In the **Settings** page, under the **Databases** section, click the button next to your database connection's textbox (i.e. **DefaultConnection**).

	![Web deploy configuration](images/web-deploy-configuration.png?raw=true "Web deploy configuration")

	_Web deploy configuration_

1. Configure the database connection as follows:
	* In the **Server name** type your SQL Database server URL using the _tcp:_ prefix.
	* In **User name** type your server administrator login name.
	* In **Password** type your server administrator login password.
	* Type a new database name.

	![Configuring destination connection string](images/configuring-destination-connection-string.png?raw=true "Configuring destination connection string")

	_Configuring destination connection string_

1. Then click **OK**. When prompted to create the database click **Yes**.

	![Creating the database](images/creating-the-database.png?raw=true "Creating the database string")

	_Creating the database_

1. The connection string you will use to connect to SQL Database in Windows Azure is shown within Default Connection textbox. Then click **Next**.

	![Connection string pointing to SQL Database](images/sql-database-connection-string.png?raw=true "Connection string pointing to SQL Database")

	_Connection string pointing to SQL Database_

1. In the **Preview** page, click **Publish**.

	![Publishing the web application](images/publishing-the-web-application.png?raw=true "Publishing the web application")

	_Publishing the web application_

1. Once the publishing process finishes, your default browser will open the published web site.


<a name="AppendixC"></a>
## Appendix C: Using Code Snippets ##

With code snippets, you have all the code you need at your fingertips. The lab document will tell you exactly when you can use them, as shown in the following figure.

 ![Using Visual Studio code snippets to insert code into your project](./images/Using-Visual-Studio-code-snippets-to-insert-code-into-your-project.png?raw=true "Using Visual Studio code snippets to insert code into your project")
 
_Using Visual Studio code snippets to insert code into your project_

_**To add a code snippet using the keyboard (C# only)**_

1. Place the cursor where you would like to insert the code.

1. Start typing the snippet name (without spaces or hyphens).

1. Watch as IntelliSense displays matching snippets' names.

1. Select the correct snippet (or keep typing until the entire snippet's name is selected).

1. Press the Tab key twice to insert the snippet at the cursor location.

 
   ![Start typing the snippet name](./images/Start-typing-the-snippet-name.png?raw=true "Start typing the snippet name")
 
_Start typing the snippet name_

   ![Press Tab to select the highlighted snippet](./images/Press-Tab-to-select-the-highlighted-snippet.png?raw=true "Press Tab to select the highlighted snippet")
 
_Press Tab to select the highlighted snippet_

   ![Press Tab again and the snippet will expand](./images/Press-Tab-again-and-the-snippet-will-expand.png?raw=true "Press Tab again and the snippet will expand")
 
_Press Tab again and the snippet will expand_

_**To add a code snippet using the mouse (C#, Visual Basic and XML)**_
1. Right-click where you want to insert the code snippet.

1. Select **Insert Snippet** followed by **My Code Snippets**.

1. Pick the relevant snippet from the list, by clicking on it.

 
  ![Right-click where you want to insert the code snippet and select Insert Snippet](./images/Right-click-where-you-want-to-insert-the-code-snippet-and-select-Insert-Snippet.png?raw=true "Right-click where you want to insert the code snippet and select Insert Snippet")
 
_Right-click where you want to insert the code snippet and select Insert Snippet_

 ![Pick the relevant snippet from the list, by clicking on it](./images/Pick-the-relevant-snippet-from-the-list,-by-clicking-on-it.png?raw=true "Pick the relevant snippet from the list, by clicking on it")
 
_Pick the relevant snippet from the list, by clicking on it_

