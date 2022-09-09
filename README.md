# Introduction 

This is a Supply Web API project that demonstrates standards and best practices for American Software Web APIs build on .NET 6.

# Getting Started

See the walk-through of how the template for this project was built at https://confluence.amsoftware.com/x/bIYIAQ.

# Build and Test

To build the E2E.Supply solution in Debug configuration, run:


```
dotnet build E2E.Supply.sln
```

For release configuration:

```
dotnet build --configuration Release E2E.Supply.sln
```

To run unit tests:

```
dotnet test E2E.Supply.sln
```

# Setting local configuration values
***Do not make environment specific changes to appsettings.json.***

## User secrets
This project is configured to manage local development specific settings in a user secrets file.  In Windows environments this file is located at *%APPDATA%\Microsoft\UserSecrets\e2e.supply.webapihelloworldservice\secrets.json*. In linux environments this file is located at *~/.microsoft/e2e.supply.webapihelloworldservice/secrets.json*.

To specify user secrets in your local build environment:
* In Visual Studio, right click on the Supply.WebApiHelloWorldService project and select "Manage User Secrets". 
* Alternately, create the secrets.json file at %APPDATA%\Microsoft\UserSecrets\e2e.supply.webapihelloworldservice (lower case folder name is important if the secrets will be mounted into a Linux container).  

See [Safe storage of app secrets in development in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=linux#manage-user-secrets-with-visual-studio) for more information.

# Identity Gateway Configuration

The supply controller is secured with Identity Gateway. A bearer token is required to call any of the controller methods. 

## Configuration settings

```
  "IdentityServer": {
    "Authority": "", // for development set this to "https://app-idg-asidev-eastus.azurewebsites.net" in your secrets.json file.
    "Audience": "AsiApiResource", 
    "ApiScope": "AsiApiScope",
    "ClientId": "AsiAuthorizationCodeClient"
  },
```

| Setting | Description | Default |
| - | - | - |
| Authority | The Identity Gateway responsible for issuing tokens for this service. <br /> This should be set to "https://app-idg-asidev-eastus.azurewebsites.net" | 
| Audience | The API resource <br /> The default value should be used in most cases. | AsiApiResource |
| ApiScope | The API scope <br /> The default value should be used in most cases. | AsiApiScope |
| ClientId | The clientid used by swagger ui when requesting tokens; not used in production environments. | AsiAuthorizationCodeClient |

## Configuring AsiAuthorizationCodeClient for local development

This service is configured to run on port 44397 locally.  The AsiAuthorizationCodeClient client in Identity Gateway will need to be updated to include the redirect uri and Cors entries for this local service.  To confirm the port this service is using locally:
* Open src/Supply.WebApiHelloWorldService/Properties/launchSetting.json in Visual Studio or a text editor
* Locate the "iisExpress" section
* Note the "sslPort" value.  This is the port that the service will be surfaced on when running locally.  You will need this port number when updating Identity Admin below.

```
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:16457",
      // note the sslPort number
      "sslPort": 44397 
    }
  }
```

Using Identity Admin: 
* Edit the AsiAuthorizationCodeClient entry
* On the Basics tab, select Client Secrets and create a unique secret for the development environment.  This secret is unique to the developer and should be labeled with the developer's name in the description field.  This will need to be done for each developer.
  * **Note the value for the secret as it will not be available again.  You can allow the page to create a secret for you, or you may choose your own secret value.**
* On the Basics tab, update the redirect URIs to include the local swagger endpoint for this service.  This should be something like "https://localhost:44397/swagger/oauth2-redirect.html" where the port number is updated to reflect the local port number for this service.
* On the Token tab, update the Allowed Cors Origins to include the protocol, hostname, and port for this service.  This should be something like "https://localhost:44397" where the port number is updated to reflect the local port number for this service. ***Note: The URL must NOT include the path or trailing slash!***

# Contribute

This is an example of what the Contribute section might look like to provide developers useful information about working on the code in a Web API project.

* Warnings will be treated as errors in Release configuration builds, so pipelines will fail if there are warnings when commits are pushed. Please build a Release configuration prior to pushing.
* [Nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references) [nullable contexts](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references#nullable-contexts) are enabled in the project.
