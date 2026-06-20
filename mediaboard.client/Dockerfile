FROM node:22-alpine AS base
WORKDIR /app

FROM base AS packages
COPY package*.json .
RUN npm ci

FROM packages AS build
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/ /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]