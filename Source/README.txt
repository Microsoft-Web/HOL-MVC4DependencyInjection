Before running any of the end solutions provided in this Hands-on lab you need to install the NuGet package dependencies. After opening the solutions with Visual Studio, follow these steps:
1. In Visual Studio, open Package Manager Console from Tools | Library Package Manager.
2. In the Package Manager Console type:
	PM> Install-Package NuGetPowerTools
3. After installing the package, type:
	PM> Enable-PackageRestore
4. Compile the solution. The NuGet dependencies will be downloaded and installed automatically.

For more information see: http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages
