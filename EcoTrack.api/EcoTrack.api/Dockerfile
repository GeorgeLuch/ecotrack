# Fase base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Fase para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o arquivo do projeto para a raiz /src
COPY ["EcoTrack.api.csproj", "."] 
RUN dotnet restore "EcoTrack.api.csproj"

# Copia todos os arquivos do projeto para /src
COPY . .

# Compila o projeto usando o arquivo .csproj na raiz /src
RUN dotnet build "EcoTrack.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Fase para publicar a aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EcoTrack.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Fase final para executar a aplicação
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcoTrack.api.dll"]
