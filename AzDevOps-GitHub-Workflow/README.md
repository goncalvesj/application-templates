# Azure DevOps and GitHub Task/Issue Sync

Sample project to test Azure DevOps and GitHub integration.
This example uses Azure DevOps to manage the work items and GitHub to manage the code.

Syncing is done using the Azure DevOps Service Hooks and GitHub Webhooks. 
The Azure DevOps Service Hooks are configured to send a notification to the Azure Function when a work item is created. The Azure Function then uses the GitHub API to create the corresponding issue.
The GitHub Webhooks are configured to send a notification to the Azure Function when an issue is closed. The Azure Function then uses the Azure DevOps API to close the corresponding work item.

## Infrastructure Repository

[Link to infrastructure repo](https://github.com/goncalvesj/iac-templates/tree/master/Bicep)

## Prerequisites

- Azure DevOps account
    - ADO PAT
- GitHub account
    - GitHub PAT
- Azure subscription

## Setup

Create a `local.settings.json` file in the Azure Function project with the following content:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "GITHUB_PAT": "<GHPAT>",
    "PRODUCT_HEADER_VALUE": "MyApp",
    "AZUREDEVOPS_ORG": "<ADOORG>",
    "AZUREDEVOPS_PAT": "<ADOPAT>"
  }
}
```

### Azure Function

1. Create a new Azure Function App
2. Deploy the Azure Function project to the Azure Function App

### Azure DevOps

1. Create a new project in Azure DevOps
2. Create a new service connection to GitHub (this is used to display the GitHub issues in Azure DevOps)
3. Create a new service hook to send a notification to the Azure Function when a work item is created
4. Create a new task type work item type, with 2 tags: 
    - `owner:GitHub username`
    - `repository:GitHub repository name`

### GitHub

1. Create a new repository in GitHub
2. Create a new webhook to send a notification to the Azure Function when an issue is closed
3. Create a new issue.
4. Close the issue.

### Flow

1. Create a new task work item in Azure DevOps
2. Add the tags to the work item and save
3. The Azure Function is triggered
4. The Azure Function reads the work item tags
5. Creates a new issue in GitHub on the corresponding repository and adds the work item ID to the issue body: `AB#123`
6. Close the issue in GitHub
7. The Azure Function is triggered
8. If the GH issue status is closed the Azure Function reads the issue body and extracts the work item ID