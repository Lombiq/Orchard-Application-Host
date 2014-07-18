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


# Usage

- See the examples in Lombiq.OrchardAppHost.Sample.
- Disable SessionConfigurationCache otherwise you'll get "The invoked member is not supported in a dynamic assembly." exceptions that are harmless but prevent the session cache from being used anyway.
- You'll get a "The invoked member is not supported in a dynamic assembly." exception during the first startup from AbstractDataServicesProvider but this is harmless.
-  Also from AbstractDataServicesProvider you'll get a "Could not load file or assembly 'NHibernate.XmlSerializers ...' or one of its dependencies. The system cannot find the file specified." exception that [is also harmless](http://www.mail-archive.com/nhusers@googlegroups.com/msg06041.html).
- If you want to use anything, even indirectly, from Orchard.Core, you have to add a project reference to it. E.g. even if you don't access anything from Orchard.Core but you use a service that gets ISiteService inject what in turn has an implementation in Orchard.Core the you indirectly depend on Orchard Core; thus, you have to add a project reference to it.
- When using SQL CE you should add a reference to its assembly System.Data.SqlServerCe and set it as Copy Local = true.
- Imported extensions don't need to declare a Module.txt but still can have features: by default they get a feature with the same name as the assembly's (short) name and also all OrchardFeature attribute usages will be processed and their values registered as features.