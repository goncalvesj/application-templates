# Intro

This application is a .NET solution that mocks a platform for hotel booking.

The main goal is to:

- Demonstrate how to implement plugins for ChatGPT where the LLM queries our API to give more relevant information to user.
- Have a prompt in our website that uses Azure OpenAI to filter the results for the user using Semantic Kernel.

Requires a valid Azure OpenAI service to run. Necessary to populate the following environment variables:

- `Environment.GetEnvironmentVariable("aoai-deployment-name")`
- `Environment.GetEnvironmentVariable("aoai-aoai-endpoint")`
- `Environment.GetEnvironmentVariable("aoai-key")`

## Main Project Structure

### HotelBooking.Api

This is the API project for the hotel booking application, written in C#. It contains the logic for handling hotel booking related operations.
The `HotelData.cs` file contains the data model for hotels in the application as well as semantic functions.
Configuration is done by environment variables.

### HotelBooking.Web

This project is the web front-end for the application. Built with Blazor.
The `UI` folder contains the same UI but built with React.

### WeatherForecast.Api

This is another API project in the solution for testing plugins. It provides weather forecast data based on latitude and longitude coordinates.

### PluginShared

This project contains code that is shared between the projects in the solution.

## Implementing Plugins for ChatGPT

In the context of ChatGPT, a plugin is a piece of software that adds specific features to an existing application—ChatGPT in this case. Plugins enable custom handling of additional tasks. In the provided code, a plugin for hotel booking is implemented.

The plugin is implemented as a web API using ASP.NET Core.

### Swagger

Swagger is used to document the API. It provides a user-friendly interface to interact with the API and understand its capabilities.
For ChatGPT plugins, the Swagger file needs to be in the `yaml` format.

### CORS

CORS (Cross-Origin Resource Sharing) is a mechanism that allows many resources (e.g., fonts, JavaScript, etc.) on a web page to be requested from another domain outside the domain from which the resource originated.
For ChatGPT plugins, we need to allow CORS requests from the ChatGPT domain `https://chat.openai.com`.

### Plugin Manifest

The plugin manifest is a JSON file that provides information about the plugin to ChatGPT. It includes the name of the plugin, a description, authentication information, the API type and URL, and a logo URL. This information is returned from the `/.well-known/ai-plugin.json` endpoint.

The plugin manifest is created in the `MapGet` method, which maps a GET request to the specified URL path (`/.well-known/ai-plugin.json`) to the specified callback function. The callback function creates a new `PluginManifest` object and returns it.

The `ExcludeFromDescription` method is used to exclude this endpoint from the Swagger documentation, as it's intended to be used by ChatGPT, not by users interacting with the Swagger UI.

## Running the Application

To replicate a ChatGPT plugin, you can run this project locally: `https://github.com/microsoft/chat-copilot`. It requires Azure OpenAI credentials to run.

Steps:

1. Run either `HotelBooking.Api` or the `WeatherForecast.Api` project.
2. Add the plugins to Chat Copilot by following the instructions in the [Chat Copilot documentation](https://learn.microsoft.com/en-us/semantic-kernel/chat-copilot/testing-plugins-with-chat-copilot)
3. Start chatting with Chat Copilot and see the results. You can ask questions like "What is the weather in Seattle?" or "I'm planning a trip. can you show me a list of hotels please with a minimum of 4 stars please.".
