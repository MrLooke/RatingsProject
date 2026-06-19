FROM node:22-alpine AS base
WORKDIR /app

FROM base AS packages
COPY package*.json .
RUN npm ci

FROM packages AS build
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/ /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]