/**
 * Required External Modules
 */
import * as dotenv from 'dotenv';
import express from 'express';
import cors from 'cors';
import helmet from 'helmet';

import swaggerUi from 'swagger-ui-express';
import swaggerJsdoc from 'swagger-jsdoc';
import { swaggerOptions } from './swaggerOptions';
import { apiRoutes } from './router';
import { Config } from './models';

dotenv.config();
/**
 * App Variables
 */
if (!process.env.PORT) {
  process.exit(1);
}

const PORT: number = parseInt(process.env.PORT as string, 10);

const config: Config = {
  APP_ENVIRONMENT: process.env.APP_ENVIRONMENT as string,
  HOST_REDIS: process.env.HOST_REDIS as string,
  PASSWORD_REDIS: process.env.PASSWORD_REDIS as string,
  PORT_REDIS: parseInt(process.env.PORT_REDIS as string, 10),
};

const app = express();
const specs = swaggerJsdoc(swaggerOptions);

const router = new apiRoutes(config);
/**
 *  App Configuration
 */
app.use(helmet());
app.use(cors());
app.use(express.json());
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(specs));
app.use('/api', router.getRouter());

/**
 * Server Activation
 */
app.listen(PORT, () => {
  console.log(`Listening on port ${PORT}`);
});
