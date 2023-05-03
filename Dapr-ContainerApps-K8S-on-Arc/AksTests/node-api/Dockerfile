# Build stage
FROM node:lts-alpine AS build
WORKDIR /usr/src/app

COPY node-api/package.json ./
COPY node-api/tsconfig.json ./
COPY node-api/src ./src

RUN npm install
RUN npm run build

# Production stage
FROM node:lts-alpine
ENV NODE_ENV=production
WORKDIR /usr/src/app

COPY node-api/package.json ./
COPY node-api/.env ./
COPY --from=build /usr/src/app/build .
RUN npm install --production --silent && mv node_modules ../

RUN chown -R node /usr/src/app
USER node
EXPOSE 3000
CMD ["node", "index.js"]

# RUN npm install pm2 -g
# EXPOSE 80
# CMD ["pm2-runtime","index.js"]
