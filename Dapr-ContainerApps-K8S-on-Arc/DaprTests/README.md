# Dapr Tests

Information about the Dapr tests.

## Infrastructure Repository

Link to the infrastructure repository: <http://someurl>

## Build

`docker compose build`

## Run

Using docker compose:
`docker compose up`

Using the Dapr CLI:

```bash
dapr run --app-port 5088 --app-id dapr-app-1 --app-protocol http --dapr-http-port 3501 -- dotnet run
dapr run --app-port 5019 --app-id dapr-app-2 --app-protocol http --dapr-http-port 3500 -- dotnet run
dapr publish --publish-app-id dapr-app-2 --pubsub pubsub --topic orders --data '{"orderId": "100"}'
```

## Test the endpoints

Open the `tests.http` file in VS Code and try the requests.

## Test the Endpoint in the Containers

Go to the container app console and run:

```bash
# Install curl if needed
apt update && apt install -y curl
apt update && apt install -y curl && curl http://localhost/app2/hello
apt update && apt install -y curl && curl http://localhost/app1/hello
# Replace localhost with the FQDN of the other container
curl http://localhost/app2/hello
curl https://dapr-2.redsmoke-ba3ce026.northeurope.azurecontainerapps.io/app2/hello
curl https://dapr-app-1.thankfulisland-a1881fcf.northeurope.azurecontainerapps.io/app1/hello
nslookup https://dapr-app-1.thankfulisland-a1881fcf.northeurope.azurecontainerapps.io
curl -v http://localhost/app2/order
curl http://localhost/app1/hello
curl -X POST http://localhost/app1/order
curl http://localhost:3500/v1.0/healthz
curl http://localhost:3500/v1.0/metadata
## Deploying a Linux container
az container create -g JPRG-ALZ-LZ2 --name testdns --image ubuntu --command-line "tail -f /dev/null" --subnet  ACI-Subnet --vnet lz2spokevnet
...
```
