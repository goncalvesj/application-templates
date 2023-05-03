# Things to Add

Redis - Done
Kafka
Config Cat / Azure App Config
Linkerd
Argo

# Run Dotnet API

`dotnet run`

# Run Node API

`npm run dev`

# Run Python API

`pip install -r requirements.txt`
`uvicorn app:app --reload`
`uvicorn app:app --env-file PATH --reload`
`uvicorn app:app --env-file .env --reload`

# Other

`docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password01" -p 1433:1433 --name sql --hostname sql -d mcr.microsoft.com/mssql/server:2022-latest`

`my-redis-master.default.svc.cluster.local for read/write operations (port 6379)`
`my-redis-replicas.default.svc.cluster.local for read-only operations (port 6379)`

`export REDIS_PASSWORD=$(kubectl get secret --namespace default my-redis -o jsonpath="{.data.redis-password}" | base64 -d)`
`REDIS_PASSWORD=ZUlahBR273`

To connect to your Redis&reg; server:

1. Run a Redis&reg; pod that you can use as a client:

`kubectl run --namespace default redis-client --restart='Never'  --env REDIS_PASSWORD=$REDIS_PASSWORD  --image docker.io/bitnami/redis:7.0.9-debian-11-r1 --command -- sleep infinity`

Use the following command to attach to the pod:

`kubectl exec --tty -i redis-client --namespace default -- bash`

2. Connect using the Redis&reg; CLI:

`REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h my-redis-master`
`REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h my-redis-replicas`

To connect to your database from outside the cluster execute the following commands:

`kubectl port-forward --namespace default svc/my-redis-master 6379:6379 & REDISCLI_AUTH="$REDIS_PASSWORD" redis-cli -h 127.0.0.1 -p 6379`

## REDIS Commands

`KEYS *`
`GET key`
`FLUSHDB`

### Python API Description

Python API that uses the FastAPI framework to build a RESTful API. It also uses Pydantic to define a data model and Redis as a caching layer.

- The `FastAPI` module is used to create the `app` instance.
- The `dotenv` module is used to load environment variables from the .`env` file.
- The `BaseModel` class from `pydantic` is used to define the `Data` model which has four properties: `name`, `age`, `location`, and `date`.
- The `aioredis` module is used to create an async Redis connection object to be used as a caching layer.
- The `startup_event` function is called when the app starts up, which initializes the Redis connection object and saves it to the app state.
- The `shutdown_event` function is called when the app is shutting down, which closes the Redis connection.
- The `create_item` function is a route that accepts a POST request at the endpoint `/api/data`. It creates a unique key, stores the JSON representation of the `Data` object in Redis using the key, and returns a response indicating success.
- The `health_check` function is a route that accepts a GET request at the endpoint `/api/health`. It checks the Redis connection by sending a PING command and returns a response indicating success.

Overall, this code demonstrates how to use FastAPI to create a RESTful API with Redis as a caching layer, which can improve the performance of the API by reducing the amount of time it takes to fetch data from a database.

## Node.js API

The code defines an Express router for handling HTTP requests to specific API endpoints. It uses the Redis database to store and retrieve data.

### Endpoints

The API exposes two endpoints:

- `/api/data/:key` for retrieving data from Redis by key
- `/api/health` for performing a health check on the Redis database

The code uses Swagger annotations to define the API's routes and parameters.

### Redis Configuration

When creating the Redis client, the code checks the configuration options for the environment to use the correct Redis URL. If the Redis client fails to connect, the code logs an error message and shuts down the client.

### `/api/data/:key` Endpoint

When handling requests to the `/api/data/:key` endpoint, the code retrieves the key from the request URL, retrieves the data from Redis, and sends the response with the retrieved data or an error message.

### `/api/health` Endpoint

When handling requests to the `/api/health` endpoint, the code sends a Redis PING command and returns a status of OK if the Redis client responds correctly.

### `getRouter()` Method

The `getRouter()` method returns the Express router instance for use in the main application code.

# Python API

This is a Python API built with [FastAPI](https://fastapi.tiangolo.com/), a modern, fast (high-performance) web framework for building APIs with Python 3.7+.

## Endpoints

The API exposes the following endpoints:

- `POST /api/data`: Creates a new data item and stores it in Redis.
- `GET /api/health`: Returns the health status of the Redis connection.

## Dependencies

The API has the following dependencies:

- [Python 3.7+](https://www.python.org/downloads/)
- [FastAPI](https://fastapi.tiangolo.com/)
- [pydantic](https://pydantic-docs.helpmanual.io/)
- [aioredis](https://aioredis.readthedocs.io/)
- [python-dotenv](https://pypi.org/project/python-dotenv/)

## Environment variables

The following environment variables need to be set:

- `PORT_REDIS`: Redis port number.
- `HOST_REDIS`: Redis host address.
- `PASSWORD_REDIS`: Redis password (if using password authentication).
- `APP_ENVIRONMENT`: App environment (`PROD` for production, `DEV` for development).

## Getting started

To get started with this API:

1. Clone the repository to your local machine.
2. Install dependencies with `pip install -r requirements.txt`.
3. Create a `.env` file with the required environment variables.
4. Start the API with `uvicorn main:app --reload`.
5. Navigate to `http://localhost:8000/docs` to view the API documentation.

# DotNet API

This is a .NET API that retrieves data from a Node.js API and sends data to a Python API. It also includes health checks for both APIs.

## Getting Started

Configure the appsettings.json file with the URLs for the Node.js and Python APIs. You can also set up secrets by adding JSON files to a directory and specifying the directory path in the CONFIG_FILES_PATH environment variable.

Run the API with `dotnet run` command.

## Endpoints

- `GET api/data`: Retrieves data from the Node.js API and returns it in JSON format as a Model object.
- `POST api/data`: Sends data to the Python API as a JSON object and returns the response as a string.

## Health Checks

- `/health`: Returns a JSON object with the status of the API and the status of the Node.js and Python APIs. The health checks use the `ApiHealthCheck` class to check the `/api/health` endpoint of the Node.js and Python APIs.

## Dependencies

This API uses the following NuGet packages:

- Microsoft.Extensions.Diagnostics.HealthChecks: For implementing health checks.
- Microsoft.OpenApi.Models: For generating OpenAPI documentation.
- System.Net.Http: For making HTTP requests.
- System.Text.Json: For serializing and deserializing JSON data.
