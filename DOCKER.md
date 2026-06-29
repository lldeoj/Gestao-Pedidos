# 🐳 Docker - GestaoPedidos API

Guia completo para executar a aplicação GestaoPedidos com Docker e Docker Compose.

## 📋 Pré-requisitos

- **Docker** 20.10+
- **Docker Compose** 1.29+
- **Docker Desktop** (Windows/Mac) ou Docker Engine (Linux)

Verifique as versões instaladas:

```bash
docker --version
docker-compose --version
```

## 🚀 Início Rápido

### 1. Linux/Mac (Bash)

```bash
# Copiar arquivo de exemplo
cp .env.example .env

# Dar permissão de execução ao script
chmod +x docker-compose.sh

# Iniciar containers
./docker-compose.sh up

# Visualizar logs
./docker-compose.sh logs

# Parar containers
./docker-compose.sh down
```

### 2. Windows (PowerShell)

```powershell
# Copiar arquivo de exemplo
Copy-Item .env.example .env

# Iniciar containers
.\docker-compose.ps1 up

# Visualizar logs
.\docker-compose.ps1 logs

# Parar containers
.\docker-compose.ps1 down
```

### 3. Usando Docker Compose diretamente

```bash
# Iniciar
docker-compose up -d

# Parar
docker-compose down

# Reiniciar
docker-compose restart

# Logs
docker-compose logs -f
```

## 📁 Estrutura de Arquivos Docker

```
GestaoPedidos/
├── Dockerfile                  # Imagem da API
├── .dockerignore              # Arquivos ignorados no build
├── docker-compose.yml         # Configuração de produção
├── docker-compose.override.yml # Configuração de desenvolvimento
├── docker-compose.sh          # Script para Linux/Mac
├── docker-compose.ps1         # Script para Windows
└── .env.example               # Exemplo de variáveis de ambiente
```

## ⚙️ Configuração de Ambiente

### .env (Variáveis de Ambiente)

Crie um arquivo `.env` na raiz do projeto:

```bash
cp .env.example .env
```

Edite as variáveis conforme necessário:

```env
# Ambiente
ASPNETCORE_ENVIRONMENT=Production

# URLs
ASPNETCORE_URLS=http://+:8080

# Banco de Dados
DB_CONNECTION_STRING=Data Source=/app/data/orders.db

# JWT
JWT_KEY=SuperSecretKeyForJWT@2024!@#$%123456
JWT_ISSUER=GestaoPedidos.Service
JWT_AUDIENCE=GestaoPedidos.Service
JWT_EXPIRATION_HOURS=8

# Logging
LOG_LEVEL=Information
```

## 🎯 Comandos Disponíveis

### Via Script (Recomendado)

#### Linux/Mac:
```bash
./docker-compose.sh up          # Iniciar
./docker-compose.sh down        # Parar
./docker-compose.sh restart     # Reiniciar
./docker-compose.sh logs        # Ver logs
./docker-compose.sh build       # Compilar imagem
./docker-compose.sh clean       # Limpar tudo
./docker-compose.sh status      # Status dos containers
./docker-compose.sh shell       # Abrir shell
```

#### Windows:
```powershell
.\docker-compose.ps1 up
.\docker-compose.ps1 down
.\docker-compose.ps1 restart
.\docker-compose.ps1 logs
.\docker-compose.ps1 build
.\docker-compose.ps1 clean
.\docker-compose.ps1 status
.\docker-compose.ps1 shell
```

### Via Docker Compose direto

```bash
# Build
docker-compose build

# Iniciar em background
docker-compose up -d

# Iniciar com output no terminal
docker-compose up

# Parar
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Logs da API
docker-compose logs -f gestao-pedidos-api

# Executar comando no container
docker-compose exec gestao-pedidos-api bash

# Reiniciar um serviço
docker-compose restart gestao-pedidos-api

# Status dos containers
docker-compose ps
```

## 📡 Acessar a Aplicação

### URLs Disponíveis

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **API** | http://localhost:5000 | API REST |
| **Swagger** | http://localhost:5000/swagger | Documentação interativa |
| **Health Check** | http://localhost:5000/health | Status da aplicação |

### Exemplo de Requisição

```bash
# 1. Login
curl -X POST http://localhost:5000/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "dev@martech.com",
	"password": "Senha@123"
  }'

# Resposta:
# {
#   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
#   "expiration": "2024-12-25T10:30:00Z",
#   "isAuthenticated": true
# }

# 2. Usar token
curl -X GET http://localhost:5000/api/orders \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## 🔍 Verificar Logs

```bash
# Logs em tempo real
docker-compose logs -f

