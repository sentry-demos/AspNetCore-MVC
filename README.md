# Summary
To show how Sentry works in the ASP.NET Core SDK.
- how to integrate the SDK into ASP.NET CORE (https://docs.sentry.io/platforms/dotnet/aspnetcore/)
- trigger an error that gets sent as Event to Sentry.io Platform
- `./AspNetCoreMvc/Controllers/HomeController.cs` contains the REST endpoints for triggering errors captured by Sentry SDK and sent as Events to Sentry
- The Sentry release cycle is covered in `./AspNetCoreMvc/deploy.ps1`

## Versions Summary:

| dependency      | version           
| ------------- |:-------------:| 
| .NET Core Runtime      | 3.1.1  |
| .NET SDK  | 3.1.404   |
| Sentry SDK  | 2.1.8   |
| sentry-cli   | 1.62.0   |
| PowerShell  | 7.1.0   |
| macOS | Catalina 10.15.6|


## First Setup & Run
1. Install dependencies
   - Powershell: 
   https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-macos?view=powershell-7.1

   - .NET Core Runtime & SDK:
    https://dotnet.microsoft.com/download/dotnet-core/3.1

    - sentry-sdk:
    ```
    dotnet add package Sentry.AspNetCore --version 2.1.8
    ```

2. Configure your DSN in [appsettings.json](appsettings.json)

3. Configure your org slug and project slug in [deploy.ps1](deploy.ps1)

4. Add auth env variable to terminal: 
```export SENTRY_AUTH_TOKEN=ADD_MY_AUTH_TOKEN_HERE```

5. ```SENTRY_RELEASE=`sentry-cli releases propose-version` pwsh deploy.ps1```

6. `http://localhost:5001/handled` to trigger error and send event to Sentry

# GIF
![Alt Text](demo.gif)

## Gotchas 

#### Question: Where is ```Sentry.init()```?
\
This demo leverages ASP.NET Core's configuration system. Because of this much of your Sentry config info (dsn etc.) is going to be located in ```appsettings.json```. This allows us to call the ```UseSentry()``` extension method on our webbuilder in ```Program.cs``` without any arguments.

#### Question: How am I setting breadcrumbs without ```SentrySdk.AddBreadcrumb()```?
\
This demo utilizes Sentry's integration with ```Microsoft.Extensions.Logging``` to both set  breadcrumbs and send errors. Your ```appsettings.json``` file configures the minimum log levels that correspond to either a crumb or an event. More information here: https://docs.sentry.io/platforms/dotnet/guides/aspnetcore/#configure