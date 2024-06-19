# Run Node API

`npm run dev`

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
