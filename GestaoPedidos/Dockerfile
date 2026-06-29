FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os .csproj dos 3 projetos antes do restore
COPY ["GestaoPedidos/GestaoPedidos.API.csproj", "GestaoPedidos/"]
COPY ["GestaoPedidos.Authorization/GestaoPedidos.Authentication.csproj", "GestaoPedidos.Authorization/"]
COPY ["GestaoPedidos.Domain/GestaoPedidos.Service.csproj", "GestaoPedidos.Domain/"]

RUN dotnet restore "./GestaoPedidos/GestaoPedidos.API.csproj"

# Copia todo o código-fonte
COPY . .

WORKDIR "/src/GestaoPedidos"
RUN dotnet build "./GestaoPedidos.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GestaoPedidos.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GestaoPedidos.API.dll"]