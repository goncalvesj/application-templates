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

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /usr/src/app
USER appuser

EXPOSE 3000
CMD ["node", "index.js"]

# RUN npm install pm2 -g
# EXPOSE 80
# CMD ["pm2-runtime","index.js"]
