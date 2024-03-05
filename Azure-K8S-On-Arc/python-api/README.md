# Getting started

To get started with this API:

1. Clone the repository to your local machine.
2. Install dependencies with `pip install -r requirements.txt`.
3. Create a `.env` file with the required environment variables.
4. Start the API with `uvicorn main:app --reload` or `uvicorn app:app --env-file .env --reload` if using the `.env` file.
5. Navigate to `http://localhost:8000/docs` to view the API documentation.

## Python API

This is a Python API built with [FastAPI](https://fastapi.tiangolo.com/), a modern, fast (high-performance) web framework for building APIs with Python 3.7+.

## Description

- The `FastAPI` module is used to create the `app` instance.
- The `dotenv` module is used to load environment variables from the .`env` file.
- The `BaseModel` class from `pydantic` is used to define the `Data` model which has four properties: `name`, `age`, `location`, and `date`.
- The `aioredis` module is used to create an async Redis connection object to be used as a caching layer.
- The `startup_event` function is called when the app starts up, which initializes the Redis connection object and saves it to the app state.
- The `shutdown_event` function is called when the app is shutting down, which closes the Redis connection.
- The `create_item` function is a route that accepts a POST request at the endpoint `/api/data`. It creates a unique key, stores the JSON representation of the `Data` object in Redis using the key, and returns a response indicating success.
- The `health_check` function is a route that accepts a GET request at the endpoint `/api/health`. It checks the Redis connection by sending a PING command and returns a response indicating success.

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
