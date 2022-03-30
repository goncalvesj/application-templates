# Angular + Serverless SignalR using Azure Functions

Simple chat app using an angular 8 app with azure ``signalr`` service and azure functions.

DEMO: <https://angular-signalr.jpcloud.tk/>

## Azure Functions

Contains 2 functions

- Negotiate (initiates the ``signalr`` connection)
- Send Message (sends message to an user)

Create ``local.settings.json`` file with the below settings and start the function.

```JSON
{
  "IsEncrypted": false,
  "Values": {
    "AzureSignalRConnectionString": "CONN-STRING-HERE;",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
  },
  "Host": {
    "CORS": "http://localhost:4200",
    "CORSCredentials": true
  }
}
```

- Start functions in Visual Studio.

## Angular App

The web client used to initiate the chat.

Update env.js file with the API URL.

```JS
(function(window) {
  window.__env = window.__env || {};

  window.__env.signalrUrl = 'http://localhost:7071/api';
})(this);
```

- Run ```npm i | ng serve``` in VS Code
- Open browser and test

### Docker commands for Angular App

- ``docker build -t <repo/imagename>:<tag> .``
- ``docker run --rm -d -p 8080:80 --name <name> <repo/imagename>:<tag>``

### Deploy

See pipelines ``yaml`` files.

- Build and Pushes docker images
- Generates Helm charts and pushes to AKS
