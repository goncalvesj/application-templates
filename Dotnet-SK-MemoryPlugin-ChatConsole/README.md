# Intro

This application is a .NET console chat application.

The application replicates a chat application where the AI assistant uses techniques like RAG and automatic function calling to assist the user. 
An example scenario would be an internal chat app where the user can ask questions about the companies expense/travel policies and request a creation of virtual card to use in a trip.

The main goal is to:

- Integrate a semantic kernel memory into the application, which will allow the LLM to remember and recall information from Azure AI Search.
- Demonstrate how to implement plugins in a semantic kernel enabled app.

Requirements:

- a Azure OpenAI Service.
- a Azure AI Search Service.
- some public document to be indexed in the Azure AI Search Service.

## Main Project Structure

### Chat.SK

Main project that contains the chat application. It uses the Semantic Kernel and Kernel Memory nuget packages.

## Running the Application

Steps:

1. Update the variables for Azure Open Ai and AI Search in `Program.cs`.
1. Update location of the location of the public document in `Program.cs` to be imported to the Kernel Memory.											
1. Add an example system prompt.
1. Run with `dotnet run` or `F5`.
