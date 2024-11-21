# .NET 9 GraphQL & REST API templates

Apps that retrieves Star Wars films data from a JSON file using .NET 6 minimal APIs.

TODO:

- Build demo environment

## Infrastructure Class

Class library shared between the 2 APIs for JSON data retrieval.
JSON file in project root folder.

## GraphQL API

Uses:

- GraphQL.Net
- GraphQL Altair for UI visualization and testing

GraphQL folder has the GraphQL Types/Schema/Query classes.

## REST API

Uses:

- Swagger for UI visualization and testing

### Docker commands

```powershell
docker build -t <repo/imagename>:<tag> .
docker build -f .\GraphQL.API\Dockerfile -t graphql-api:dev .
docker build -f .\Rest.API\Dockerfile -t rest-api:dev .

docker run --rm -d -p 8080:80 --name <name> <repo/imagename>:<tag>
docker run --rm -d -p 8080:80 --name graphapi graphql-api:dev
docker run --rm -d -p 8080:80 --name restapi rest-api:dev
```

```powershell
$ACR_NAME="myacr"
$ACR_PWD="XXXX"
helm registry login $ACR_NAME.azurecr.io --username $ACR_NAME --password $ACR_PWD
helm push sw-restapi-0.1.0.tgz oci://$ACR_NAME.azurecr.io/helm
helm install myhelmtest oci://$ACR_NAME.azurecr.io/helm/hello-world --version 0.1.0
```

### Deploy

TODO:

- Update Helm Charts
- Add deploy step in pipeline
