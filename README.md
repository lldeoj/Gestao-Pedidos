# GestaoPedidos API

Uma API REST moderna para gestão de pedidos desenvolvida em **.NET 10** com autenticação JWT, arquitetura em camadas e cobertura completa de testes unitários.

## 🚀 Características

- ✅ **Autenticação JWT** - Segurança com tokens JWT
- ✅ **CRUD de Pedidos** - Criar, listar, atualizar e cancelar pedidos
- ✅ **Paginação** - Suporte a paginação em listagens
- ✅ **Validação** - Validação de dados com FluentValidation
- ✅ **Testes Unitários** - 34 testes com xUnit e Moq
- ✅ **Documentação OpenAPI** - Swagger UI integrada
- ✅ **Arquitetura Limpa** - Separação de responsabilidades (Clean Architecture)
- ✅ **CQRS** - Padrão CQRS com MediatR
- ✅ **Database SQLite** - Banco de dados leve e portátil

## 📋 Pré-requisitos

- **.NET 10** SDK ou superior
- **Visual Studio 2022** ou **Visual Studio Code**
- **Git**

## 🛠️ Tecnologias Utilizadas

### Core
- **.NET 10**
- **ASP.NET Core** - Framework web
- **Entity Framework Core** - ORM

### Padrões e Bibliotecas
- **MediatR** - Implementação do padrão CQRS
- **FluentValidation** - Validação fluente
- **Swagger/OpenAPI** - Documentação de API

### Autenticação e Segurança
- **JWT (JSON Web Tokens)** - Autenticação
- **IdentityModel** - Validação de tokens

### Testes
- **xUnit** - Framework de testes
- **Moq** - Mocking
- **EntityFrameworkCore.InMemory** - Testes de banco de dados

### Database
- **SQLite** - Banco de dados relacional

## 📁 Estrutura do Projeto

```
GestaoPedidos/
├── GestaoPedidos/                      # API REST Principal
│   ├── Controllers/
│   │   ├── AuthController.cs           # Endpoints de autenticação
│   │   └── OrdersController.cs         # Endpoints de pedidos
│   ├── Program.cs                      # Configuração da aplicação
│   └── appsettings.json               # Configurações
│
├── GestaoPedidos.Domain/               # Camada de Domínio e Aplicação
│   ├── Domain/
│   │   ├── Entities/                   # Entidades do domínio
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   └── OrderStatus.cs
│   │   └── Exceptions/
│   ├── Application/
│   │   ├── Orders/
│   │   │   ├── Commands/               # Comandos CQRS
│   │   │   │   ├── CreateOrderCommand.cs
│   │   │   │   └── CancelOrderCommand.cs
│   │   │   └── Queries/                # Queries CQRS
│   │   │       ├── GetOrdersQuery.cs
│   │   │       └── GetOrderByIdQuery.cs
│   │   ├── Interfaces/                 # Abstrações
│   │   ├── Models/                     # DTOs
│   │   └── Validation/                 # Validadores
│   └── Infrastructure/
│       ├── Data/
│       │   ├── OrderDbContext.cs       # DbContext
│       │   └── UnitOfWork.cs
│       └── Repositories/
│           └── OrderRepository.cs
│
├── GestaoPedidos.Authentication/       # Camada de Autenticação
│   ├── JwtTokenGenerator.cs            # Gerador de tokens
│   ├── Models/
│   │   ├── LoginRequest.cs
│   │   └── TokenResponse.cs
│   └── Config/
│       └── ServiceCollectionExtensions.cs
│
└── GestaoPedidos.Test/                 # Testes Unitários
	├── UnitTest1.cs                    # Testes JWT
	├── OrderHandlerTests.cs            # Testes CQRS
	├── OrderEntityTests.cs             # Testes de entidades
	└── OrderRepositoryTests.cs         # Testes de repositório
```

## 🔧 Configuração e Execução

### 1. Clonar o repositório

```bash
git clone https://github.com/lldeoj/Gestao-Pedidos.git
cd GestaoPedidos
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Configurar banco de dados

O banco de dados SQLite é criado automaticamente na primeira execução. Verifique `appsettings.json`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=orders.db"
  },
  "Jwt": {
	"Key": "SuperSecretKeyForJWT@2024!@#$%123456",
	"Issuer": "GestaoPedidos.Service",
	"Audience": "GestaoPedidos.Service"
  }
}
```

### 4. Executar a aplicação

```bash
dotnet run --project GestaoPedidos
```

A API estará disponível em: `https://localhost:5000` (ou a porta configurada)

### 5. Acessar o Swagger

Abra no navegador: `https://localhost:5000/swagger`

## 🔐 Autenticação

### Login

Para acessar os endpoints protegidos, primeiro faça login:

```http
POST /auth/login
Content-Type: application/json

{
  "email": "dev@martech.com",
  "password": "Senha@123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2024-12-25T10:30:00Z",
  "isAuthenticated": true
}
```

### Usar o Token

