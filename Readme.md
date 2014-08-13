# Orchard Application Host Readme



## Overview

The Orchard Application Host is a portable environment that lets you run your application inside [Orchard](http://orchardproject.net). This enables you to use Orchard's features and services from any application (not just web applications), including:

- Automatic dependency injection
- Helpers and utilities
- Data access services, including content management
- Orchard-style events
- Shapes
- Caching
- Localization
- Logging
- Background tasks

With Orchard Application Host you can create console applications, Windows services, desktop application, cloud workers or any other type of app that uses Orchard's capabilities. No more low-level project start: you get an application framework that you can begin developing awesome software with, utilizing your Orchard knowledge and Orchard's power.

You can see a demo of the Orchard Application Host on the [recording of the Orchard Community Meeting](https://www.youtube.com/watch?v=_9lf7uZ-Ztk&feature=youtu.be&t=22m55s).


# Usage

- See examples in [Lombiq.OrchardAppHost.Sample](https://orchardapphostsample.codeplex.com/) and for a full usage scenario with a non-Orchard solution in the [Orchard Application Host Quick Start](https://bitbucket.org/Lombiq/orchard-application-host-quick-start). When you add Orchard App Host to a solution as source make sure to put Orchard into a folder called "Orchard" at the same level as the containing folder of the App Host project (as it is in the sample solution).
- Disable SessionConfigurationCache otherwise you'll get "The invoked member is not supported in a dynamic assembly." exceptions that are harmless but prevent the session cache from being used anyway.
- You'll get a "The invoked member is not supported in a dynamic assembly." exception during the first startup from AbstractDataServicesProvider but this is harmless.
-  Also from AbstractDataServicesProvider you'll get a "Could not load file or assembly 'NHibernate.XmlSerializers ...' or one of its dependencies. The system cannot find the file specified." exception that [is also harmless](http://www.mail-archive.com/nhusers@googlegroups.com/msg06041.html).
- If you want to use anything, even indirectly, from Orchard.Core, you have to add a project reference to it. E.g. even if you don't access anything from Orchard.Core but you use a service that gets ISiteService injected what in turn has an implementation in Orchard.Core then you indirectly depend on Orchard Core; thus, you have to add a project reference to it.
- When using SQL CE you should add a reference to its assembly System.Data.SqlServerCe and set it as Copy Local = true.
- Imported extensions don't need to declare a Module.txt but still can have features: by default they get a feature with the same name as the assembly's (short) name and also all OrchardFeature attribute usages will be processed and their values registered as features.