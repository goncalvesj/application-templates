# Application templates & Cloud Architectures using .NET/Angular/Azure

The purpose of this repository is to provide examples of application templates and cloud architectures that use Azure, .NET, Angular and other frameworks.

## Templates

[.Net 7 App on Azure App Service with Private Endpoints Enabled](AppService-FrontDoor-Private-Endpoints)

[Azure Function App that syncs Azure DevOps Work Items and GH Issues](AzDevOps-GitHub-Workflow)

[Azure Function App and Logic App Standard that interact through Webhooks](Azure-Webhooks/)

[.Net Apps on AKS that process Service Bus Queues using KEDA](AzureServiceBus-Worker-Notification)

[A .NET solution that mocks a platform for hotel booking and demonstrates how to implement plugins for ChatGPT.](DotNet-ChatGPT-Plugins/)

[.NET 6 Minimal APIs based on GraphQL and REST](DotNet-GraphQL-Rest-APIs)

[.NET console chat application using Azure Open AI and Semantic Kernel](Dotnet-SK-MemoryPlugin-ChatConsole/)

[Electron Application with Angular that uses Azure AD for AUTH](Electron-Angular-AzureAd)

[Application based on the Event Sourcing Pattern, React Front End, Azure Functions Backend and Azure Storage for Data](Event-Sourcing)

[Application based on the Federated Identity Pattern, .NET Core MVC and Azure B2C for Auth](Federated-Identity)

[.NET Core MVC Application with Angular Client App that uses Azure AD for AUTH](NetCore-Angular-AzureAd)

[.NET Core MVC Application with Angular Client App that uses Azure AD B2C for AUTH](NetCore-Angular-AzureB2C)

[.NET Core MVC Application with Angular Client App that contains an example of usage of Angular Elements](NetCore-Angular-Elements)

[.NET Core that contains an example of usage of Azure Durable Functions](NetCore-Durable-Functions)

[.NET Core Application that can be used as a Windows Service](NetCore-WinService)

## Project Planning

Backlog is managed using Azure DevOps. The project is public and can be accessed using the link below.

[Azure DevOps GitHub Integration](https://dev.azure.com/jpgoncalves/GitHub%20Integration)

Work items are created in Azure DevOps and linked to GitHub issues. The work items are then linked to the issues with the GitHub Integration Service Connection.

## Infrastructure as Code

The infrastructure is managed using a combination of Terraform and Bicep. 

The templates scripts are located in this repository:

[Infrastructure Repository](https://github.com/goncalvesj/iac-templates)