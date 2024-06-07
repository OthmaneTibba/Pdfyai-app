# Use the official Node.js image
FROM node:18.16.1 AS builder
WORKDIR /app
COPY package*.json .
RUN npm install
COPY . .
RUN npm run build


# Create the runtime image
FROM nginx:stable-alpine

COPY --from=builder /app/dist/pdfyai/browser /usr/share/nginx/html
COPY ./nginx.conf  /etc/nginx/conf.d/default.conf

EXPOSE 80