Adicione o token no header de autorização:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**No Swagger:** Clique no botão "Authorize" e cole: `Bearer {seu_token}`

## 📚 Endpoints da API

### Autenticação

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/auth/login` | Fazer login e obter token JWT |

### Pedidos

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| `GET` | `/api/orders` | Listar pedidos com paginação | Requerida |
| `GET` | `/api/orders/{id}` | Obter pedido por ID | Requerida |
| `POST` | `/api/orders` | Criar novo pedido | Requerida |
| `PATCH` | `/api/orders/{id}/cancel` | Cancelar pedido | Requerida |

### Exemplos de Requisições

#### Criar Pedido

```http
POST /api/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerId": "550e8400-e29b-41d4-a716-446655440000",
  "items": [
	{
	  "productName": "Notebook",
	  "quantity": 1,
	  "unitPrice": 2500.00
	},
	{
	  "productName": "Mouse",
	  "quantity": 2,
	  "unitPrice": 50.00
	}
  ]
}
```

#### Listar Pedidos

```http
GET /api/orders?page=1&pageSize=10
Authorization: Bearer {token}
```

#### Obter Pedido por ID

```http
GET /api/orders/550e8400-e29b-41d4-a716-446655440000
Authorization: Bearer {token}
```

#### Cancelar Pedido

```http
PATCH /api/orders/550e8400-e29b-41d4-a716-446655440000/cancel
Authorization: Bearer {token}
```

## ✅ Testes

### Executar todos os testes

```bash
dotnet test
```

### Resultado esperado

```
Execução de teste concluída. 34 teste(s) executado(s). 34 Aprovado, 0 Com Falha
```

### Cobertura de testes

- **JwtTokenGenerator** (7 testes)
  - Geração de token com credenciais válidas
  - Validação de email e senha
  - Verificação de claims e expiração

- **Order Entity** (8 testes)
  - Criação e validação
  - Cálculo de totais
  - Cancelamento

- **OrderItem Entity** (4 testes)
  - Criação com validação
  - Cálculo de preço

- **CQRS Handlers** (7 testes)
  - CreateOrderCommand
  - GetOrdersQuery
  - Tratamento de exceções

- **OrderRepository** (8 testes)
  - CRUD operations
  - Paginação
  - Ordenação

## 🏗️ Arquitetura

### Camadas

```
┌─────────────────────────────────────┐
│      API Layer (Controllers)         │
├─────────────────────────────────────┤
│   Application Layer (CQRS Handlers)  │
├─────────────────────────────────────┤
│    Domain Layer (Entities, Rules)    │
├─────────────────────────────────────┤
│  Infrastructure Layer (Data Access)  │
└─────────────────────────────────────┘
```

### Padrões

- **CQRS** - Separação de Commands e Queries
- **Repository Pattern** - Abstração de dados
- **Dependency Injection** - Injeção de dependências
- **Unit of Work** - Transações
- **Validation Behavior** - Pipeline de validação

## 🐛 Solução de Problemas

### Erro: "no such table: Orders"

O banco de dados não foi inicializado. Execute:

```bash
dotnet run --project GestaoPedidos
```

Isso criará as tabelas automaticamente na primeira execução.

### Erro: "Unauthorized (401)"

Verifique se:
1. ✅ Você realizou o login e obteve um token válido
2. ✅ O token está sendo enviado no header `Authorization: Bearer {token}`
3. ✅ As chaves JWT em `appsettings.json` estão corretas

### Erro: "Invalid token signature"

As chaves JWT não estão sincronizadas. Verifique:

```json
{
  "Jwt": {
	"Key": "SuperSecretKeyForJWT@2024!@#$%123456",
	"Issuer": "GestaoPedidos.Service",
	"Audience": "GestaoPedidos.Service"
  }
}
```

## 📝 Credenciais Padrão

Para testes, use:

- **Email:** `dev@martech.com`
- **Senha:** `Senha@123`

## 🔄 Fluxo de Trabalho

1. **Login** → Obter token JWT
2. **Criar Pedido** → POST `/api/orders`
3. **Listar Pedidos** → GET `/api/orders`
4. **Obter Pedido** → GET `/api/orders/{id}`
5. **Cancelar Pedido** → PATCH `/api/orders/{id}/cancel`

## 🚀 Deploy

### Docker (em desenvolvimento)

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GestaoPedidos.csproj", "."]
RUN dotnet restore "GestaoPedidos.csproj"
COPY . .
RUN dotnet build "GestaoPedidos.csproj" -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "GestaoPedidos.dll"]
```

## 📖 Documentação Adicional

- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://fluentvalidation.net)
- [JWT](https://jwt.io)

## 🤝 Contribuindo

1. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
2. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
3. Push para a branch (`git push origin feature/AmazingFeature`)
4. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

## 👨‍💻 Autor

Desenvolvido como projeto de gestão de pedidos com arquitetura moderna em .NET.

---

**Última atualização:** Dezembro 2024
**Versão:** 1.0.0
**Status:** ✅ Em produção
