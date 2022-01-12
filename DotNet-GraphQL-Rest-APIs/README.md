# .NET 6 GraphQL & REST API templates

Apps that retrieves Star Wars films data from a JSON file using .NET 6 minimal APIs.

DEMO: TODO

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

- ``docker build -t <repo/imagename>:<tag> .``
- ``docker build -f .\GraphQL.API\Dockerfile -t graphql-api:dev .``

- ``docker run --rm -d -p 8080:80 --name <name> <repo/imagename>:<tag>``
- ``docker run --rm -d -p 8080:80 --name graphapi graphql-api:dev``

### Deploy

TODO
