# Run Dotnet API

`dotnet run`

## DotNet API

This is a .NET API that retrieves data from a Node.js API and sends data to a Python API. It also includes health checks for both APIs.

## Getting Started

Configure the `appsettings.json` file with the URLs for the Node.js and Python APIs. You can also set up secrets by adding JSON files to a directory and specifying the directory path in the CONFIG_FILES_PATH environment variable.

Run the API with `dotnet run` command.

## Endpoints

- `GET api/data`: Retrieves data from the Node.js API and returns it in JSON format as a Model object.
- `POST api/data`: Sends data to the Python API as a JSON object and returns the response as a string.

## Health Checks

- `/health`: Returns a JSON object with the status of the API and the status of the Node.js and Python APIs. The health checks use the `ApiHealthCheck` class to check the `/api/health` endpoint of the Node.js and Python APIs.

## Dependencies

This API uses the following NuGet packages:

- `Microsoft.Extensions.Diagnostics.HealthChecks`: For implementing health checks.
- `Microsoft.OpenApi.Models`: For generating OpenAPI documentation.
- `System.Net.Http`: For making HTTP requests.
- `System.Text.Json`: For serializing and deserializing JSON data.
