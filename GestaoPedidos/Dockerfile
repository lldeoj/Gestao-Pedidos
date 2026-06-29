# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Restaurar dependências
COPY ["GestaoPedidos/GestaoPedidos.API.csproj", "GestaoPedidos/"]
COPY ["GestaoPedidos.Authorization/GestaoPedidos.Authentication.csproj", "GestaoPedidos.Authorization/"]
COPY ["GestaoPedidos.Domain/GestaoPedidos.Service.csproj", "GestaoPedidos.Domain/"]
COPY ["GestaoPedidos.Test/GestaoPedidos.Test.csproj", "GestaoPedidos.Test/"]

RUN dotnet restore "./GestaoPedidos/GestaoPedidos.API.csproj"

# Copiar código-fonte e compilar
COPY . .
WORKDIR "/src/GestaoPedidos"
RUN dotnet build "./GestaoPedidos.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GestaoPedidos.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Criar diretório de dados
RUN mkdir -p /app/data && \
    chown -R app:app /app

USER app

COPY --from=publish /app/publish .

EXPOSE 8080 8081

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "GestaoPedidos.API.dll"]