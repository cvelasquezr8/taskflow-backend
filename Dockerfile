# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código
COPY . ./

# Compilar y publicar en modo Release
RUN dotnet publish -c Release -o /out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar desde la etapa de build
COPY --from=build /out ./

# Puerto expuesto (debe coincidir con el que usas en .env)
EXPOSE 5297

# Iniciar la app
ENTRYPOINT ["dotnet", "task-management-backend.dll"]
