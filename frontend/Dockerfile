FROM node:20-alpine AS base
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

# This value is used in the browser (client-side fetches), so it must be
# reachable from the user's machine, not from inside the container.
# The API container is exposed on host port 5000 by docker-compose.
ENV NEXT_PUBLIC_API_BASE_URL="http://localhost:5000"
ENV PORT=3000

RUN npm run build

EXPOSE 3000

CMD ["npm", "run", "start"]


