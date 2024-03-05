from fastapi import FastAPI
from dotenv import load_dotenv
from pydantic import BaseModel
from redis import asyncio as aioredis
from datetime import datetime
import uuid
import os
import logging

# load variables from .env file
load_dotenv()

app = FastAPI()

# Initialize logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

class Data(BaseModel):
    name: str
    age: int
    location: str
    date: datetime


@app.on_event("startup")
async def startup_event():
    # Get Redis configuration details from environment variables
    port_redis = os.environ.get("PORT_REDIS")
    host_redis = os.environ.get("HOST_REDIS")
    password_redis = os.environ.get("PASSWORD_REDIS")
    app_environment = os.environ.get("APP_ENVIRONMENT")
    
    # Initialize the cache with your Redis URL    
    redis_url = f"redis://{host_redis}:{port_redis}"

    # If the app environment is PROD, use a password-protected Redis URL
    if app_environment == "PROD":
        redis_url = f"redis://:{password_redis}@{host_redis}:{port_redis}"

    # Create an async Redis connection object
    redis = await aioredis.from_url(redis_url)
    
    try:
        await redis.ping()
        logger.info("Redis connection successful")
    except:
        logger.error("Failed to connect to Redis")
    # Save the Redis connection to the app state    
    app.state.redis = redis

@app.on_event("shutdown")
async def shutdown_event():
    # Close the Redis connection when the app is shutting down
    await app.state.redis.close()
    await app.state.redis.connection_pool.disconnect()

# Define route for POST request to /api/data
@app.post("/api/data", status_code=201)
async def create_item(data: Data):
    # Store the data in Redis
    redis = app.state.redis
    key = f"data-{uuid.uuid4()}"
    json_data = data.json()
    await redis.set(key, json_data)
    return {"Data Saved": key}

# Define route for GET request to /api/health
@app.get("/api/health")
async def health_check():
    redis = app.state.redis
    try:
        ping_result = await redis.ping()
        logger.info(f"Redis ping result: {ping_result}")
        return {"status": "OK"}
    except:
        logger.error("Failed to ping Redis")
        return {"status": "ERROR"}