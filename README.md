# Summary
To show how Sentry works in the ASP.NET Core SDK.
- how to integrate the SDK into ASP.NET CORE (https://docs.sentry.io/platforms/dotnet/aspnetcore/)
- trigger an error that gets sent as Event to Sentry.io Platform
- `./AspNetCoreMvc/Controllers/HomeController.cs` contains the REST endpoints for triggering errors captured by Sentry SDK and sent as Events to Sentry
- The Sentry release cycle is covered in `./AspNetCoreMvc/deploy.ps1`

# Initial Setup & Run
1. Configure your DSN in [appsettings.json](appsettings.json)
2. Configure your org slug and project slug in [deploy.ps1](deploy.ps1)
3. ```SENTRY_RELEASE=`sentry-cli releases propose-version` pwsh deploy.ps1```
4. `http://localhost:62920/Home/handled` to trigger error and send event to Sentry

# GIF
here
