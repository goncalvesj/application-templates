# Azure DevOps and GitHub Task/Issue Sync

Sample project to test Azure DevOps and GitHub integration.
This example uses Azure DevOps to manage the work items and GitHub to manage the code.

Syncing is done using the Azure DevOps Service Hooks and GitHub Webhooks. 
The Azure DevOps Service Hooks are configured to send a notification to the Azure Function when a work item is created. The Azure Function then uses the GitHub API to create the corresponding issue.
The GitHub Webhooks are configured to send a notification to the Azure Function when an issue is closed. The Azure Function then uses the Azure DevOps API to close the corresponding work item.

## Prerequisites

- Azure DevOps account
- GitHub account
    - GitHub PAT
- Azure subscription

## Setup

### Azure DevOps

1. Create a new project in Azure DevOps
2. Create a new service connection to GitHub (this is used to display the GitHub issues in Azure DevOps)
3. Create a new service hook to send a notification to the Azure Function when a work item is created
4. Create a new task type work item type, with 2 tags: owner and repository

### GitHub