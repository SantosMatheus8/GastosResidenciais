# === Estágio 1: Build da aplicação React ===
FROM node:20-alpine AS build

WORKDIR /app

# Copia arquivos de dependências e instala
COPY package.json package-lock.json ./
RUN npm ci

# Copia código-fonte e builda
COPY . .

# VITE_API_URL vazio = requisições relativas (nginx faz proxy para o backend)
ENV VITE_API_URL=
RUN npm run build

# === Estágio 2: Servir com nginx ===
FROM nginx:alpine

# Remove a config padrão do nginx e copia a customizada
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copia os arquivos buildados para o diretório do nginx
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
