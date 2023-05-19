# Apps on Kubernetes with Azure Arc

## Install MicroK8S on Ubuntu Server

Supporting links:

- <https://securecloud.blog/2021/11/15/microk8s-notes-on-installation-with-azure-arc/>
- <https://www.linkedin.com/pulse/multi-cloud-experiment-azure-arc-kubernetes-raspberry-brian-amedro/>
- <https://ubuntu.com/kubernetes/install>

1. Download Ubuntu Server from <https://ubuntu.com/download/server>.
2. Create a VM in Hyper-V with the downloaded ISO.
   1. Disable Secure Boot.
   2. 2 vCPUs, 4 GB RAM, 20 GB HDD.
3. Install Ubuntu Server, select OpenSSH Server to connect remotely from VSCode.
4. Start Ubuntu Server and connect with SSH.
   2. `sudo snap install microk8s --classic`
5. Extract the `kubeconfig` file from the VM for integration with Azure Arc.
   1. `mkdir ~/.kube`
   2. `microk8s config > ~/.kube/config`
6. Enable the required add-ons.
   1. `microk8s enable dns metrics-server hostpath-storage`

## Kubernetes on Docker Desktop

## Install Redis with Helm

Install the chart in the same namespace as the apps.

1. `sudo microk8s helm repo add bitnami https://charts.bitnami.com/bitnami`
2. `sudo microk8s helm repo update`
3. `sudo microk8s helm install my-redis bitnami/redis`
4. `kubectl get pods`
5. `kubectl get svc`
6. `kubectl get secret --namespace default my-redis -o jsonpath="{.data.redis-password}" | base64 -d`

### Notes

Read/Write: `my-redis-master.default.svc.cluster.local for read/write operations (port 6379)`
Read Only: `my-redis-replicas.default.svc.cluster.local for read-only operations (port 6379)`

To get Redis secret: `export REDIS_PASSWORD=$(kubectl get secret --namespace default my-redis -o jsonpath="{.data.redis-password}" | base64 -d)`

### Testing Redis

Run a Redis pod that you can use as a client:

`kubectl run --namespace default redis-client --restart='Never' --env REDIS_PASSWORD=$REDIS_PASSWORD --image docker.io/bitnami/redis:7.0.9-debian-11-r1 --command -- sleep infinity`

Use the following command to attach to the pod:

`kubectl exec --tty -i redis-client --namespace default -- bash`

Connect using the Redis CLI:

`REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h my-redis-master`
`REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h my-redis-replicas`

To connect to your database from outside the cluster execute the following commands:

`kubectl port-forward --namespace default svc/my-redis-master 6379:6379 & REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h 127.0.0.1 -p 6379`

### REDIS Commands

`KEYS *`
`GET key`
`FLUSHDB`


## Integrating with Azure Arc

1. Create a Resource Group in Azure.
2. Connect the Kubernetes Cluster to Azure Arc.
   1. Install the Azure CLI with the extension `connectedk8s`.
      1. `curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash`
      2. `az extension add --name connectedk8s`
   2. Login to Azure with `az login --tenant <tenant-id>` or with service principal `az login --service-principal -u <app-id> -p <password> --tenant <tenant-id>`.
   3. `az connectedk8s connect --name microk8s-ubuntu-x64 --resource-group JPRG-ALZ-LZ4`

## Integrate with Flux for GitOps

1. In the Azure Portal, go to the Resource Group and select the Kubernetes Cluster. Under Settings, select GitOps. Select the GitOps provider and follow the instructions to connect to the Git repository.

## Integrate with Azure API Management Self-hosted Gateway

1. Create an API Management instance.
2. In the Azure Portal, go to the API Management instance and select Self-hosted gateways. Select Add gateway and follow the instructions to create the gateway.
3. Go to the Arc Enabled Kubernetes Cluster. Under Extensions, select Add. Select API Management Gateway and follow the instructions to install the gateway in the cluster.

## Things to Add

Redis - Done
Kafka
Config Cat / Azure App Config
App Insights
Linkerd
