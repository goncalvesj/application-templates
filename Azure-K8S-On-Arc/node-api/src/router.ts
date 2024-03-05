import express, { Request, Response } from 'express';
import * as redis from 'redis';
import { Config, Model } from './models';

export class apiRoutes {
  private router = express.Router();
  private redisClient!: redis.RedisClientType;

  constructor(config: Config) {
    console.log(config);

    this.initRedis(config);
    this.setApiRoutes();
    this.setHealthCheck();
  }

  private initRedis(config: Config) {
    var redisUrl = `redis://${config.HOST_REDIS}:${config.PORT_REDIS}`;
    if (config.APP_ENVIRONMENT === 'PROD')
      redisUrl = `redis://:${config.PASSWORD_REDIS}@${config.HOST_REDIS}:${config.PORT_REDIS}`;

    this.redisClient = redis.createClient({
      url: redisUrl,
    });

    this.redisClient.on('connect', () => {
      console.log('Redis client connected');
    });

    this.redisClient.on('error', (err) => {
      if (err.code === 'ECONNREFUSED') {
        console.log('Error connecting to Redis:', err);
        // handle the ECONNREFUSED error here
        this.redisClient.quit();
      }
    });
    this.redisClient.connect();
  }

  private setApiRoutes() {
    /**
     * @swagger
     * /api/data/{key}:
     *   get:
     *     summary: Returns data from the cache
     *     parameters:
     *      - in: path
     *        name: key
     *     responses:
     *       200:
     *         description: A list of items
     *         content:
     *           application/json:
     *             schema:
     *               type: array
     *               items:
     *                 type: number
     */
    this.router.get('/data/:key', async (req: Request, res: Response) => {
      try {
        const key = req.params.key; // retrieve the key from the route parameter
        if (typeof key !== 'string') {
          throw new Error('Invalid key parameter');
        }

        const data = await this.redisClient.get(key);
        if (data === null) {
          throw new Error('Data not found');
        }
        const items: Model = JSON.parse(data) as Model;

        res.status(200).send(items);
      } catch (e: unknown) {
        res.status(500).send(e);
      }
    });
  }

  private setHealthCheck() {
    /**
     * @swagger
     * /api/health:
     *   get:
     *     summary: Returns a health check
     *     responses:
     *      200:
     *       description: A health check
     */
    this.router.get('/health', async (req: Request, res: Response) => {
      try {
        // Do some health check
        // PING command
        console.log('\nCache command: PING');
        console.log('Cache response : ' + (await this.redisClient.ping()));
        res.status(200).send({ status: 'OK' });
      } catch (e: unknown) {
        res.status(500).send(e);
      }
    });
  }

  public getRouter() {
    return this.router;
  }
}
