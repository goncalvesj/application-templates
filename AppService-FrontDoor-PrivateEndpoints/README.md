# Azure Private Endpoint Test App

Sample app to test connectivity to Azure Private Endpoints.

## Infrastructure

[Link to infrastructure repo](https://github.com/goncalvesj/iac-templates/tree/master/Bicep/AppService-FrontDoor-PrivateEndpoints)

## Application

.NET 7 Web API with Swagger UI.

Has 3 REST endpoints:

- `/testsql` - returns `200 OK` and the SQL Server Version if the app can access the SQL private endpoint.
- `/testcache` - returns `200 OK` and set a value in the Redis Cache if the app can access the private endpoint.
- `/teststorage` - returns `200 OK` and creates a container if the app can access the private endpoint.

## How to run

1. Deploy the infrastructure using the infrastructure repo
2. Create a deployment slot in the app service to deploy the app
3. Clone this repo
4. Set the environment variables in the `appsettings.json` file
5. Deploy the app to the deployment slot
6. Swap the deployment slot with the production slot
7. Test using the `/testsql`, `/testcache` and `/teststorage` endpoints

The main slot will only be blocked and only accessible from the Front Door endpoint so to deploy we have to use a deployment slot which is accessible from the internet and then swap.
