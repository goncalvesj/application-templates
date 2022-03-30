FROM node:alpine AS builder

WORKDIR /app

COPY . .

RUN npm install && npm run build-prod

FROM nginx:alpine

COPY --from=builder /app/dist/SignalR-Web /usr/share/nginx/html/
