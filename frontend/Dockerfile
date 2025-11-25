FROM node:20-alpine AS base
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .

ENV NEXT_PUBLIC_API_BASE_URL="http://api:8080"
ENV PORT=3000

RUN npm run build

EXPOSE 3000

CMD ["npm", "run", "start"]


