# Orchard Application Host Readme



## Project Description

A light-weight framework that allows you to write arbitrary code (console or web applications, anything) empowered with [Orchard](http://orchardproject.net/).


## Overview

The [Orchard Application Host](https://github.com/Lombiq/Orchard-Application-Host) is a portable environment that lets you run your application inside a standalone [Orchard](http://orchardproject.net) shell. I.e. you can write any app with an Orchard developer experience, without using an Orchard web app. This enables you to use Orchard's features and services from any application (not just web applications), including:

- Automatic dependency injection
- Helpers and utilities
- Data access services, including content management
- Orchard-style events
- Shapes
- Caching
- Localization
- Logging
- Background tasks

With Orchard Application Host you can create console applications, Windows services, desktop applications, cloud workers or any other type of app that uses Orchard's capabilities. No more low-level project start: you get an application framework that you can begin developing awesome software with, utilizing your Orchard knowledge and Orchard's power.

You can see a demo of the Orchard Application Host on the [recording of the Orchard Community Meeting](https://www.youtube.com/watch?v=_9lf7uZ-Ztk&feature=youtu.be&t=22m55s).

Among others Orchard Application Host powers the reverse proxy of the [Hosting Suite](https://dotnest.com/knowledge-base/topics/lombiq-hosting-suite) too.


## Usage

- See examples in [Lombiq.OrchardAppHost.Sample](https://github.com/Lombiq/Orchard-Application-Host-Sample) and for a full usage scenario with a non-Orchard solution in the [Orchard Application Host Quick Start](https://bitbucket.org/Lombiq/orchard-application-host-quick-start).
- Disable SessionConfigurationCache otherwise you'll get "The invoked member is not supported in a dynamic assembly." exceptions that are harmless but prevent the session cache from being used anyway.
- You'll get a "The invoked member is not supported in a dynamic assembly." exception during the first startup from AbstractDataServicesProvider but this is harmless.
-  Also from AbstractDataServicesProvider you'll get a "Could not load file or assembly 'NHibernate.XmlSerializers ...' or one of its dependencies. The system cannot find the file specified." exception that [is also harmless](http://www.mail-archive.com/nhusers@googlegroups.com/msg06041.html).
- If you want to use anything, even indirectly, from Orchard.Core, you have to add a project reference to it. E.g. even if you don't access anything from Orchard.Core but you use a service that gets `ISiteService` injected what in turn has an implementation in Orchard.Core then you indirectly depend on Orchard Core; thus, you have to add a project reference to it.
- When using SQL CE you should add a reference to its assembly System.Data.SqlServerCe and set it as Copy Local = true.
- Imported extensions don't need to declare a Module.txt but still can have features: by default they get a feature with the same name as the assembly's (short) name and also all OrchardFeature attribute usages will be processed and their values registered as features.
- Note that starting Orchard App Host will currently take over ASP.NET MVC and Web API controller instantiation, see [this Orchard issue](https://github.com/OrchardCMS/Orchard/issues/4748).

### Using Orchard App Host as source in a solution

The solution **must** follow this folder structure:

- Lombiq.OrchardAppHost
	- Lombiq.OrchardAppHost.csproj
- Orchard (a full Orchard source, i.e. the lib, src folder under it)
- Arbitrarily named subfolder for 3rd party modules, e.g. Modules.
	- Module1
		- Module1.csproj

The [Orchard Application Host Quick Start](https://bitbucket.org/Lombiq/orchard-application-host-quick-start) solution shows these conventions.

3rd party modules may reference dlls from the Orchard lib folder. By default these references will break since modules in an Orchard solution are under src/Orchard.Web/Modules, not above the Orchard folder (and thus paths differ). To make a module compatible with both standard Orchard solutions and Orchard App Host solutions add the following elements to the modules's csproj:
	
	<!-- Orchard App Host (https://orchardapphost.codeplex.com/) compatibility start. Enabling the usage of a lib folder at a different location. -->
	<ItemGroup>
	  <LibReferenceSearchPathFiles Include="..\..\Orchard\lib\**\*.dll">
	    <InProject>false</InProject>
	  </LibReferenceSearchPathFiles>
	</ItemGroup>
	<Target Name="BeforeResolveReferences">
	  <RemoveDuplicates Inputs="@(LibReferenceSearchPathFiles->'%(RootDir)%(Directory)')">
	    <Output TaskParameter="Filtered" ItemName="LibReferenceSearchPath" />
	  </RemoveDuplicates>
	  <CreateProperty Value="@(LibReferenceSearchPath);$(AssemblySearchPaths)">
	    <Output TaskParameter="Value" PropertyName="AssemblySearchPaths" />
	  </CreateProperty>
	</Target>
	<PropertyGroup Condition="Exists('..\..\Orchard\lib')">
	  <ModulesRoot>..\..\Orchard\src\Orchard.Web\Modules\Orchard.Alias\</ModulesRoot>
	</PropertyGroup>
	<!-- Orchard App Host (https://orchardapphost.codeplex.com/) compatibility end. -->

Also make sure to prefix every project reference that points to one of Orchard's built-in projects with `$(ModulesRoot)`:

	<ProjectReference Include="$(ModulesRoot)..\..\..\Orchard\Orchard.Framework.csproj">
	  <Project>{2D1D92BB-4555-4CBE-8D0E-63563D6CE4C6}</Project>
	  <Name>Orchard.Framework</Name>
	</ProjectReference>

The project's source is available in two public source repositories, automatically mirrored in both directions with [Git-hg Mirror](https://githgmirror.com):

- [https://bitbucket.org/Lombiq/orchard-application-host](https://bitbucket.org/Lombiq/orchard-application-host) (Mercurial repository)
- [https://github.com/Lombiq/Orchard-Application-Host](https://github.com/Lombiq/Orchard-Application-Host) (Git repository)

Bug reports, feature requests and comments are warmly welcome, **please do so via GitHub**.
Feel free to send pull requests too, no matter which source repository you choose for this purpose.

This project is developed by [Lombiq Technologies Ltd](http://lombiq.com/). Commercial-grade support is available through Lombiq.