# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build / Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia todo o projeto primeiro (garante que restore veja tudo o que precisa)
COPY . .

# (higiene) garante que não há lixo local de bin/obj vindo do host
RUN rm -rf ./bin ./obj || true

# Restore completo já com todos os arquivos presentes
RUN dotnet restore EcoTrack.api.csproj

# Publish (já compila), sem restaurar de novo
RUN dotnet publish EcoTrack.api.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EcoTrack.api.dll"]
