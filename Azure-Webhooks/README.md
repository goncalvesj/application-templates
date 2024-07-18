# Azure Webhook Handling Projects

*The below text was generated using GH Copilot.*

This repository contains two main projects, each designed to handle webhooks but with different technologies and purposes:

1. **AzFunc.Webhook**:
   - **Type**: Azure Functions project
   - **Language/Framework**: C# with .NET (specifically targeting .NET 8.0)
   - **Purpose**: Designed to handle HTTP requests as webhooks using Azure Functions, a serverless compute service. It's set up to respond to HTTP triggers.
   - **Deployment and Configuration**: Includes a `deploy.ps1` script for deployment, to Azure, and uses `host.json` and `local.settings.json` for configuration.
   - **Notable Features**: Targets the Azure Functions v4 runtime and includes references to Application Insights for monitoring.

2. **LogicApp.Webhook**:
   - **Type**: Logic Apps Standard project
   - **Language/Framework**: Extension bundle-based (Node.js), common for defining the workflow logic in Azure Logic Apps.
   - **Purpose**: Designed to implement workflows, to process webhooks, using Azure Logic Apps. Defines specific workflows for handling HTTP requests and webhooks.
   - **Deployment and Configuration**: Uses `host.json` and `local.settings.json` for configuration. The `.funcignore` file indicates which files and directories are ignored during deployment to Azure.
   - **Notable Features**: Configured to work with Azure Logic Apps for creating automated workflows between apps and services.

Both projects are part of a larger ecosystem for handling webhooks in a cloud environment, leveraging Azure's serverless and workflow orchestration services. The repository is structured to support development, testing, and deployment of these projects within Azure, utilizing Visual Studio Code settings and tasks for a streamlined workflow.

## Example `local.settings.json` for Both Projects

For **AzFunc.Webhook**:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

For **LogicApp.Webhook**:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "APP_KIND": "workflowapp",
    "ProjectDirectoryPath": "",
    "FUNCTIONS_WORKER_RUNTIME": "node",
    "WORKFLOWS_TENANT_ID": "xxxxxxx",
    "WORKFLOWS_SUBSCRIPTION_ID": "xxxxxxxxxx",
    "WORKFLOWS_RESOURCE_GROUP_NAME": "xxxxx",
    "WORKFLOWS_LOCATION_NAME": "northeurope",
    "WORKFLOWS_MANAGEMENT_BASE_URI": "https://management.azure.com/",
    "Workflows.WebhookRedirectHostUri": "http://localhost:7071"
  }
}
```
