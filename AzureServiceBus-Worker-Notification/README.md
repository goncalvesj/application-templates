# .NET Service Bus Workers samples

## Infrastructure

TODO: Link to infrastructure repo

## K8s

Deployment files for AKS

- deploy-app.yaml - deploys the Net.ServiceBus.Workers sample app as K8s deployment

- deploy-autoscaling.yaml - deploys the KEDA auto scaler for the Net.ServiceBus.Workers sample app

- deploy-job.yaml - deploys the Net.ServiceBus.SingleJob sample app as a K8s job, contains the KEDA auto scaler

- deploy-keda-trigger.yaml - deploys the KEDA trigger that connects to Azure Service Bus

- deploy-kv-provider.yaml - deploys the Key Vault object that maps to KV secrets to be used in the apps

## Net.ServiceBus.Client

Blazor client for real time notifications

## Net.ServiceBus.Entities

Classes that map to Event Grid events

## Net.ServiceBus.Notification.Functions

Azure Durable Functions that handle SignalR notifications and Email notifications
Triggered by Service Bus messages on a queue

## Net.ServiceBus.SingleJob

Console App to be used as Kubernetes long running job to process Service Bus messages

## Net.ServiceBus.Worker

Worker Service that processes Service Bus messages continuously to be used as a Kubernetes deployment