# .NET Service Bus Workers samples

Sample apps to test the functionality of processing Azure service bus queues using AKS + KEDA.

Flow:

1. User drops files in input container
2. Event grid posts a message to the service bus input queue
3. KEDA scales up the worker pod in AKS
4. Sample worker processes the message, simulates some work , moves the file to the output container and deletes the file from the input container
5. Event grid posts a message to the service bus output queue
6. Azure Function processes the message, sends an email to the user and posts a real time notification to the SignalR service
7. Real time notification is received by the web app and displayed to the user

## Infrastructure

[Link to infrastructure repo](https://github.com/goncalvesj/iac-templates/tree/master/Bicep/AzureServiceBus-Worker-Notification)

## K8s

Deployment files for AKS

- [deploy-app.yaml](K8s/deploy-app.yaml) - deploys the Net.ServiceBus.Workers sample app as K8s deployment

- [deploy-autoscaling.yaml](K8s/deploy-autoscaling.yaml) - deploys the KEDA auto scaler for the Net.ServiceBus.Workers sample app

- [deploy-job.yaml](K8s/deploy-job.yaml) - deploys the Net.ServiceBus.SingleJob sample app as a K8s job, contains the KEDA auto scaler

- [deploy-keda-trigger.yaml](K8s/deploy-keda-trigger.yaml) - deploys the KEDA trigger that connects to Azure Service Bus

- [deploy-kv-provider.yaml](K8s/deploy-kv-provider.yaml) - deploys the Key Vault object that maps to KV secrets to be used in the apps

The applications are deployed to the Job Node pool which scales down to zero by using the following yaml properties:

```yaml
tolerations:
- key: "kubernetes.azure.com/scalesetpriority"
  operator: "Equal"
  value: "spot"
  effect: "NoSchedule"
nodeSelector:
  workload: jobs
```

If you want to deploy the apps to the Virtual node pool, swap the above properties with this.

```yaml
tolerations:
- key: virtual-kubelet.io/provider
  operator: Exists
- key: azure.com/aci
  effect: NoSchedule
nodeSelector:
  kubernetes.io/role: agent
  beta.kubernetes.io/os: linux
  type: virtual-kubelet
```

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