# Logs apenas da API
docker-compose logs -f gestao-pedidos-api

# Últimas 100 linhas
docker-compose logs --tail=100

# Logs com timestamp
docker-compose logs -f -t
```

## 🛠️ Desenvolvendo com Docker

### Hot Reload (Desenvolvimento)

O arquivo `docker-compose.override.yml` permite desenvolvimento com hot-reload:

```bash
# Iniciar em modo desenvolvimento
docker-compose up -d

# Os arquivos modificados serão automaticamente recompilados
```

### Acessar o Shell do Container

```bash
# Via script
./docker-compose.sh shell      # Linux/Mac
.\docker-compose.ps1 shell     # Windows

# Direto
docker-compose exec gestao-pedidos-api bash
```

### Instalar Dependências

```bash
docker-compose exec gestao-pedidos-api dotnet add package <package-name>
```

### Executar Testes

```bash
docker-compose exec gestao-pedidos-api dotnet test
```

## 📊 Dados Persistentes

### Banco de Dados SQLite

Os dados são persistidos em um volume Docker:

```bash
# Visualizar volumes
docker volume ls

# Inspecionar volume
docker volume inspect gestao-pedidos_sqlite_data

# Backup do banco
docker run --rm -v gestao-pedidos_sqlite_data:/data -v $(pwd):/backup \
  alpine tar czf /backup/backup-$(date +%Y%m%d_%H%M%S).tar.gz /data/orders.db

# Restaurar backup
docker run --rm -v gestao-pedidos_sqlite_data:/data -v $(pwd):/backup \
  alpine tar xzf /backup/backup-YYYYMMDD_HHMMSS.tar.gz -C /data
```

## 🐛 Troubleshooting

### Container não inicia

```bash
# Verificar logs
docker-compose logs gestao-pedidos-api

# Verificar recursos disponíveis
docker stats

# Remover e recriar
docker-compose down
docker-compose up --build -d
```

### Porta já em uso

```bash
# Verificar qual processo usa a porta
lsof -i :5000  # Linux/Mac
netstat -ano | findstr :5000  # Windows

# Mudar a porta em docker-compose.yml ou .env
ports:
  - "5002:8080"  # Usar porta 5002 ao invés de 5000
```

### Erro de permissão

```bash
# Linux: Dar permissão ao script
chmod +x docker-compose.sh

# Windows: Permitir execução de scripts PowerShell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Banco de dados corrupto

```bash
# Remover volume (CUIDADO: perde dados!)
docker-compose down -v

# Recriar com dados novos
docker-compose up -d
```

## 🔒 Segurança

### Variáveis Sensíveis

1. **NUNCA** commitar `.env` no Git
2. Verificar `.gitignore`:

```
.env
.env.local
.env.*.local
```

3. Usar `.env.example` com valores dummy:

```env
JWT_KEY=CHANGE_ME_IN_PRODUCTION
```

### Secrets do Docker (Produção)

Para produção, use Docker Secrets:

```bash
# Criar secret
echo "seu-secret-seguro" | docker secret create jwt_key -

# Usar em docker-compose
services:
  api:
	secrets:
	  - jwt_key
	environment:
	  JWT_KEY_FILE: /run/secrets/jwt_key
```

## 📦 Build Otimizado

```bash
# Build com cache
docker-compose build

# Build sem cache (rebuild completo)
docker-compose build --no-cache

# Build com variáveis personalizadas
docker-compose build --build-arg BUILD_CONFIGURATION=Release
```

## 🚀 Deployment

### Produção

```bash
# Atualizar imagens
docker-compose pull

# Iniciar com nova versão
docker-compose up -d

# Verificar status
docker-compose ps

# Ver logs
docker-compose logs -f
```

### Stack/Swarm

```bash
# Deploy em Docker Swarm
docker stack deploy -c docker-compose.yml gestao-pedidos

# Verificar stack
docker stack ps gestao-pedidos

# Remover stack
docker stack rm gestao-pedidos
```

## 📚 Recursos Adicionais

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [Best practices for writing Dockerfiles](https://docs.docker.com/develop/develop-images/dockerfile_best-practices/)

## ✅ Checklist de Deployment

- [ ] `.env` configurado com valores de produção
- [ ] JWT_KEY alterado para um valor seguro
- [ ] ASPNETCORE_ENVIRONMENT configurado como Production
- [ ] Logs verificados com `docker-compose logs`
- [ ] Health check retornando OK
- [ ] Backup do banco de dados feito
- [ ] SSL/TLS configurado (se necessário)
- [ ] Firewall configurado para liberar porta 5000

---

**Versão:** 1.0.0  
**Última atualização:** Dezembro 2024
