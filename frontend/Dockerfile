# Frontend Dockerfile for production
FROM node:22 AS build
WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm install

# Copy source code and build
COPY . ./
RUN npm run build

# Production image with nginx
FROM nginx:alpine

# TODO: For production deployment, configure Nginx for HTTPS and reverse proxy:
# - Add SSL certificates (Let's Encrypt or corporate certs)
# - Configure nginx.conf with SSL settings and security headers
# - Set up reverse proxy to backend API (replace localhost:5000 with actual backend service)
# - Implement proper error pages and gzip compression
# - Consider adding rate limiting and security headers (HSTS, CSP, etc.)

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
# TODO: Also expose port 443 for HTTPS: EXPOSE 443

CMD ["nginx", "-g", "daemon off;"]
